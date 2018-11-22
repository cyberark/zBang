##################################################################################
# The script querys all the DCs in the forest and try to connect with them through kerberos with AES256 encryption type.
# If the DC is in the updated Domain Function Level and not supported AES256 encryption - It might be infected with Skeleton Key malware! You need to check this DC.

# The script is based on PowerView, PyKek, Evil_DC projects from -> Will Schroeder, Sylvain Monne and Ian Haken:
# https://github.com/PowerShellMafia/PowerSploit/tree/master/Recon
# https://github.com/bidord/pykek # It was compiled to exe file to work without python on the target machine
# https://github.com/JackOfMostTrades/bluebox/blob/master/evil_dc/kdc/evil_server.py # It was compiled to exe file to work without python on the target machine
# You are welcome to take a look on those sites for more information and their projects' full functionalities


##################################################################################
# Here are some few reconnaissance functions from PowerView - to get the DCs list:
##################################################################################
# Author of PowerView: Will Schroeder (@harmj0y)

function Get-NetDomainController {
<#
    .SYNOPSIS

        Return the current domain controllers for the active domain.

    .PARAMETER Domain

        The domain to query for domain controllers, defaults to the current domain.

    .PARAMETER DomainController

        Domain controller to reflect LDAP queries through.

    .PARAMETER LDAP

        Switch. Use LDAP queries to determine the domain controllers.

    .EXAMPLE

        PS C:\> Get-NetDomainController -Domain test
#>

    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline=$True)]
        [String]
        $Domain,

        [String]
        $DomainController,

        [Switch]
        $LDAP
    )

    process {
        if($LDAP -or $DomainController) {
            # filter string to return all domain controllers
            Get-NetComputer -Domain $Domain -DomainController $DomainController -FullData -Filter '(userAccountControl:1.2.840.113556.1.4.803:=8192)'
        }
        else {
            $FoundDomain = Get-NetDomain -Domain $Domain
            
            if($FoundDomain) {
                $Founddomain.DomainControllers
            }
        }
    }
}


function Get-DomainSearcher {
<#
    .SYNOPSIS

        Helper used by various functions that takes an ADSpath and
        domain specifier and builds the correct ADSI searcher object.

    .PARAMETER Domain

        The domain to use for the query, defaults to the current domain.

    .PARAMETER DomainController

        Domain controller to reflect LDAP queries through.

    .PARAMETER ADSpath

        The LDAP source to search through, e.g. "LDAP://OU=secret,DC=testlab,DC=local"
        Useful for OU queries.

    .PARAMETER ADSprefix

        Prefix to set for the searcher (like "CN=Sites,CN=Configuration")

    .PARAMETER PageSize

        The PageSize to set for the LDAP searcher object.

    .EXAMPLE

        PS C:\> Get-DomainSearcher -Domain testlab.local

    .EXAMPLE

        PS C:\> Get-DomainSearcher -Domain testlab.local -DomainController SECONDARY.dev.testlab.local
#>

    [CmdletBinding()]
    param(
        [String]
        $Domain,

        [String]
        $DomainController,

        [String]
        $ADSpath,

        [String]
        $ADSprefix,

        [ValidateRange(1,10000)] 
        [Int]
        $PageSize = 200
    )

    if(!$Domain) {
        $Domain = (Get-NetDomain).name
    }
    else {
        if(!$DomainController) {
            try {
                # if there's no -DomainController specified, try to pull the primary DC
                #   to reflect queries through
                $DomainController = ((Get-NetDomain).PdcRoleOwner).Name
            }
            catch {
                throw "Get-DomainSearcher: Error in retrieving PDC for current domain"
            }
        }
    }

    $SearchString = "LDAP://"

    if($DomainController) {
        $SearchString += $DomainController + "/"
    }
    if($ADSprefix) {
        $SearchString += $ADSprefix + ","
    }

    if($ADSpath) {
        if($ADSpath -like "GC://*") {
            # if we're searching the global catalog
            $DistinguishedName = $AdsPath
            $SearchString = ""
        }
        else {
            if($ADSpath -like "LDAP://*") {
                $ADSpath = $ADSpath.Substring(7)
            }
            $DistinguishedName = $ADSpath
        }
    }
    else {
        $DistinguishedName = "DC=$($Domain.Replace('.', ',DC='))"
    }

    $SearchString += $DistinguishedName
    Write-Verbose "Get-DomainSearcher search string: $SearchString"

    $Searcher = New-Object System.DirectoryServices.DirectorySearcher([ADSI]$SearchString)
    $Searcher.PageSize = $PageSize
    $Searcher
}


