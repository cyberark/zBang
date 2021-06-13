<#----------------------------------------------------------------------------------------------------
Release Notes:

The SID History module queries the Active Directory and searches for
accounts that have SID history attribute.
 
Version 1: 14.6.17
Last Update: 13/06/2021

Based on riskySPN script:
https://github.com/CyberArkLabs/RiskySPN

----------------------------------------------------------------------------------------------------#>


function Get-UsersWithSIDHistory
{
    <#
    .SYNOPSIS
        Reveals users that have SID History and gives important information about them.

    .DESCRIPTION
        This function queries the Active Directory and searches for accounts
        that have SID history attribute. 
        If you didn't do "migration" in your domain - this attribute 
        shouldn't be there and it might be as a result of an attack.
        With compromised SID history attribute - an attacker can impersonate
        to an Entrepise/Domain admin.
        Requires Active Directory authentication (domain user is enough). 

    .PARAMETER Domain
        The name of the domain to query. "Current" for the user's current domain. Defualts to the entire forest. 

    .PARAMETER AddGroups
        Add additional groups to consider as sensitive 

    .PARAMETER FullData
        Show more user data

    .EXAMPLE 
        Get-UsersWithSIDHistory -Domain "IT.company.com"
        Returns all user accounts witj SID History attribute in the IT.company.com domain.
        
    .EXAMPLE
        Get-UsersWithSIDHistory -FullData -Verbose
        Returns detailed information about all user accounts with SID History attribute in the forest. Enable verbose mode 
    #>

    [CmdletBinding()]
    param
    (
        [string]$Domain,
        [array]$AddGroups,
        [switch]$Sensitive,
        [switch]$Stealth,
        [switch]$GetSPNs,
        [switch]$FullData
    )

    #recursivly get nested groups of a group object
    function Get-NestedGroups
    {
        [CmdletBinding()]
        param
        (
            [parameter(Mandatory=$True, ValueFromPipeline=$True)]
            [ValidateNotNullOrEmpty()]
            [string]$DN
        )

        $GroubObj = [adsi]"LDAP://$DN"
        #if the object is a group
        if ($GroubObj.Properties.samaccounttype -match '536870912' -or $GroubObj.Properties.samaccounttype -match '268435456')
        {
            Write-Verbose "Searching for nested groups inside group: $($GroubObj.Properties.samaccountname)"
            foreach ($Member in $GroubObj.Properties.member)
            {
                #get group objects inside this group object
                Get-NestedGroups -DN $Member
            }
            return $GroubObj.Properties.distinguishedname
        }
    }

#========================================================================= Creating ADSI Searcher =========================================================================
    
    $SearchList = @()
    if($Domain)
    {
        if ($Domain -eq "Current")
        {
            $SearchScope = [System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
        }
        else
        {
            try
            {
                $TargetDomain = New-Object System.DirectoryServices.ActiveDirectory.DirectoryContext('Domain', $Domain)
                $SearchScope = [System.DirectoryServices.ActiveDirectory.Domain]::GetDomain($TargetDomain)
            }
            catch 
            {
                Write-Error "Could not communicate with the foreigen domain: $Domain"
                return
            } 
        }
        if ($SearchScope.DomainMode.value__ -lt 4)
        {
            Write-host "The function level of domain: $($ChildDomain.name) is lower than 2008R2 - it may cause partial results"
        }
        $SearchList += 'LDAP://DC=' + ($SearchScope.Name -Replace ("\.",',DC='))
        Write-Host "Searching the domain: $($SearchScope.name)"
    }
    else 
    {
        try{
        $SearchScope = [System.DirectoryServices.ActiveDirectory.Forest]::GetCurrentForest()
        }catch{
            write-host "The current forest cannot be reached. Seems like the machine is not part of any domain." -ForegroundColor Red
            exit
        }
        foreach ($ChildDomain in $($SearchScope.Domains))
        {
            if ($ChildDomain.DomainMode.value__ -lt 4)
            {
                Write-host "The function level of domain: $($ChildDomain.Name) is lower than 2008R2 - it may cause partial results"
            }
            $SearchList += 'LDAP://DC=' + ($ChildDomain.Name -Replace ("\.",',DC='))
        }
        Write-Host "Searching the forest: $($SearchScope.name)"    
    }

    #creating ADSI searcher 
    $Searcher = New-Object System.DirectoryServices.DirectorySearcher
    $Searcher.PageSize = 500
    $Searcher.PropertiesToLoad.Add("distinguishedname") | Out-Null

#========================================================================= Gathering Sensitive Groups =========================================================================

    #list of built-in sensitive groups (Administratos group conatins domain and enterprise admins) - did I missed a group? -> maybe "DS Restore Mode Administrator"
    $SensitiveGroups = @("Administrators", "Account Operators", "Backup Operators", "Print Operators", "Server Operators", "Group Policy Creator Owners", "Schema Admins")
    if ($AddGroups)
    {
        Write-Verbose "Adding $AddGroups to the list of senstivie groups"
        $SensitiveGroups += $AddGroups
    }
    $AllSensitiveGroups = @()
    Write-Verbose "Gathering sensitive groups"
    foreach ($Path in $SearchList)
    {
        $Searcher.SearchRoot = $Path
        foreach ($GroupName in $SensitiveGroups)
        {
            #filter group objects with specific name
            $Searcher.Filter = "(&(|(samAccountType=536870912)(samAccountType=268435456))(|(samAccountName=$GroupName)(name=$GroupName)))"
            try {$GroupObjects = $Searcher.FindAll()}
            catch {Write-Warning "Could not communicate with the domain: $($Path -replace "LDAP://DC=" -replace ",DC=", ".") - Does it have trust?"}
            #if we find groups
            if ($GroupObjects)
            {
                foreach ($GroupObject in $GroupObjects)
                {
                    #recursivly get all nested groups inherited from sensitive groups - don't trust AdminCount=1 
                    $AllSensitiveGroups += Get-NestedGroups -DN $GroupObject.Properties.distinguishedname
                }
            }
            else {Write-Warning "Could not find group: $Group"}     
        }
    }
    Write-Verbose "Number of sensitive groups found: $($AllSensitiveGroups.Count)"

#========================================================================= Gathering users with SID History attribute =========================================================================
  
    Write-Host "Gathering user accounts with SID History attribute"
    #list of properties to retreive from AD
    $Properies = "samaccountname","displayname", "SID", "SIDHistory", "userprincipalname", "memberof","pwdlastset","objectCategory","ObjectClass"

    foreach ($Property in $Properies)
    {
        $Searcher.PropertiesToLoad.Add($Property) | Out-Null
    }
    
    #filter user accounts with SID History
    $Searcher.Filter = "(&(objectCategory=User)(SIDHistory=*))"

    $UsersWithSIDHistory = @()
    foreach ($Path in $SearchList)
    {
        $Searcher.SearchRoot = $Path
        #Write-Host $Path
        #printing the user results
        #foreach ($objResult in $Searcher.FindAll())
        #    {$objItem = $objResult.Properties; $objItem.displayname}
        
        try {$UsersWithSIDHistory += $Searcher.FindAll()}
        catch {Write-Warning "Could not communicate with the domain: $($Path -replace "LDAP://DC=" -replace ",DC=", ".") - Does it have trust?"}      
    } 
          
    if ($($UsersWithSIDHistory.Count -eq 0))
    {
        Write-host "`nSID History scan completed`nThe scanned forest don't have user accounts with SID History" -ForegroundColor Yellow
    }
    else {
        Write-host "`nSID History scan completed`nFound users with SID History - Number of users: $($UsersWithSIDHistory.Count)" -ForegroundColor Yellow
        Write-host "Please check the results file in `"\Results\SIDHistory`" folder" -ForegroundColor Yellow
    }

    
# ========================================================================= Gathering info about users =========================================================================

    $CurrentDate = Get-Date
    $AllData = @()
    foreach ($User in $UsersWithSIDHistory)
    {
        #write-host ""
        Write-Verbose "Gathering info about the user: $($User.Properties.displayname)"
        
        [int32]$UAC = [string]$User.Properties.useraccountcontrol
        #reading UAC attributes using bitmask - https://support.microsoft.com/en-us/kb/305144
        $IsEnabled = $true
        #if the user is disabled or lockedout
        if (($UAC -band 2) -eq 2 -or ($UAC -band 16) -eq 16) {$IsEnabled = $false} # 0x0002 / 0x0010
        
        if ($User.Properties.memberof)
        {
            #get sensitive groups that the user is a memberof
            $UserSensitiveGroups = (@(Compare-Object $AllSensitiveGroups $([array]$User.Properties.memberof) -IncludeEqual -ExcludeDifferent)).InputObject
        }
        $IsSensitive = 'False'
        #if the user is a member of a sensitive group or is allowed for Kerberos unconstrained delegation
        if ($UserSensitiveGroups -or $Unconstrained)
        {
            Write-Verbose "$($User.Properties.displayname) is sensitive"
            $IsSensitive = $true
        }

# ========================================================================= Gathering info about user's SID History =========================================================================
        
        #Write-host "Found user with SIDHistory: $($User.Properties.displayname)"
        #Write-Host "main SID:"
	    $userprincipalname = [string]$User.Properties.userprincipalname
        $objUser = New-Object System.Security.Principal.NTAccount($userprincipalname)
        $strSID = $objUser.Translate([System.Security.Principal.SecurityIdentifier])
        #write-host $strSID.Value
		
        #write-host "SidHistory:"
        $objItemT = $User.Properties
		$tsam = $objItemT.samaccountname
		$objpath = $User.path
		$objpath1=[ADSI]"$objpath"
		$objectSIDHistory = [byte[]]$objpath1.sidhistory.value
		$sidHistory = new-object System.Security.Principal.SecurityIdentifier $objectSIDHistory,0 
		#write-host $sidHistory 

# ========================================================================= Gathering info on the SID History =========================================================================

        #search for the SID History object in the forest
        $Searcher.Filter = "(objectSID=$sidHistory)"
       
        $infoFromHistory = @()
        foreach ($Path in $SearchList)
        {
            $Searcher.SearchRoot = $Path
            #Write-Host $Path
            #printing the user results
            #foreach ($objResult in $Searcher.FindAll())
            #    {$objItem = $objResult.Properties; $objItem.displayname}
        
            try {$infoFromHistory += $Searcher.FindAll()}
            catch {Write-Warning "Could not communicate with the domain: $($Path -replace "LDAP://DC=" -replace ",DC=", ".") - Does it have trust?"}      
        }

        $historyNotFound = 'previousSIDnotFound'
        if ($infoFromHistory.Count -eq 0)
        {
            $HistoryName = $historyNotFound
            $HistoryMemberOf = $historyNotFound
        }
        else {
            #write-host "SidHistory Name:"
            $objSID = New-Object System.Security.Principal.SecurityIdentifier ($sidHistory)
            $objUser = $objSID.Translate( [System.Security.Principal.NTAccount])
            $HistoryName = $objUser.Value
            #write-host $HistoryName
            #$HistoryName = $HistoryName -replace ".*\\"
            #write-host $userHistoryName
        }
        $secondaryName = ""
        $secondaryMemberOf = ""
        $isSecondaryPriv = $false
        
        $secondaryName = Convert-SidToName $sidHistory
        $secondaryDomainName = ((Convert-ADName -ObjectName $secondaryName) -split "/")[0]

        foreach ($history in $infoFromHistory)
        {
            $secondaryDomainSID = [string]$history.Properties.userprincipalname -replace ".*@"            
            #write-host "SID History member of:"
            #$MemberOf = $history.Properties.memberof -replace "CN=" -replace ",.*"
            $HistoryMemberOf = $history.Properties.memberof -replace "CN=" -replace ",.*"
            $HistoryMemberOf | foreach {
                if ($SensitiveGroups -contains $_){
                    $isSecondaryPriv = $true
                    }
                }
            $ofs = '<|>'
            $secondaryMemberOf = [string]($HistoryMemberOf | foreach {[string]$_})  
            #write-host $HistoryMemberOf
        }



        #$secondaryDomainSID = Convert-SidToName $sid

        write-host ""
        $ofs = '<|>'
        $initiallyMemberOf = [string](($User.Properties.memberof -replace "CN=" -replace ",.*") | foreach {[string]$_}) 
         
        #NS 02-01-2018
        $userphoto = ""
        $bytes = GetMemberThumbnail([string]$User.Properties.samaccountname)
        if ($bytes -ne $null) {
            $userphoto = [Convert]::ToBase64String($bytes)
        }
        else {
            #$bytes = GetMemberThumbnail("asafh")
            #$userphoto = [Convert]::ToBase64String($bytes)
        }





# ========================================================================= Building the final Data Structure =========================================================================

        $UserData = [PSCustomObject][ordered] @{
            UserName            = [string]$User.Properties.samaccountname
            DomainName          = [string]$User.Properties.userprincipalname -replace ".*@"
            DisplayName         = [string]$User.Properties.displayname
            mainSID             = [string]$strSID.Value
            SIDHistory          = [string]$sidHistory           
            initiallySensitive  = $IsSensitive
            Description         = [string]$User.Properties.description
            IsEnabled           = $IsEnabled
            SensitiveGroups     = $UserSensitiveGroups -replace "CN=" -replace ",.*"
            initiallyMemberOf   = $initiallyMemberOf
            SIDHistoryName      = [string]$HistoryName 
            SIDHistoryMemberOf  = $HistoryMemberOf 
            secondaryDomainSID  = $secondaryName
            secondaryDomainName = $secondaryDomainName
            secondaryMemberOf   = $secondaryMemberOf
            isSecondaryPriv     = $isSecondaryPriv
            UserPhoto           = $userphoto
        } 
        $AllData += $UserData 
    }

    Write-Verbose "Number of users included in the list: $($AllData.UserName.Count)"

    #For now the FullData paramter is not relevant
    if ($FullData) {return $AllData}    
    else {return $AllData}   
}



#################################################
## NS 01/01/2018
#################################################
function GetMemberThumbnail($userName)
{
    $searcher = new-object System.DirectoryServices.DirectorySearcher("")
    $searcher.filter = "(&(objectCategory=person)(sAMAccountName=$userName))"
    #$searcher.SearchScope = "subtree"
    #$searcher.PropertiesToLoad.Add("memberOf")

    $result = $searcher.findOne()

    if ($result -ne $null)
    {
        return $result.Properties["thumbnailPhoto"]
    }
}




function Report-UsersWithSIDHistory
{
    <#
    .SYNOPSIS
        Report important information about users that have SID History attribute

    .DESCRIPTION
        This function queries the Active Directory and searches for accounts
        that have SID history attribute. 
        If you didn't do "migration" in your domain - this attribute 
        shouldn't be there and it might be as a result of an attack.
        With compromised SID history attribute - an attacker can impersonate
        to an Entrepise/Domain admin.

    .PARAMETER Type
        The format of the report file. The default is CSV 

    .PARAMETER Path
        The path to store the file. The default is the user's "Documents" folder

    .PARAMETER Name
        The name of the report. The default is "Report" 

    .PARAMETER Summary
        Report minimial information

    .PARAMETER DoNotOpen
        Do not open the report

    .EXAMPLE 
        Report-UsersWithSIDHistory
        Report all user accounts that have SID History attribute. Save and open the report in CSV format in Documents folder 
        
    .EXAMPLE
        Report-UsersWithSIDHistory -Type XML -Path C:\Report -DoNotOpen
        Report all user accounts that have SID History attribute in entire forest. Save the report in XML format in C:\Report folder  
    #>

    [CmdletBinding()]
    param
    (
        [String]$Domain ,
        [ValidateSet("CSV", "XML", "HTML", "TXT")]
        [String]$Type = "CSV",
       # [String]$Path = "$env:USERPROFILE\Documents",
	[String]$Path = "Results/",
        [String]$Name = "Report",
        [Switch]$Summary,
        [Switch]$DoNotOpen
    )

    # Credits for Boe Prox from TechNet - https://gallery.technet.microsoft.com/scriptcenter/Convert-OutoutForCSV-6e552fc6
    Function Convert-Output
    {
        [cmdletbinding()]
        Param (
            [parameter(ValueFromPipeline=$true)]
            [psobject]$InputObject
        )
        Begin {
            $PSBoundParameters.GetEnumerator() | ForEach {
                Write-Verbose "$($_)"
            }
            $FirstRun = $True
        }
        Process {
            If ($FirstRun) {
                $OutputOrder = $InputObject.psobject.properties.name
                $FirstRun = $False
                #Get properties to process
                $Properties = Get-Member -InputObject $InputObject -MemberType *Property
                #Get properties that hold a collection
                $Properties_Collection = @(($Properties | Where-Object {
                    $_.Definition -match "Collection|\[\]"
                }).Name)
                #Get properties that do not hold a collection
                $Properties_NoCollection = @(($Properties | Where-Object {
                    $_.Definition -notmatch "Collection|\[\]"
                }).Name)
            }
            $InputObject | ForEach {
                $Line = $_
                $stringBuilder = New-Object Text.StringBuilder
                $Null = $stringBuilder.AppendLine("[pscustomobject] @{")
                $OutputOrder | ForEach {
                        <#if ($_ -eq "SIDHistoryMemberOf")
                        {
                            $Null = $stringBuilder.Append("`"$($_)`" = `"$((($line.$($_) | Out-String).Trim()) -replace "\n",",")`"")
                        }
                        else {
                            $Null = $stringBuilder.AppendLine("`"$($_)`" = `"$(($line.$($_) | Out-String).Trim())`"") 
                        }#>
                        $Null = $stringBuilder.AppendLine("`"$($_)`" = `"$((($line.$($_) | Out-String).Trim()) -replace "\n",",")`"")
                    }
                }
                $Null = $stringBuilder.AppendLine("}")
                Invoke-Expression $stringBuilder.ToString()
            }
        End {}
    }


    $FilePath = "$Path\$Name.$($Type.ToLower())"
    $FilePathCSV = "$Path\$Name" +".csv"
	if($Domain)
    {
		$Report = Get-UsersWithSIDHistory -FullData -Domain $Domain
	}else{
		$Report = Get-UsersWithSIDHistory -FullData
	}

    if ($Summary)
    {
        #---------------Not relevant for now-------------------------------------------------------------------------#
        #$Report = $Report | Select-Object UserName,DomainName,IsSensitive,PwdAge,CrackWindow,RunsUnder
    }
# NS    if ($Type -eq "CSV" ) {$Report | Convert-Output | Export-Csv $FilePath -Encoding UTF8 -NoTypeInformation}
    if ($Type -eq "CSV" ) {$Report | Export-Csv $FilePath -Encoding UTF8 -NoTypeInformation}
    elseif ($Type -eq "XML") 
    {
        $Report | Export-Clixml $FilePath -Encoding UTF8
        $Report | Convert-Output | Export-Csv $FilePathCSV -Encoding UTF8 -NoTypeInformation
    }
    elseif ($Type -eq "HTML") {$Report |  Convert-Output | ConvertTo-Html | Out-File $FilePath -Encoding utf8}
    elseif ($Type -eq "TXT") {$Report |  Convert-Output | Out-File $FilePath -Encoding utf8}  
    #Write-Host "$Type file saved in: $FilePath"

    if (!$DoNotOpen)
    {
#        Invoke-Item $FilePath
    }    
}


