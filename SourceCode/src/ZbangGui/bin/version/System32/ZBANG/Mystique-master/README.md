# Mystique
### PowerShell tool to play with Kerberos S4U extensions
#### This module can assist blue teams to identify risky Kerberos delegation configurations as well as red teams to impersonate arbitrary users by leveraging KCD with Protocol Transition


## Usage
### Install the module
```powershell
Import-Module .\Mystique.psm1
```
**Or just load the script (you can also `IEX` from web)**
```powershell
. .\Mystique.ps1
```
Make sure `Set-ExecutionPolicy` is `Unrestricted` or `Bypass`
### Get information about a function
```powershell
Get-Help Find-DelegationAccounts -Full
```
All fucntions also have `-Verbose` mode
### Functions
    Add-SeTcbPrivilege      - Adds the "act as part of the operating system" (SeTcbPrivilege) privilege to a user
    Find-DelegationAccounts - Find accounts that are trusted for Kerberos delegation
    Get-CurrentIdentity     - Retreives information about the current identity
    New-Impersonation       - Impersonate a user using Protocol Transition
    Read-DelegatedFlag      - Checks if a user or users in a specific group can be delegated
    Undo-Impersonation      - Reverts an impersonated session to the original account