function Get-NetDomain {
<#
    .SYNOPSIS

        Returns a given domain object.

    .PARAMETER Domain

        The domain name to query for, defaults to the current domain.

    .EXAMPLE

        PS C:\> Get-NetDomain -Domain testlab.local

    .LINK

        http://social.technet.microsoft.com/Forums/scriptcenter/en-US/0c5b3f83-e528-4d49-92a4-dee31f4b481c/finding-the-dn-of-the-the-domain-without-admodule-in-powershell?forum=ITCG
#>

    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline=$True)]
        [String]
        $Domain
    )

    process {
        if($Domain) {
            $DomainContext = New-Object System.DirectoryServices.ActiveDirectory.DirectoryContext('Domain', $Domain)
            try {
                [System.DirectoryServices.ActiveDirectory.Domain]::GetDomain($DomainContext)
            }
            catch {
                Write-Warning "The specified domain $Domain does not exist, could not be contacted, or there isn't an existing trust."
                $Null
            }
        }
        else {
            [System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
        }
    }
}


function Get-NetComputer {
<#
    .SYNOPSIS

        This function utilizes adsisearcher to query the current AD context
        for current computer objects. Based off of Carlos Perez's Audit.psm1
        script in Posh-SecMod (link below).

    .PARAMETER ComputerName

        Return computers with a specific name, wildcards accepted.

    .PARAMETER SPN

        Return computers with a specific service principal name, wildcards accepted.

    .PARAMETER OperatingSystem

        Return computers with a specific operating system, wildcards accepted.

    .PARAMETER ServicePack

        Return computers with a specific service pack, wildcards accepted.

    .PARAMETER Filter

        A customized ldap filter string to use, e.g. "(description=*admin*)"

    .PARAMETER Printers

        Switch. Return only printers.

    .PARAMETER Ping

        Switch. Ping each host to ensure it's up before enumerating.

    .PARAMETER FullData

        Switch. Return full computer objects instead of just system names (the default).

    .PARAMETER Domain

        The domain to query for computers, defaults to the current domain.

    .PARAMETER DomainController

        Domain controller to reflect LDAP queries through.

    .PARAMETER ADSpath

        The LDAP source to search through, e.g. "LDAP://OU=secret,DC=testlab,DC=local"
        Useful for OU queries.

    .PARAMETER Unconstrained

        Switch. Return computer objects that have unconstrained delegation.

    .PARAMETER PageSize

        The PageSize to set for the LDAP searcher object.

    .EXAMPLE

        PS C:\> Get-NetComputer
        
        Returns the current computers in current domain.

    .EXAMPLE

        PS C:\> Get-NetComputer -SPN mssql*
        
        Returns all MS SQL servers on the domain.

    .EXAMPLE

        PS C:\> Get-NetComputer -Domain testing
        
        Returns the current computers in 'testing' domain.

    .EXAMPLE

        PS C:\> Get-NetComputer -Domain testing -FullData
        
        Returns full computer objects in the 'testing' domain.

    .LINK

        https://github.com/darkoperator/Posh-SecMod/blob/master/Audit/Audit.psm1
#>

    [CmdletBinding()]
    Param (
        [Parameter(ValueFromPipeline=$True)]
        [Alias('HostName')]
        [String]
        $ComputerName = '*',

        [String]
        $SPN,

        [String]
        $OperatingSystem,

        [String]
        $ServicePack,

        [String]
        $Filter,

        [Switch]
        $Printers,

        [Switch]
        $Ping,

        [Switch]
        $FullData,

        [String]
        $Domain,

        [String]
        $DomainController,

        [String]
        $ADSpath,

        [Switch]
        $Unconstrained,

        [ValidateRange(1,10000)] 
        [Int]
        $PageSize = 200
    )

    begin {
        # so this isn't repeated if users are passed on the pipeline
        $CompSearcher = Get-DomainSearcher -Domain $Domain -DomainController $DomainController -ADSpath $ADSpath -PageSize $PageSize
    }

    process {

        if ($CompSearcher) {

            # if we're checking for unconstrained delegation
            if($Unconstrained) {
                Write-Verbose "Searching for computers with for unconstrained delegation"
                $Filter += "(userAccountControl:1.2.840.113556.1.4.803:=524288)"
            }
            # set the filters for the seracher if it exists
            if($Printers) {
                Write-Verbose "Searching for printers"
                # $CompSearcher.filter="(&(objectCategory=printQueue)$Filter)"
                $Filter += "(objectCategory=printQueue)"
            }
            if($SPN) {
                Write-Verbose "Searching for computers with SPN: $SPN"
                $Filter += "(servicePrincipalName=$SPN)"
            }
            if($OperatingSystem) {
                $Filter += "(operatingsystem=$OperatingSystem)"
            }
            if($ServicePack) {
                $Filter += "(operatingsystemservicepack=$ServicePack)"
            }

            $CompSearcher.filter = "(&(sAMAccountType=805306369)(dnshostname=$ComputerName)$Filter)"

            try {

                $CompSearcher.FindAll() | Where-Object {$_} | ForEach-Object {
                    $Up = $True
                    if($Ping) {
                        # TODO: how can these results be piped to ping for a speedup?
                        $Up = Test-Connection -Count 1 -Quiet -ComputerName $_.properties.dnshostname
                    }
                    if($Up) {
                        # return full data objects
                        if ($FullData) {
                            # convert/process the LDAP fields for each result
                            Convert-LDAPProperty -Properties $_.Properties
                        }
                        else {
                            # otherwise we're just returning the DNS host name
                            $_.properties.dnshostname
                        }
                    }
                }
            }
            catch {
                Write-Warning "Error: $_"
            }
        }
    }
}


