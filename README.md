![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/zBang%20tool.png "zBang risk assessment tool")  
  
**zBang is a special risk assessment tool that detects potential privileged account threats in the scanned network.**  
  
Organizations and red teamers can utilize zBang to identify potential attack vectors and improve the security posture of the network. The results can be analyzed with the graphic interface or by reviewing the raw output files.  
  
**The tool is built from five different scanning modules:**  
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
  
<p align="center">
  <img width="556" height="274" src="https://github.com/Hechtov/Photos/blob/master/zBang/opening%20menu.png">
</p>
  
3.	To view demo results, click “Reload.”  
zBang tool comes with built-in initiating demo data; you can view the results of the different scans and play with the graphic interface.
4.	To initiate new scans in your network, click “Launch.” A new window will pop up and will display the status of the different scans.
  
<p align="center">
  <img src="https://github.com/Hechtov/Photos/blob/master/zBang/aclight.png">
</p>
  
5.	When the scans are completed, there will be a message saying the results were exported to an external zip file.
  
<p align="center">
  <img src="https://github.com/Hechtov/Photos/blob/master/zBang/zbang%20finished.png">
</p>
  
6.	The results zip file will be in the same folder of zBang and will have a unique name with the time and the date of the scans. You can also import previous results into the zBang GUI without the need of rerunning the scans.  
To import previous results, click “Import” in the zBang’s opening screen.  
  
# Go Over zBang Results:
a.	ACLight scan:
![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/aclight%20result.png "ACLight results")  
i.	Choose the domain you have scanned.
ii.	You will see a list of the most privileged accounts that were discovered.
iii.	On the left side - view “standard” privileged accounts that get their privileges due to their group membership.
iv.	On the right side - view “Shadow Admins.” Those accounts get their privileges through direct ACL permissions assignment. Those accounts might be stealthier than standard “domain admin” users, and therefore, they might not be as secure as they should be. Attackers often target and try to compromise such accounts. 
v.	On each account, you can double click and review its permissions graph. It may help you understand why this account was classified as privileged.
![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/aclight%20tree.png "ACLight permissions tree")
vi.	The different abusable ACL permissions are described in a small help page. Click the “question mark” in the upper right corner to view:  
![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/risky%20ACLs.png "The abusable ACLs")  
vii.	More details on the threat of Shadow Admins are available in the blog post -
“Shadow Admins – The Stealthy Accounts That You Should Fear The Most”:
https://www.cyberark.com/threat-research-blog/shadow-admins-stealthy-accounts-fear/
viii.	For manual examination of the scan results,  unzip the saved zBang results file and check the results folder:
"[Path of the zBang’s unzipped results file]\ACLight-master\Results”, contains a summary report - “Privileged Accounts - Layers Analysis.txt”.
ix.	On each of the discovered privileged accounts: 
1.	Identify the privileged account.
2.	Reduce unnecessary permissions from the account.
3.	Secure the account. After validating these three steps, you can mark the account with a “V” in the small selection box, turning it green on the interface.
x.	The goal is to make all the accounts marked as “secured” with the green color.
  
b.	Skeleton Key scan
i.	In the scan page (click the relevant bookmark in the above section), there will be a list of all the scanned DCs. 
ii.	Make sure all of them are clean and marked with green.
iii.	If the scan finds a potential infected DC, it is crucial to initiate an investigation process.
![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/skeleton%20key.png "Skeleton Key scan results")  
iv.	More details on Skeleton Key malware are available in the blog post
“Active Directory Domain Controller Skeleton Key Malware & Mimikatz” by @PyroTek3: https://adsecurity.org/?p=1255
  
c.	SID History scan
i.	In this scan page, there will be a list of the domain accounts with secondary SID (SID History attribute).
ii.	Each account will have two connector arrows, one to the left for its main SID, the other to the right for its secondary SID (with the mask icon).
iii.	If the main SID is privileged, it will be in red, and if the SID history is privileged, there will be displayed as a red mask.
iv.	You should search for the possible very risky situations, in which an account has a non-privileged main SID but at the same time has a privileged secondary SID.
This scenario is very suspicious and you should check this account and investigate why it received a privileged secondary SID. Make sure it wasn’t added by a potential intruder in the network.
![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/SIDhistory.png "SID History scan results")  
* For a visualization convenience, if a large number of accounts with non-privileged SID history are present (more than ten), they will be filtered out from the display, as those accounts are less sensitive.
v.	For manual examination of the scan results, unzip the saved zBang results file and check csv file:
“[Path of the zBang’s unzipped results file]\SIDHistory\Results\Report.csv".
vi.	More details on abusing SID History are available in the blog post 
“Security Focus: sIDHistory” by Ian Farr: https://blogs.technet.microsoft.com/poshchap/2015/12/04/security-focus-sidhistory-sid-filtering-sanity-check-part-1-aka-post-100/
  
d.	RiskySPNs scan
i.	In the scan results page, there will be a list of all the SPNs that registered with user accounts.
ii.	If the user account is a privileged account, it will be in red.
iii.	It is very risky to have SPNs that are registered under privileged accounts. Try and change/disable those SPNs. Use machine accounts for SPNs or reduce unnecessary permissions from the users who have SPNs registered to them. It’s also recommended to assign strong passwords to those users, and implement automatic rotation of each password. 
![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/riskySPNs.png "riskySPN scan results")   
iv.	For manual examination of the scan results, unzip the saved zBang results file and check csv file:
“[Path of the zBang’s unzipped results file]\RiskySPN-master\Results\RiskySPNs-test.csv".
v.	More details on Risky SPNs are available in the blog post-
“Service Accounts – Weakest Link in the Chain”:
https://www.cyberark.com/blog/service-accounts-weakest-link-chain/
  
e.	Mystique scan
i.	The scan result page includes a list of all the discovered accounts trusted with delegation permissions.
ii.	There are three delegation types: Unconstrained, Constrained and Constrained with Protocol Transition. The account color corresponds to its delegation permission type.
iii.	Disable old and unused accounts trusted with delegation rights. In particular, check the risky delegation types of “Unconstrained” and “Constrained with Protocol Transition.” Convert “Unconstrained” delegation to “Constrained” delegation so it will be permitted only for specific needed services. “Protocol Transition” type of delegation must be revalidated and disabled, if possible.
![alt text](https://github.com/Hechtov/Photos/blob/master/zBang/mystique.png "Mystique scan results") 
iv.	For manual examination of the scan results, unzip the saved zBang results file and check csv file:
“[Path of the zBang’s unzipped results file]\Mystique-master\Results\delegation_info.csv".
v.	More details on risky delegation configuration are available in the blog post - 
“Weakness Within: Kerberos Delegation”:
https://www.cyberark.com/threat-research-blog/weakness-within-kerberos-delegation/
  
# Performance
zBang runs quickly and doesn’t need any special privileges over the network. As the only communication required is to the domain controller through legitimate read-only LDAP queries, a typical execution time of zBang on a network with around 1,000 user accounts will be seven minutes.  
When you intend to scan large networks with multiple trust-connected domains, it’s recommended to check the domain trusts configuration or run zBang separately from within each domain to avoid possible permission and connectivity issues.  

# Authors
zBang was developed by CyberArk Labs as a quick and dirty POC intended to help security teams worldwide. Feedback and comments are welcome.  
  
**Main points of contact:**  
Asaf Hecht ([@Hechtov](https://twitter.com/Hechtov)), Nimrod Stoler ([@n1mr0d5](https://twitter.com/n1mr0d5)) and Lavi Lazarovitz ([@\_\_Curi05ity\_\_](https://twitter.com/__Curi05ity__))  
