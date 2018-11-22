<#
    Author: Matan Hart
    Contact: cyberark.labs@cyberark.com
    License: GNU v3
    Requirements: PowerShell v3+
#>

function Get-PotentiallyCrackableAccounts
{
    <#
    .SYNOPSIS
        Reveals juicy information about user accounts associated with SPN 

    .DESCRIPTION
        This function queries the Active Directory and retreive information about user accounts associated with SPN.
        This infromation could detremine if a service account is potentially crackable.
        User accounts associated with SPN are vulnerable to offline brute-forceing and they are often (by defualt)
        configured with weak password and encryption (RC4-HMAC).  
        Requires Active Directory authentication (domain user is enough). 

    .PARAMETER Domain
        The name of the domain to query. "Current" for the user's current domain. Defualts to the entire forest. 

    .PARAMETER AddGroups
        Add additional groups to consider as sensitive 

    .PARAMETER Sensitive
        Show only sensitive accounts.

    .PARAMETER Stealth
        Do not check service/server connectivity

    .PARAMETER GetSPNs
        Show SPNs instead of user's data

    .PARAMETER FullData
        Show more user data

    .EXAMPLE 
        Get-PotentiallyCrackableAccounts -Domain "IT.company.com"
        Returns all user accounts associated with SPN in the IT.company.com domain.
        
    .EXAMPLE
        Get-PotentiallyCrackableAccounts -FullData -Verbose
        Returns detailed information about all user accounts associated with SPN in the forest. Enable verbose mode 

    .EXAMPLE
        Get-PotentiallyCrackableAccounts -AddGroups "Remote Desktop Users" -Sensitive -Stealth -GetSPNs
        Returns all SPNs of sensitive user account in the forest. Consider "Remote Desktop Users" as sensitive and skip connectivity check.
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
            Write-output "Warning: The function level of domain: $($ChildDomain.name) is lower than 2008R2 - it may cause partial results"
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
                Write-output "Warning: The function level of domain: $($ChildDomain.Name) is lower than 2008R2 - it may cause partial results"
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

    #list of built-in sensitive groups (Administratos group conatins domain and enterprise admins) - did I missed a group?
    $SensitiveGroups = @("Administrators", "Account Operators", "Backup Operators", "Print Operators", "Server Operators", "Group Policy Creator Owners", "Schema Admins")
    if ($AddGroups)
    {
        Write-Verbose "Adding $AddGroups to the list of senstivie groups"
        $SensitiveGroups += $AddGroups
    }
    $AllSensitiveGroups = @()
    Write-Host "Gathering sensitive groups"
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

#========================================================================= Gathering users with SPN =========================================================================
  
    Write-Host "Gathering user accounts associated with SPN"
    #list of properties to retreive from AD
    $Properies = "msDS-UserPasswordExpiryTimeComputed", "msDS-AllowedToDelegateTo", "msDS-SupportedEncryptionTypes", "samaccountname", "userprincipalname", "useraccountcontrol", "displayname", "memberof", "serviceprincipalname", "pwdlastset", "description"
    foreach ($Property in $Properies)
    {
        $Searcher.PropertiesToLoad.Add($Property) | Out-Null
    }
    #filter user accounts with SPN except krbtgt account
    $Searcher.Filter = "(&(samAccountType=805306368)(servicePrincipalName=*)(!(samAccountName=krbtgt)))"
    $UsersWithSPN = @()
    foreach ($Path in $SearchList)
    {
        $Searcher.SearchRoot = $Path
        try {$UsersWithSPN += $Searcher.FindAll()}
        catch {Write-Warning "Could not communicate with the domain: $($Path -replace "LDAP://DC=" -replace ",DC=", ".") - Does it have trust?"}
    }
    Write-Verbose "Number of users that contain SPN: $($UsersWithSPN.Count)"
    
# ========================================================================= Gathering info about users =========================================================================

    $CurrentDate = Get-Date
    $AllData = @()
    foreach ($User in $UsersWithSPN)
    {
        Write-Verbose "Gathering info about the user: $($User.Properties.displayname)"
        
        #----------------------------------- Time stuff -----------------------------------
        
        #if the user's password has expiration date (works with FGPP) - https://msdn.microsoft.com/en-us/library/cc223410.aspx
        if ($user.Properties.'msds-userpasswordexpirytimecomputed' -ne 9223372036854775807) # 0x7FFFFFFFFFFFFFFF
        {
            $PasswordExpiryDate = [datetime]::FromFileTime([string]$user.Properties.'msds-userpasswordexpirytimecomputed')
            Write-Verbose "$($User.Properties.displayname)'s password will expire on $PasswordExpiryDate"
            $CrackWindow = $PasswordExpiryDate.Subtract($CurrentDate).Days
            Write-Verbose "Which means it has crack window of $CrackWindow days"
        }
        $PasswordLastSet = [datetime]::FromFileTime([string]$User.Properties.pwdlastset)
        $PasswordAge = $CurrentDate.Subtract($PasswordLastSet).Days

        #----------------------------------- UAC stuff -----------------------------------
        
        [int32]$UAC = [string]$User.Properties.useraccountcontrol
        #reading UAC attributes using bitmask - https://support.microsoft.com/en-us/kb/305144
        $IsEnabled = $true
        #if the user is disabled or lockedout
        if (($UAC -band 2) -eq 2 -or ($UAC -band 16) -eq 16) {$IsEnabled = $false} # 0x0002 / 0x0010
        $IsPasswordExpires = $true
        #if the user password never expires
        if (($UAC -band 65536) -eq 65536) {$IsPasswordExpires = $false} # 0x10000
        $Unconstrained = $false
        #if the user is trusted for Kerberos unconstrained delegation
        if (($UAC -band 524288) -eq 524288) {$Unconstrained = $true} # 0x80000
        #if the user is trusted for Kerberos constrained delegation with protocol transition
        if (($UAC -band 16777216) -eq 16777216) {$Constrained = [array]$User.Properties.'msds-allowedtodelegateto'} # 0x1000000
        else {$Constrained = $false}
        $EncType = "RC4-HMAC"
        [int32]$eType = [string]$User.Properties.'msds-supportedencryptiontypes'
        #if the user supports AES encryptions (MS-KILE 2.2.6) - https://msdn.microsoft.com/en-us/library/cc220375.aspx
        if ($eType)
        {
            if ($eType -band 16 -eq 16) {$EncType = "AES256-HMAC"} # 0x10
            elseif ($eType -band 8 -eq 8) {$EncType = "AES128-HMAC"} # 0x08
        }
        else 
        {
            #if the UF_USE_DES_KEY_ONLY bit is set (account can only use DES in Kerberos authentication)
            if ($UAC -band 2097152 -eq 2097152) {$EncType = "DES"} #0x200000
        }

        #----------------------------------- SPN stuff -----------------------------------
        
        $AccountRunUnder = @()
        #arranging SPNs to <service>/<server> format
        [array]$SPNs = $User.Properties.serviceprincipalname -replace "\*" -replace ":.*"  | Get-Unique 
        foreach ($SPN in $SPNs)
        {
            #splitting the SPN to service and server
            $SPN = $SPN -split("/")
            [array]$Service = switch -Wildcard ([string]$SPN[0])
            {
                "MSSQL*"                 {"MS SQL", @(1447)}
                "Exchange*"              {"Exchange"}
                {$_ -in "HTTP","WWW"}    {"Web", @(80,8080,443)}
                {$_ -in "TERMSRV","VNC"} {"Terminal Services"}
                "MONGO*"                 {"MongoDB Enterprise"}
                "WSMAN"                  {"WinRM"}
                "FTP"                    {"File Transfer", @(21)}
                default                  {$SPN[0]}
            }
            $RunUnder = [PSCustomObject][ordered]@{
                Service = $Service[0]
                Server  = $SPN[1]
                IsAccessible = "N/A"
            }

            if (!$Stealth)
            {
                $RunUnder.IsAccessible = "No"
                Write-Verbose "Checking connectivity to server: $($RunUnder.Server)"
                #if the service contains default ports to check
                if ($Service[1])
                {
                    $Socket = New-Object System.Net.Sockets.TcpClient
                    foreach ($Port in $Service[1])
                    {
                        #check if the service's default ports are open on ther server
                        try
                        {
                            $Socket.Connect($RunUnder.Server, $Port) | Out-Null
                            $RunUnder.IsAccessible = "Yes"
                        }
                        catch {Write-Verbose "The server: $($RunUnder.Server) is not accessiable on port: $Port"}   
                    }
                }
                else
                {
                    #if the server answers to one ping
                    if (Test-Connection -ComputerName $RunUnder.Server -Quiet -Count 1)
                    {
                        $RunUnder.IsAccessible = "Yes"
                    }
                    else {Write-Verbose "The server: $($RunUnder.Server) is not accessiable - Is it exist?"}
                }
            }
            $AccountRunUnder += $RunUnder      
        }
        
        if ($User.Properties.memberof)
        {
            #get sensitive groups that the user is a memberof
            $UserSensitiveGroups = (@(Compare-Object $AllSensitiveGroups $([array]$User.Properties.memberof) -IncludeEqual -ExcludeDifferent)).InputObject
        }
        $IsSensitive = $false
        #if the user is a member of a sensitive group or is allowed for Kerberos unconstrained delegation
        if ($UserSensitiveGroups -or $Unconstrained)
        {
            Write-Verbose "$($User.Properties.displayname) is sensitive"
            $IsSensitive = $true
        }

	# AH 2612
        $ofs = '<|>'
        $AccountRunUnder = [string]($AccountRunUnder | foreach {[string]$_})

        $UserData = [PSCustomObject][ordered] @{
            UserName        = [string]$User.Properties.samaccountname
            DomainName      = [string]$User.Properties.userprincipalname -replace ".*@"
            IsSensitive     = $IsSensitive
            EncType         = $EncType
            Description     = [string]$User.Properties.description
            IsEnabled       = $IsEnabled
            IsPwdExpires    = $IsPasswordExpires
            PwdAge          = $PasswordAge
            CrackWindow     = $CrackWindow
            SensitiveGroups = $UserSensitiveGroups -replace "CN=" -replace ",.*"
            MemberOf        = $User.Properties.memberof -replace "CN=" -replace ",.*"
            IsUnconstrained = $Unconstrained
            IsConstrained   = $Constrained
            RunsUnder       = $AccountRunUnder
            AssociatedSPNs  = [String]$User.Properties.serviceprincipalname   
        }
        $AllData += $UserData 
    }

    if ($Sensitive)
    {
       Write-Verbose "Removing non-sensitive users from the list"
       $AllData = $AllData | Where-Object {$_.IsSensitive}
    }  
    Write-Verbose "Number of users included in the list: $($AllData.UserName.Count)"
    if ($GetSPNs) {return @($AllData.AssociatedSPNs)}
    elseif ($FullData) {return $AllData}
    else {return $AllData | Select-Object UserName,DomainName,IsSensitive,EncType,Description,PwdAge,CrackWindow,RunsUnder}        
}


function Report-PotentiallyCrackableAccounts
{
    <#
    .SYNOPSIS
        Report juicy information about user accounts associated with SPN 

    .DESCRIPTION
        This function queries the Active Directory and retreive information about user accounts associated with SPN.
        This infromation could detremine if a service account is potentially crackable.
        User accounts associated with SPN are vulnerable to offline brute-forceing and they are often (by defualt)
        configured with weak password and encryption (RC4-HMAC).  
        Requires Active Directory authentication (domain user is enough). 

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
        Report-PotentiallyCrackableAccounts 
        Report all user accounts associated with SPN in entire forest. Save and open the report in CSV format in Documents folder 
        
    .EXAMPLE
        Report-PotentiallyCrackableAccounts -Type XML -Path C:\Report -DoNotOpen
        Report all user accounts associated with SPN in entire forest. Save the report in XML format in C:\Report folder  
    #>
    [CmdletBinding()]
    param
    (
        [ValidateSet("CSV", "XML", "HTML", "TXT")]
        [String]$Type = "CSV",
#        [String]$Path = "$env:USERPROFILE\Documents",
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
            $temp = $InputObject.RunsUnder
            if ([string]::IsNullOrEmpty($temp)) { return }


            $strArray = $temp | Foreach {"Service=$($_.Service),Server=$($_.Server),IsAccessible=$($_.IsAccessible)"}
            $InputObject.RunsUnder = $strArray
 
            $InputObject | ForEach {
                $Line = $_
                $stringBuilder = New-Object Text.StringBuilder
                $Null = $stringBuilder.AppendLine("[pscustomobject] @{")
                $OutputOrder | ForEach {
                        #$Null = $stringBuilder.AppendLine("`"$($_)`" = `"$(($line.$($_) | Out-String).Trim())`"")
                        $Null = $stringBuilder.AppendLine("`"$($_)`" = `"$((($line.$($_) | Out-String).Trim()) -replace "\n","<|>")`"")
                    }
                }
                $Null = $stringBuilder.AppendLine("}")
                Invoke-Expression $stringBuilder.ToString()
            }
        End {}
    }

    $FilePath = "$Path\$Name.$($Type.ToLower())"
    $FilePathCSV = "$Path\$Name" +".csv"

    $Report = Get-PotentiallyCrackableAccounts -FullData
    if ($Summary)
    {
       $Report = $Report | Select-Object UserName,DomainName,IsSensitive,PwdAge,CrackWindow,RunsUnder
    }
#NS 25-12    if ($Type -eq "CSV" ) {$Report | Convert-Output | Export-Csv $FilePath -Encoding UTF8 -NoTypeInformation}
    if ($Type -eq "CSV" ) {$Report | Export-Csv $FilePath -Encoding UTF8 -NoTypeInformation}
    elseif ($Type -eq "XML") 
    {
        $Report | Export-Clixml $FilePath -Encoding UTF8
        $Report | Convert-Output | Export-Csv $FilePathCSV -Encoding UTF8 -NoTypeInformation
    }
    elseif ($Type -eq "HTML") {$Report |  Convert-Output | ConvertTo-Html | Out-File $FilePath -Encoding utf8}
    elseif ($Type -eq "TXT") {$Report |  Convert-Output | Out-File $FilePath -Encoding utf8}  
    #Write-Host "$Type file saved in: $FilePath"
    write-host "`nRiskySPNs scan completed - check the results in `"\results\RiskySPNs`" folder" -ForegroundColor Yellow
    if (!$DoNotOpen)
    {
     #   Invoke-Item $FilePath
    }    
}

Get-PotentiallyCrackableAccounts
Report-PotentiallyCrackableAccounts  -Name RiskySPNs-test
#-Type CSV -DoNotOpen -Path Results/ -Name RiskySPNs-test}\"",