function Convert-LDAPProperty {
    # helper to convert specific LDAP property result fields
    param(
        [Parameter(Mandatory=$True,ValueFromPipeline=$True)]
        [ValidateNotNullOrEmpty()]
        $Properties
    )

    $ObjectProperties = @{}

    $Properties.PropertyNames | ForEach-Object {
        if (($_ -eq "objectsid") -or ($_ -eq "sidhistory")) {
            # convert the SID to a string
            $ObjectProperties[$_] = (New-Object System.Security.Principal.SecurityIdentifier($Properties[$_][0],0)).Value
        }
        elseif($_ -eq "objectguid") {
            # convert the GUID to a string
            $ObjectProperties[$_] = (New-Object Guid (,$Properties[$_][0])).Guid
        }
        elseif( ($_ -eq "lastlogon") -or ($_ -eq "lastlogontimestamp") -or ($_ -eq "pwdlastset") -or ($_ -eq "lastlogoff") -or ($_ -eq "badPasswordTime") ) {
            # convert timestamps
            if ($Properties[$_][0] -is [System.MarshalByRefObject]) {
                # if we have a System.__ComObject
                $Temp = $Properties[$_][0]
                [Int32]$High = $Temp.GetType().InvokeMember("HighPart", [System.Reflection.BindingFlags]::GetProperty, $null, $Temp, $null)
                [Int32]$Low  = $Temp.GetType().InvokeMember("LowPart",  [System.Reflection.BindingFlags]::GetProperty, $null, $Temp, $null)
                $ObjectProperties[$_] = ([datetime]::FromFileTime([Int64]("0x{0:x8}{1:x8}" -f $High, $Low)))
            }
            else {
                $ObjectProperties[$_] = ([datetime]::FromFileTime(($Properties[$_][0])))
            }
        }
        elseif($Properties[$_][0] -is [System.MarshalByRefObject]) {
            # convert misc com objects
            $Prop = $Properties[$_]
            try {
                $Temp = $Prop[$_][0]
                Write-Verbose $_
                [Int32]$High = $Temp.GetType().InvokeMember("HighPart", [System.Reflection.BindingFlags]::GetProperty, $null, $Temp, $null)
                [Int32]$Low  = $Temp.GetType().InvokeMember("LowPart",  [System.Reflection.BindingFlags]::GetProperty, $null, $Temp, $null)
                $ObjectProperties[$_] = [Int64]("0x{0:x8}{1:x8}" -f $High, $Low)
            }
            catch {
                $ObjectProperties[$_] = $Prop[$_]
            }
        }
        elseif($Properties[$_].count -eq 1) {
            $ObjectProperties[$_] = $Properties[$_][0]
        }
        else {
            $ObjectProperties[$_] = $Properties[$_]
        }
    }

    New-Object -TypeName PSObject -Property $ObjectProperties
}


