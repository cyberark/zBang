# a special version of Mystique for Zbang tool

# this is script version: 1

function Find-DelegationAccounts
{
    <#
        .SYNOPSIS
            Find accounts that are trusted for Kerberos delegation.
            The output csv file will be used in BloodHound.

            Modified by: Asaf Hecht (@Hechtov)
            Source: https://github.com/cyberark/Mystique (@machosec) - CyberArk Labs
            https://github.com/PowerShellMafia/PowerSploit/tree/master/Recon (@harmjoy) - Will Schroeder
            
        .DESCRIPTION 
            Queries the Active Directory for accounts that are trusted for delegation (Unconstrained, Contrained and Protocol Transition).
            The output csv file will be used in BloodHound.
            The query filters Domain Controllers and disabled accounts.

        .PARAMETER Domain
            Domain name to query. Defaults to current forest

        .PARAMETER Type
            Delegation type to filter. Options are Unconstraind Delegation, Constrained Delegation and Protocol Transition

        .PARAMETER GetTicket
            Obtain a service ticket for all SPNs that are vulnerable for imperosnation (constrained delegation)

        .PARAMETER csvWriter
            Csv Writer pointer as part of BloodHound intergration

        .PARAMETER forBloodHound
            If the function is for BloodHound intergation

        .EXAMPLE 
            Find-AccountsTrustedForDelegation
            Retreives information about accounts that are trusted for delegation in the current forest

        .EXAMPLE
            Find-AccountsTrustedForDelegation -Domain CompanyA -Type "Protocol Transition"
            Retreives information about accounts that are trusted for constrained delegation with protocol transition in "CompanyA" domain.
    #>

    [CmdletBinding()]
    param
    (
        [string]$Domain,
        [ValidateSet("Unconstrained Delegation","Constrained Delegation", "Protocol Transition")]
        [string]$Type,
        [switch]$GetTicket,
        [String]$csvOutput,
        $csvWriter,
        [switch]$forBloodHound
    )
    if ($Domain)
    {
        $TargetDomain = New-Object System.DirectoryServices.ActiveDirectory.DirectoryContext('Domain', $Domain)
        try 
        {
            $Scope = [System.DirectoryServices.ActiveDirectory.Domain]::GetDomain($TargetDomain).Name
        }
        catch 
        {
            Write-Error "Error getting domain ""$Domain"" object: $($_.Exception.Message)" -ErrorAction Stop
        }
    }
    else 
    {
        try 
        {
            $Scope = [System.DirectoryServices.ActiveDirectory.Forest]::GetCurrentForest().Domains.Name
        }
        catch 
        {
            Write-Error "Error getting current forest object: $($_.Exception.Message)" -ErrorAction Stop
        }
    }
    $Searcher = New-Object System.DirectoryServices.DirectorySearcher
    $Properties = "msDS-AllowedToDelegateTo", "samaccounttype", "useraccountcontrol", "samaccountname", "serviceprincipalname", "description", 'distinguishedname', 'cn', 'dnshostname'
    foreach ($Property in $Properties)
    {
        $Searcher.PropertiesToLoad.Add($Property) | Out-Null
    }
    #filter all enabled accounts that are trusted for unconstrained or constrained delegation without DCs
    $Searcher.Filter = "(&(!(userAccountControl:1.2.840.113556.1.4.803:=2))(|(userAccountControl:1.2.840.113556.1.4.803:=524288)(msDS-AllowedToDelegateTo=*))(!((userAccountControl:1.2.840.113556.1.4.803:=8192))))"
    $DelegationData = @()
    Write-Verbose "Querying $($Scope.Count) domains"
    foreach ($Domain in $Scope)
    {
        $Searcher.SearchRoot = 'LDAP://DC=' + ($Domain -Replace ("\.",',DC='))
        Write-Verbose "Querying $Domain"
        try 
        {
            $Accounts = $Searcher.FindAll().Properties
            if ($Accounts)
            {
                foreach ($Account in $Accounts)
                {
                    $Account.Add("domain", ($Domain -split "\.")[0].ToUpper())
                    $DelegationData += $Account
                }
            }
        }
        catch 
        {
            Write-Warning "couldn't query domain ""$Domain"" - $($_.Exception.Message)" 
        }
    }
    $delegationForZbang = @()
    if ($DelegationData)
    {
        Write-Verbose "Found $($DelegationData.Count) accounts that are trusted for delegation"
        $DelegationAccounts = @()
        foreach ($Account in $DelegationData)
        {
            [int32]$UAC = [string]$Account.useraccountcontrol
            #if the account is trusted for Kerberos unconstrained delegation
            if ($UAC -band 0x80000)
            {
                $DelegationType = "Unconstrained Delegation"
                $AllowedServices = "Any service (SPN)"
                $VulnerableServices = "Any service (SPN)"
            }
            #The account is trusted for Kerberos constrained delegation
            else
            {
                $DelegationType = "Constrained Delegation"
                #if the account is trusted for protocol transition
                if ($UAC -band 0x1000000) 
                {
                    $DelegationType = "Protocol Transition"
                }
                $AllowedServices = [array]$Account.'msds-allowedtodelegateto'
                $VulnerableServices = @()
                foreach ($SPN in $AllowedServices)
                {
                    $Searcher.Filter = "(serviceprincipalname=$SPN)"
                    $VulnerableServices += $Searcher.FindAll().Properties.serviceprincipalname
                }
                #requesting TGS (service ticket) for the target SPN
                if ($GetTicket)
                {
                    Add-Type -AssemblyName System.IdentityModel
                    foreach ($SPN in $VulnerableServices)
                    {
                        try
                        {
                            Write-Verbose "Asking for TGS for the SPN: $SPN"
                            #requesting TGS (service ticket) for the target SPN
                            New-Object System.IdentityModel.Tokens.KerberosRequestorSecurityToken -ArgumentList $SPN
                        }
                        catch
                        {
                            Write-Warning "Could not request a TGS for the SPN: $SPN - Is it exists?"
                        }
                    }
                }
            }
            $AccountType = "Computer"
            if ($Account.samaccounttype -eq "805306368")
            {
                $AccountType = "User"
            }
            $distinguishedname = ($Account.distinguishedname)[0]
            [string]$accountName = $Account.samaccountname
            $accountName += "@"+ $distinguishedname.subString($distinguishedname.IndexOf("DC=")) -replace 'DC=','' -replace ',','.'
            
            $AccountData = New-Object -TypeName psobject -Property @{
                AccountName          = $accountName
                AccountDN            = $distinguishedname
                TrustedFor           = $DelegationType
                AccountType          = $AccountType
                Description          = [string]$Account.description
                AllowedServices      = $AllowedServices
                VulnerableServices   = $VulnerableServices | Sort-Object -Unique
                AccountSPNs          = $Account.serviceprincipalname
                DnsHostname          = [string]$Account.dnshostname
            } 
            $DelegationAccounts += $AccountData

            # added for Zbang integration:
            $ofs = ","
            $VulnerableServicesString = [string]($VulnerableServices | Sort-Object -Unique | foreach {[string]$_})
            $AccountInfo = [PSCustomObject][ordered] @{
                GroupsName = ""
                AccountName = ""
                AccountType = ""
                AccountAllowedForDelegation = ""
                "|" = "|" 
                DelegationAccountName = $accountName
                DelegationAccountType = $AccountType
                DelegationType = $DelegationType
                VulnerableServices = $VulnerableServicesString
                newEscalationPath = ""
            } 
            $delegationForZbang += $AccountInfo 

        }
    }
    else 
    {
        Write-Warning "Couldn't find any accounts that are trusted for delegation"
    }
    if ($Type)
    {
        return $DelegationAccounts | where {$_.TrustedFor -eq $Type}
    }
    $resultsPath = $PSScriptRoot + "\Results\delegation_info.csv"
    $delegationForZbang | Export-Csv -NoTypeInformation $resultsPath    
}

Find-DelegationAccounts