#############
# The following functions are from the open source project PowerView by Will Schroeder (@harmj0y):
# https://raw.githubusercontent.com/PowerShellMafia/PowerSploit/master/Recon/PowerView.ps1
#############


filter Convert-ADName {
<#
    .SYNOPSIS

        Converts user/group names from NT4 (DOMAIN\user) or domainSimple (user@domain.com)
        to canonical format (domain.com/Users/user) or NT4.

        Based on Bill Stewart's code from this article: 
            http://windowsitpro.com/active-directory/translating-active-directory-object-names-between-formats

    .PARAMETER ObjectName

        The user/group name to convert.

    .PARAMETER InputType

        The InputType of the user/group name ("NT4","Simple","Canonical").

    .PARAMETER OutputType

        The OutputType of the user/group name ("NT4","Simple","Canonical").

    .EXAMPLE

        PS C:\> Convert-ADName -ObjectName "dev\dfm"
        
        Returns "dev.testlab.local/Users/Dave"

    .EXAMPLE

        PS C:\> Convert-SidToName "S-..." | Convert-ADName
        
        Returns the canonical name for the resolved SID.

    .LINK

        http://windowsitpro.com/active-directory/translating-active-directory-object-names-between-formats
#>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$True, ValueFromPipeline=$True)]
        [String]
        $ObjectName,

        [String]
        [ValidateSet("NT4","Simple","Canonical")]
        $InputType,

        [String]
        [ValidateSet("NT4","Simple","Canonical")]
        $OutputType
    )

    $NameTypes = @{
        'Canonical' = 2
        'NT4'       = 3
        'Simple'    = 5
    }

    if(-not $PSBoundParameters['InputType']) {
        if( ($ObjectName.split('/')).Count -eq 2 ) {
            $ObjectName = $ObjectName.replace('/', '\')
        }

        if($ObjectName -match "^[A-Za-z]+\\[A-Za-z ]+") {
            $InputType = 'NT4'
        }
        elseif($ObjectName -match "^[A-Za-z ]+@[A-Za-z\.]+") {
            $InputType = 'Simple'
        }
        elseif($ObjectName -match "^[A-Za-z\.]+/[A-Za-z]+/[A-Za-z/ ]+") {
            $InputType = 'Canonical'
        }
        else {
            Write-Warning "Can not identify InType for $ObjectName"
            return $ObjectName
        }
    }
    elseif($InputType -eq 'NT4') {
        $ObjectName = $ObjectName.replace('/', '\')
    }

    if(-not $PSBoundParameters['OutputType']) {
        $OutputType = Switch($InputType) {
            'NT4' {'Canonical'}
            'Simple' {'NT4'}
            'Canonical' {'NT4'}
        }
    }

    # try to extract the domain from the given format
    $Domain = Switch($InputType) {
        'NT4' { $ObjectName.split("\")[0] }
        'Simple' { $ObjectName.split("@")[1] }
        'Canonical' { $ObjectName.split("/")[0] }
    }

    # Accessor functions to simplify calls to NameTranslate
    function Invoke-Method([__ComObject] $Object, [String] $Method, $Parameters) {
        $Output = $Object.GetType().InvokeMember($Method, "InvokeMethod", $Null, $Object, $Parameters)
        if ( $Output ) { $Output }
    }
    function Set-Property([__ComObject] $Object, [String] $Property, $Parameters) {
        [Void] $Object.GetType().InvokeMember($Property, "SetProperty", $Null, $Object, $Parameters)
    }

    $Translate = New-Object -ComObject NameTranslate

    try {
        Invoke-Method $Translate "Init" (1, $Domain)
    }
    catch [System.Management.Automation.MethodInvocationException] { 
        Write-Verbose "Error with translate init in Convert-ADName: $_"
    }

    Set-Property $Translate "ChaseReferral" (0x60)

    try {
        Invoke-Method $Translate "Set" ($NameTypes[$InputType], $ObjectName)
        (Invoke-Method $Translate "Get" ($NameTypes[$OutputType]))
    }
    catch [System.Management.Automation.MethodInvocationException] {
        Write-Verbose "Error with translate Set/Get in Convert-ADName: $_"
    }
}


filter Convert-SidToName {
<#
    .SYNOPSIS
    
        Converts a security identifier (SID) to a group/user name.

    .PARAMETER SID
    
        The SID to convert.

    .EXAMPLE

        PS C:\> Convert-SidToName S-1-5-21-2620891829-2411261497-1773853088-1105
#>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$True, ValueFromPipeline=$True)]
        [String]
        [ValidatePattern('^S-1-.*')]
        $SID
    )

    try {
        $SID2 = $SID.trim('*')

        # try to resolve any built-in SIDs first
        #   from https://support.microsoft.com/en-us/kb/243330
        Switch ($SID2) {
            'S-1-0'         { 'Null Authority' }
            'S-1-0-0'       { 'Nobody' }
            'S-1-1'         { 'World Authority' }
            'S-1-1-0'       { 'Everyone' }
            'S-1-2'         { 'Local Authority' }
            'S-1-2-0'       { 'Local' }
            'S-1-2-1'       { 'Console Logon ' }
            'S-1-3'         { 'Creator Authority' }
            'S-1-3-0'       { 'Creator Owner' }
            'S-1-3-1'       { 'Creator Group' }
            'S-1-3-2'       { 'Creator Owner Server' }
            'S-1-3-3'       { 'Creator Group Server' }
            'S-1-3-4'       { 'Owner Rights' }
            'S-1-4'         { 'Non-unique Authority' }
            'S-1-5'         { 'NT Authority' }
            'S-1-5-1'       { 'Dialup' }
            'S-1-5-2'       { 'Network' }
            'S-1-5-3'       { 'Batch' }
            'S-1-5-4'       { 'Interactive' }
            'S-1-5-6'       { 'Service' }
            'S-1-5-7'       { 'Anonymous' }
            'S-1-5-8'       { 'Proxy' }
            'S-1-5-9'       { 'Enterprise Domain Controllers' }
            'S-1-5-10'      { 'Principal Self' }
            'S-1-5-11'      { 'Authenticated Users' }
            'S-1-5-12'      { 'Restricted Code' }
            'S-1-5-13'      { 'Terminal Server Users' }
            'S-1-5-14'      { 'Remote Interactive Logon' }
            'S-1-5-15'      { 'This Organization ' }
            'S-1-5-17'      { 'This Organization ' }
            'S-1-5-18'      { 'Local System' }
            'S-1-5-19'      { 'NT Authority' }
            'S-1-5-20'      { 'NT Authority' }
            'S-1-5-80-0'    { 'All Services ' }
            'S-1-5-32-544'  { 'BUILTIN\Administrators' }
            'S-1-5-32-545'  { 'BUILTIN\Users' }
            'S-1-5-32-546'  { 'BUILTIN\Guests' }
            'S-1-5-32-547'  { 'BUILTIN\Power Users' }
            'S-1-5-32-548'  { 'BUILTIN\Account Operators' }
            'S-1-5-32-549'  { 'BUILTIN\Server Operators' }
            'S-1-5-32-550'  { 'BUILTIN\Print Operators' }
            'S-1-5-32-551'  { 'BUILTIN\Backup Operators' }
            'S-1-5-32-552'  { 'BUILTIN\Replicators' }
            'S-1-5-32-554'  { 'BUILTIN\Pre-Windows 2000 Compatible Access' }
            'S-1-5-32-555'  { 'BUILTIN\Remote Desktop Users' }
            'S-1-5-32-556'  { 'BUILTIN\Network Configuration Operators' }
            'S-1-5-32-557'  { 'BUILTIN\Incoming Forest Trust Builders' }
            'S-1-5-32-558'  { 'BUILTIN\Performance Monitor Users' }
            'S-1-5-32-559'  { 'BUILTIN\Performance Log Users' }
            'S-1-5-32-560'  { 'BUILTIN\Windows Authorization Access Group' }
            'S-1-5-32-561'  { 'BUILTIN\Terminal Server License Servers' }
            'S-1-5-32-562'  { 'BUILTIN\Distributed COM Users' }
            'S-1-5-32-569'  { 'BUILTIN\Cryptographic Operators' }
            'S-1-5-32-573'  { 'BUILTIN\Event Log Readers' }
            'S-1-5-32-574'  { 'BUILTIN\Certificate Service DCOM Access' }
            'S-1-5-32-575'  { 'BUILTIN\RDS Remote Access Servers' }
            'S-1-5-32-576'  { 'BUILTIN\RDS Endpoint Servers' }
            'S-1-5-32-577'  { 'BUILTIN\RDS Management Servers' }
            'S-1-5-32-578'  { 'BUILTIN\Hyper-V Administrators' }
            'S-1-5-32-579'  { 'BUILTIN\Access Control Assistance Operators' }
            'S-1-5-32-580'  { 'BUILTIN\Access Control Assistance Operators' }
            Default { 
                $Obj = (New-Object System.Security.Principal.SecurityIdentifier($SID2))
                $Obj.Translate( [System.Security.Principal.NTAccount]).Value
            }
        }
    }
    catch {
        Write-Verbose "Invalid SID: $SID"
        $SID
    }
}


filter Get-NetDomain {
<#
    .SYNOPSIS

        Returns a given domain object.

    .PARAMETER Domain

        The domain name to query for, defaults to the current domain.

    .PARAMETER Credential

        A [Management.Automation.PSCredential] object of alternate credentials
        for connection to the target domain.

    .EXAMPLE

        PS C:\> Get-NetDomain -Domain testlab.local

    .EXAMPLE

        PS C:\> "testlab.local" | Get-NetDomain

    .LINK

        http://social.technet.microsoft.com/Forums/scriptcenter/en-US/0c5b3f83-e528-4d49-92a4-dee31f4b481c/finding-the-dn-of-the-the-domain-without-admodule-in-powershell?forum=ITCG
#>

    param(
        [Parameter(ValueFromPipeline=$True)]
        [String]
        $Domain,

        [Management.Automation.PSCredential]
        $Credential
    )

    if($Credential) {
        
        Write-Verbose "Using alternate credentials for Get-NetDomain"

        if(!$Domain) {
            # if no domain is supplied, extract the logon domain from the PSCredential passed
            $Domain = $Credential.GetNetworkCredential().Domain
            Write-Verbose "Extracted domain '$Domain' from -Credential"
        }
   
        $DomainContext = New-Object System.DirectoryServices.ActiveDirectory.DirectoryContext('Domain', $Domain, $Credential.UserName, $Credential.GetNetworkCredential().Password)
        
        try {
            [System.DirectoryServices.ActiveDirectory.Domain]::GetDomain($DomainContext)
        }
        catch {
            Write-Verbose "The specified domain does '$Domain' not exist, could not be contacted, there isn't an existing trust, or the specified credentials are invalid."
            $Null
        }
    }
    elseif($Domain) {
        $DomainContext = New-Object System.DirectoryServices.ActiveDirectory.DirectoryContext('Domain', $Domain)
        try {
            [System.DirectoryServices.ActiveDirectory.Domain]::GetDomain($DomainContext)
        }
        catch {
            Write-Verbose "The specified domain '$Domain' does not exist, could not be contacted, or there isn't an existing trust."
            $Null
        }
    }
    else {
        [System.DirectoryServices.ActiveDirectory.Domain]::GetCurrentDomain()
    }
}


filter Convert-ADName {
<#
    .SYNOPSIS

        Converts user/group names from NT4 (DOMAIN\user) or domainSimple (user@domain.com)
        to canonical format (domain.com/Users/user) or NT4.

        Based on Bill Stewart's code from this article: 
            http://windowsitpro.com/active-directory/translating-active-directory-object-names-between-formats

    .PARAMETER ObjectName

        The user/group name to convert.

    .PARAMETER InputType

        The InputType of the user/group name ("NT4","Simple","Canonical").

    .PARAMETER OutputType

        The OutputType of the user/group name ("NT4","Simple","Canonical").

    .EXAMPLE

        PS C:\> Convert-ADName -ObjectName "dev\dfm"
        
        Returns "dev.testlab.local/Users/Dave"

    .EXAMPLE

        PS C:\> Convert-SidToName "S-..." | Convert-ADName
        
        Returns the canonical name for the resolved SID.

    .LINK

        http://windowsitpro.com/active-directory/translating-active-directory-object-names-between-formats
#>
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$True, ValueFromPipeline=$True)]
        [String]
        $ObjectName,

        [String]
        [ValidateSet("NT4","Simple","Canonical")]
        $InputType,

        [String]
        [ValidateSet("NT4","Simple","Canonical")]
        $OutputType
    )

    $NameTypes = @{
        'Canonical' = 2
        'NT4'       = 3
        'Simple'    = 5
    }

    if(-not $PSBoundParameters['InputType']) {
        if( ($ObjectName.split('/')).Count -eq 2 ) {
            $ObjectName = $ObjectName.replace('/', '\')
        }

        if($ObjectName -match "^[A-Za-z]+\\[A-Za-z ]+") {
            $InputType = 'NT4'
        }
        elseif($ObjectName -match "^[A-Za-z ]+@[A-Za-z\.]+") {
            $InputType = 'Simple'
        }
        elseif($ObjectName -match "^[A-Za-z\.]+/[A-Za-z]+/[A-Za-z/ ]+") {
            $InputType = 'Canonical'
        }
        else {
            Write-Warning "Can not identify InType for $ObjectName"
            return $ObjectName
        }
    }
    elseif($InputType -eq 'NT4') {
        $ObjectName = $ObjectName.replace('/', '\')
    }

    if(-not $PSBoundParameters['OutputType']) {
        $OutputType = Switch($InputType) {
            'NT4' {'Canonical'}
            'Simple' {'NT4'}
            'Canonical' {'NT4'}
        }
    }

    # try to extract the domain from the given format
    $Domain = Switch($InputType) {
        'NT4' { $ObjectName.split("\")[0] }
        'Simple' { $ObjectName.split("@")[1] }
        'Canonical' { $ObjectName.split("/")[0] }
    }

    # Accessor functions to simplify calls to NameTranslate
    function Invoke-Method([__ComObject] $Object, [String] $Method, $Parameters) {
        $Output = $Object.GetType().InvokeMember($Method, "InvokeMethod", $Null, $Object, $Parameters)
        if ( $Output ) { $Output }
    }
    function Set-Property([__ComObject] $Object, [String] $Property, $Parameters) {
        [Void] $Object.GetType().InvokeMember($Property, "SetProperty", $Null, $Object, $Parameters)
    }

    $Translate = New-Object -ComObject NameTranslate

    try {
        Invoke-Method $Translate "Init" (1, $Domain)
    }
    catch [System.Management.Automation.MethodInvocationException] { 
        Write-Verbose "Error with translate init in Convert-ADName: $_"
    }

    Set-Property $Translate "ChaseReferral" (0x60)

    try {
        Invoke-Method $Translate "Set" ($NameTypes[$InputType], $ObjectName)
        (Invoke-Method $Translate "Get" ($NameTypes[$OutputType]))
    }
    catch [System.Management.Automation.MethodInvocationException] {
        Write-Verbose "Error with translate Set/Get in Convert-ADName: $_"
    }
}

filter Get-DomainSearcher {
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

    .PARAMETER Credential

        A [Management.Automation.PSCredential] object of alternate credentials
        for connection to the target domain.

    .EXAMPLE

        PS C:\> Get-DomainSearcher -Domain testlab.local

    .EXAMPLE

        PS C:\> Get-DomainSearcher -Domain testlab.local -DomainController SECONDARY.dev.testlab.local
#>

    param(
        [Parameter(ValueFromPipeline=$True)]
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
        $PageSize = 200,

        [Management.Automation.PSCredential]
        $Credential
    )

    if(-not $Credential) {
        if(-not $Domain) {
            $Domain = (Get-NetDomain).name
        }
        elseif(-not $DomainController) {
            try {
                # if there's no -DomainController specified, try to pull the primary DC to reflect queries through
                $DomainController = ((Get-NetDomain).PdcRoleOwner).Name
            }
            catch {
                throw "Get-DomainSearcher: Error in retrieving PDC for current domain"
            }
        }
    }
    elseif (-not $DomainController) {
        # if a DC isn't specified
        try {
            $DomainController = ((Get-NetDomain -Credential $Credential).PdcRoleOwner).Name
        }
        catch {
            throw "Get-DomainSearcher: Error in retrieving PDC for current domain"
        }

        if(!$DomainController) {
            throw "Get-DomainSearcher: Error in retrieving PDC for current domain"
        }
    }

    $SearchString = "LDAP://"

    if($DomainController) {
        $SearchString += $DomainController
        if($Domain){
            $SearchString += '/'
        }
    }

    if($ADSprefix) {
        $SearchString += $ADSprefix + ','
    }

    if($ADSpath) {
        if($ADSpath -Match '^GC://') {
            # if we're searching the global catalog
            $DN = $AdsPath.ToUpper().Trim('/')
            $SearchString = ''
        }
        else {
            if($ADSpath -match '^LDAP://') {
                if($ADSpath -match "LDAP://.+/.+") {
                    $SearchString = ''
                }
                else {
                    $ADSpath = $ADSpath.Substring(7)
                }
            }
            $DN = $ADSpath
        }
    }
    else {
        if($Domain -and ($Domain.Trim() -ne "")) {
            $DN = "DC=$($Domain.Replace('.', ',DC='))"
        }
    }

    $SearchString += $DN
    Write-Verbose "Get-DomainSearcher search string: $SearchString"

    if($Credential) {
        Write-Verbose "Using alternate credentials for LDAP connection"
        $DomainObject = New-Object DirectoryServices.DirectoryEntry($SearchString, $Credential.UserName, $Credential.GetNetworkCredential().Password)
        $Searcher = New-Object System.DirectoryServices.DirectorySearcher($DomainObject)
    }
    else {
        $Searcher = New-Object System.DirectoryServices.DirectorySearcher([ADSI]$SearchString)
    }

    $Searcher.PageSize = $PageSize
    $Searcher.CacheResults = $False
    $Searcher
}


function Get-ADObject {
<#
    .SYNOPSIS

        Takes a domain SID and returns the user, group, or computer object
        associated with it.

    .PARAMETER SID

        The SID of the domain object you're querying for.

    .PARAMETER Name

        The Name of the domain object you're querying for.

    .PARAMETER SamAccountName

        The SamAccountName of the domain object you're querying for. 

    .PARAMETER Domain

        The domain to query for objects, defaults to the current domain.

    .PARAMETER DomainController

        Domain controller to reflect LDAP queries through.

    .PARAMETER ADSpath

        The LDAP source to search through, e.g. "LDAP://OU=secret,DC=testlab,DC=local"
        Useful for OU queries.

    .PARAMETER Filter

        Additional LDAP filter string for the query.

    .PARAMETER ReturnRaw

        Switch. Return the raw object instead of translating its properties.
        Used by Set-ADObject to modify object properties.

    .PARAMETER PageSize

        The PageSize to set for the LDAP searcher object.

    .PARAMETER Credential

        A [Management.Automation.PSCredential] object of alternate credentials
        for connection to the target domain.

    .EXAMPLE

        PS C:\> Get-ADObject -SID "S-1-5-21-2620891829-2411261497-1773853088-1110"
        
        Get the domain object associated with the specified SID.
        
    .EXAMPLE

        PS C:\> Get-ADObject -ADSpath "CN=AdminSDHolder,CN=System,DC=testlab,DC=local"
        
        Get the AdminSDHolder object for the testlab.local domain.
#>

    [CmdletBinding()]
    Param (
        [Parameter(ValueFromPipeline=$True)]
        [String]
        $SID,

        [String]
        $Name,

        [String]
        $SamAccountName,

        [String]
        $Domain,

        [String]
        $DomainController,

        [String]
        $ADSpath,

        [String]
        $Filter,

        [Switch]
        $ReturnRaw,

        [ValidateRange(1,10000)] 
        [Int]
        $PageSize = 200,

        [Management.Automation.PSCredential]
        $Credential
    )
    process {
        if($SID) {
            # if a SID is passed, try to resolve it to a reachable domain name for the searcher
            try {
                $Name = Convert-SidToName $SID
                if($Name) {
                    $Canonical = Convert-ADName -ObjectName $Name -InputType NT4 -OutputType Canonical
                    if($Canonical) {
                        $Domain = $Canonical.split("/")[0]
                    }
                    else {
                        Write-Warning "Error resolving SID '$SID'"
                        return $Null
                    }
                }
            }
            catch {
                Write-Warning "Error resolving SID '$SID' : $_"
                return $Null
            }
        }

        $ObjectSearcher = Get-DomainSearcher -Domain $Domain -DomainController $DomainController -Credential $Credential -ADSpath $ADSpath -PageSize $PageSize

        if($ObjectSearcher) {
            if($SID) {
                $ObjectSearcher.filter = "(&(objectsid=$SID)$Filter)"
            }
            elseif($Name) {
                $ObjectSearcher.filter = "(&(name=$Name)$Filter)"
            }
            elseif($SamAccountName) {
                $ObjectSearcher.filter = "(&(samAccountName=$SamAccountName)$Filter)"
            }

            $Results = $ObjectSearcher.FindAll()
            $Results | Where-Object {$_} | ForEach-Object {
                if($ReturnRaw) {
                    $_
                }
                else {
                    # convert/process the LDAP fields for each result
                    Convert-LDAPProperty -Properties $_.Properties
                }
            }
            $Results.dispose()
            $ObjectSearcher.dispose()
        }
    }
}


function Convert-LDAPProperty {
<#
    .SYNOPSIS
    
        Helper that converts specific LDAP property result fields.
        Used by several of the Get-Net* function.

    .PARAMETER Properties

        Properties object to extract out LDAP fields for display.
#>
    param(
        [Parameter(Mandatory=$True, ValueFromPipeline=$True)]
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
            # try to convert misc com objects
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


#Report-UsersWithSIDHistory
