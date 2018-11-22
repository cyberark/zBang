![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/zBang%20tool.png "zBang risk assessment tool")  
**zBang is a special risk assessment tool that detects potential privileged account threats in the scanned network.**  
  
Organizations and red teamers can utilize zBang to identify potential attack vectors and improve the security posture of the network. The results can be analyzed with the graphic interface or by reviewing the raw output files.  
  
The tool is built from five different scanning modules:  
1.	**ACLight scan** - discovers the most privileged accounts that must be protected, including suspicious Shadow Admins.
2.	**Skeleton Key scan** - discovers Domain Controllers that might be infected by Skeleton Key malware.
3.	**SID History scan** - discovers hidden privileges in domain accounts with secondary SID (SID History attribute).
4.	**RiskySPNs scan** - discovers risky configuration of SPNs that might lead to credential theft of Domain Admins
5.	**Mystique scan** - discovers risky Kerberos delegation configuration in the network.  
  
# Execution Requirements
1.	Run it with any domain user. The scans do not require any extra privileges; the tool performs read-only LDAP queries to the DC.
2.	Run the tool from a domain joined machine (a Windows machine).
3.	PowerShell version 3 or above and .NET 4.5 (it comes by default in Windows 8/2012 and above).
  
# Quick Start Guide
1.	Download and run the release version from this GitHub repository [link] or compile it with your favorite compiler.
2.	In the opening screen, choose what scans you wish to execute.  
In the following example, all five scans are chosen:  
![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/opening%20menu.png "Opening menu")  
3.	To view demo results, click “Reload.”  
zBang tool comes with built-in initiating demo data; you can view the results of the different scans and play with the graphic interface.
4.	To initiate new scans in your network, click “Launch.” A new window will pop up and will display the status of the different scans.