function Get-NetForestDomain {
<#
    .SYNOPSIS

        Return all domains for a given forest.

    .PARAMETER Forest

        The forest name to query domain for.

    .PARAMETER Domain

        Return domains that match this term/wildcard.

    .EXAMPLE

        PS C:\> Get-NetForestDomain

    .EXAMPLE

        PS C:\> Get-NetForestDomain -Forest external.local
#>

    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline=$True)]
        [String]
        $Forest,

        [String]
        $Domain
    )

    process {
        if($Domain) {
            # try to detect a wild card so we use -like
            if($Domain.Contains('*')) {
                (Get-NetForest -Forest $Forest).Domains | Where-Object {$_.Name -like $Domain}
            }
            else {
                # match the exact domain name if there's not a wildcard
                (Get-NetForest -Forest $Forest).Domains | Where-Object {$_.Name.ToLower() -eq $Domain.ToLower()}
            }
        }
        else {
            # return all domains
            $ForestObject = Get-NetForest -Forest $Forest
            if($ForestObject) {
                $ForestObject.Domains
            }
        }
    }
}

function Get-NetForest {
<#
    .SYNOPSIS

        Returns a given forest object.

    .PARAMETER Forest

        The forest name to query for, defaults to the current domain.

    .EXAMPLE
    
        PS C:\> Get-NetForest -Forest external.domain
#>

    [CmdletBinding()]
    param(
        [Parameter(ValueFromPipeline=$True)]
        [String]
        $Forest
    )

    process {
        if($Forest) {
            $ForestContext = New-Object System.DirectoryServices.ActiveDirectory.DirectoryContext('Forest', $Forest)
            try {
                $ForestObject = [System.DirectoryServices.ActiveDirectory.Forest]::GetForest($ForestContext)
            }
            catch {
                Write-Debug "The specified forest $Forest does not exist, could not be contacted, or there isn't an existing trust."
                $Null
            }
        }
        else {
            # otherwise use the current forest
            $ForestObject = [System.DirectoryServices.ActiveDirectory.Forest]::GetCurrentForest()
        }

        if($ForestObject) {
            # get the SID of the forest root
            $ForestSid = (New-Object System.Security.Principal.NTAccount($ForestObject.RootDomain,"krbtgt")).Translate([System.Security.Principal.SecurityIdentifier]).Value
            $Parts = $ForestSid -Split "-"
            $ForestSid = $Parts[0..$($Parts.length-2)] -join "-"
            $ForestObject | Add-Member NoteProperty 'RootDomainSid' $ForestSid
            $ForestObject
        }
    }
}


########################################################################################
# The main function
########################################################################################

if ($PSVersionTable.PSVersion.Major -eq 2){ 
    $PSScriptRoot =  split-path $MyInvocation.MyCommand.Path -Parent
}

$ScriptPath = $PSScriptRoot

