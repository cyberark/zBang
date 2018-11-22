<#----------------------------------------------------------------------------------------------------
Release Notes:

The SID History module queries the Active Directory and searches for
accounts that have SID history attribute.
 
Version 1: 14.6.16

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

        foreach ($history in $infoFromHistory)
        {            
            #write-host "SID History member of:"
            #$MemberOf = $history.Properties.memberof -replace "CN=" -replace ",.*"
            $HistoryMemberOf = $history.Properties.memberof -replace "CN=" -replace ",.*"
            #write-host $HistoryMemberOf
        }
        write-host ""

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
            initiallyMemberOf   = $User.Properties.memberof -replace "CN=" -replace ",.*"
            SIDHistoryName      = [string]$HistoryName 
            SIDHistoryMemberOf  = $HistoryMemberOf 
        } 
        $AllData += $UserData 
    }

    Write-Verbose "Number of users included in the list: $($AllData.UserName.Count)"

    #For now the FullData paramter is not relevant
    if ($FullData) {return $AllData}    
    else {return $AllData}   
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
        [ValidateSet("CSV", "XML", "HTML", "TXT")]
        [String]$Type = "CSV",
        [String]$Path = "$env:USERPROFILE\Documents",
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

    $Report = Get-UsersWithSIDHistory -FullData

    if ($Summary)
    {
        #---------------Not relevant for now-------------------------------------------------------------------------#
        #$Report = $Report | Select-Object UserName,DomainName,IsSensitive,PwdAge,CrackWindow,RunsUnder
    }
    if ($Type -eq "CSV" ) {$Report | Convert-Output | Export-Csv $FilePath -Encoding UTF8 -NoTypeInformation}
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
        Invoke-Item $FilePath
    }    
}