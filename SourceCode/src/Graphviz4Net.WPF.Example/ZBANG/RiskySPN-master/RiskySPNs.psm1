<# 
    Author: Matan Hart (@machosec)
    Contact: cyberark.labs@cyberark.com
    License: GNU v3
    Required Dependencies: None
    Optional Dependencies: None
#>
#Import this module to use all RiskySPNs scripts (require all .ps1 files to be in the directory)

#PS C:\> Import-Module .\RiskySPNs.psm1
Get-ChildItem (Join-Path $PSScriptRoot *.ps1) | % {Import-Module $_.FullName -WarningAction SilentlyContinue}