Add-Type -AssemblyName System.IO.Compression.FileSystem
$unzipfile = $ScriptPath + "\scanner\SkeletonCheck.exe"
if (Test-Path $unzipfile)
{
    write-verbose "Good, the zip file was already unzipped"
}
else
{
    # unzip the scanner file
    $zipfile = $ScriptPath + "\scanner.zip"
    $outpath = $ScriptPath 
    [System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
}

$DClist = @()
$DomainListNames = @()
$DomainList = Get-NetForestDomain
$DCchecked = 0
$resultData = @()

foreach ($DomainName in $DomainList)
{
    write-verbose "The Domain name:`n$DomainName"
    $DomainListNames += $DomainName
    $DClistTemp = Get-NetDomainController -Domain $DomainName
    
    $DClist += $DClistTemp
    foreach ($DCtoCheck in $DClistTemp)
    {
        write-verbose "The DC name:`n$DCtoCheck"
        $DomainRoot = [ADSI]"LDAP://$DCtoCheck/RootDSE"
        $domainFunctionalLevel = $DomainRoot.domainfunctionality
        write-verbose "The Domain Funciotnal Level:`n$domainFunctionalLevel"
        if (! $domainFunctionalLevel)
        {
	        write-verbose "Error with Domain Functional Level of: $DCtoCheck"
	        continue
        }
        if ($domainFunctionalLevel -lt 3 )
        {
	        write-output "Domain Functional Level of `"$DCtoCheckDomain`" need be at least 2008 to test for Skeleton Key."
	        continue
        }
        $domainFunctionalLevelName = ""
        if ($domainFunctionalLevel -eq 3) {$domainFunctionalLevelName = "Level3=2008"}
        if ($domainFunctionalLevel -eq 4) {$domainFunctionalLevelName = "Level4=2008R2"}
        if ($domainFunctionalLevel -eq 5) {$domainFunctionalLevelName = "Level5=2012"}
        if ($domainFunctionalLevel -eq 6) {$domainFunctionalLevelName = "Level6=2012R2"}
        if ($domainFunctionalLevel -eq 7) {$domainFunctionalLevelName = "Level7=2016"}

        $AccountSearcher = Get-DomainSearcher -Domain $DomainName -DomainController $DCtoCheck
        $AccountSearcher.filter = "(&(objectClass=Computer)(msds-supportedencryptiontypes>=8)(useraccountcontrol=4096))"
        $AccountToUse = $AccountSearcher.FindOne()
        $accountName = $AccountToUse.properties["name"]

        write-verbose "The account to use: $accountName"
        try{
            $exeScanner = $ScriptPath + "\scanner\SkeletonCheck.exe"
            $AllArgs = @("--DCname", $DCtoCheck, "--DomainName", $DomainName, "--AccountName", $accountName)
            $result = & $exeScanner $AllArgs 2> $Null
        }
        catch{
            Write-Output "Error while scanning $DCtoCheck"
            continue
        }
        if ($result -like "Suspected with SkeletonKey malware" )
        {
            $DCchecked = $DCchecked + 1
            $details = [ordered]@{
                DomainName = $DomainName
                DomainFunctionalLevel = [string]$domainFunctionalLevelName
                DCname = $DCtoCheck
                DCinfected = 'Yes'
                }
                Write-Output "`"$DCtoCheck`" is suspected"
                $resultData += New-Object PSObject -Property $details
	        continue
        }
        if ($result -like "Clear" )
        {
	        $DCchecked = $DCchecked + 1	

            $details = [ordered]@{
                DomainName = $DomainName
                DomainFunctionalLevel = [string]$domainFunctionalLevelName
                DCname = $DCtoCheck
                DCinfected = 'No'
                }
                Write-Output "`"$DCtoCheck`" is clear"
                $resultData += New-Object PSObject -Property $details
        }
        else
        {
	        write-output "Error while scanning DC $DCtoCheck"
        }
    }
}

try {
    [string]$removeScanner = $ScriptPath + "\scanner\*"
    [string]$removeScannerFolder = $ScriptPath + "\scanner"
    Remove-Item $removeScanner -force
    Remove-Item $removeScannerFolder -force
}
catch {write-verbose "Error while trying to delete the scanner file"}


$output = "`nChecked " + $DCchecked + " domain controllers out of " + $DClist.Length + " identified domain conrtollers"
write-host $output -ForegroundColor Yellow

$resultData | sort DomainName | export-csv -Path "$ScriptPath\Results\SkeletonKeyResults.csv" -NoTypeInformation
$output = "SkeletonKey scan completed - the results are in  the folder: `"\Results\SkeletonKey`" under the Zbang directory"
Write-Host $output -ForegroundColor Yellow