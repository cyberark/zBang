using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using Microsoft.VisualBasic.FileIO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media;

namespace Graphviz4Net.WPF.Example
{
    using Graphs;
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;
    using System.DirectoryServices.ActiveDirectory;

    class SIDHistory
    {
        static int MAX_NON_DANGEROUS = 10;

        public static int Run(string selectedDomainName, string filename)
        {
            try
            {
                if (!File.Exists(filename))
                {
                    //MessageBox.Show("SIDHistory File Name Not Found", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    haveNoData();
                    return 0;
                }
                FileStream fs = File.OpenRead(filename);
                if (fs.Length < 20)
                {
                    fs.Close();
                    haveNoData();
                    //MessageBox.Show("No data in SIDHistory output file. Aborting!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return 0;
                }
                fs.Close();
                // ------ start by reading the file to memory ------
                Dictionary<Person, List<Machine>> usersDict = new Dictionary<Person, List<Machine>>();         // user name->
                Dictionary<Person, Machine> machinesDict = new Dictionary<Person, Machine>();                   // -> SIDs

                MainWindow Form = Application.Current.Windows[0] as MainWindow;
                var graph = new Graph<INotifyPropertyChanged>();


                var subGraph = new SubGraph<INotifyPropertyChanged> { Label = "SID History", borderColor = "Red" };
                graph.AddSubGraph(subGraph);
                Machine LegendMachine = null;

                // check first the total number of account with no risk on them
                // if less than 10 show them. If more or 10, don't show them at all
                int stlines = 0, totalNotDanger = 0;
                using (TextFieldParser start_prsr = new TextFieldParser(filename))
                {
                    start_prsr.TextFieldType = FieldType.Delimited;
                    start_prsr.SetDelimiters(",");
                    while (!start_prsr.EndOfData)
                    {
                        stlines++;
                        string[] fields = start_prsr.ReadFields();
                        if (stlines == 1) continue;

                        // parse domain name and open a dictionary card for it
                        string domainname = fields[1];

                        if (domainname != selectedDomainName && selectedDomainName != null)
                            continue;

                        if (fields[5].ToUpper() == "FALSE")
                            totalNotDanger++;
                    }
                } // endusing
                int lines = 0, countD = 0;
                try
                {
                    using (TextFieldParser parser = new TextFieldParser(filename))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");
                        while (!parser.EndOfData)
                        {
                            lines++;
                            string[] fields = parser.ReadFields();
                            if (lines == 1) continue;

                            // parse domain name and open a dictionary card for it
                            string domainname = fields[1];

                            if (domainname != selectedDomainName && selectedDomainName != null)
                                continue;

                            string accntSenseColor = "White";
                            string ava = "./Images/user_blue_cut.png";
                            string _LegendKey = "User";
                            if ((fields[5].ToUpper() != "FALSE" || (Form.ViewModel.ACLightPrivilegedList.Count > 0 && Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[3]))))
                                accntSenseColor = "#fee7ea"; // crimson"#FF8080";

                            if ((fields[5].ToUpper() == "FALSE" && (Form.ViewModel.ACLightPrivilegedList.Count > 0 && !Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[3])) &&
                                (fields[15].ToUpper() != "FALSE" || (Form.ViewModel.ACLightPrivilegedList.Count > 0 && Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[4])))))
                            {
                                // NS 31122017   ava = "./Images/unmanaged-priveleged-user-noback.png";
                                ava = "./Images/ic-user-red-reject.png";
                                _LegendKey = "Cloaked Privileged User";
                            }
                            else if (totalNotDanger >= MAX_NON_DANGEROUS && countD >= MAX_NON_DANGEROUS)
                                continue;

                            countD++;

                            var a = new Person(graph, Form.ViewModel)
                            {
                                Name = fields[1] + "\\" + fields[0],
                                ShowName = fields[2],
                                Avatar = ava,
                                BackColor = accntSenseColor,
                                Domain = domainname,
                                Importance = 0,
                                LegendKey = _LegendKey
                            };
                            // MAIN SID: if either it is marked or its sid is in the ACLight list
                            /*
                            if (fields[5].ToUpper() != "FALSE" || Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[3]))
                                a.Avatar = "./Images/attacker_redback.png";
                                */
                            if (!usersDict.ContainsKey(a))
                            {
                                subGraph.AddVertex(a);
                                string nnn = fields[0];
                                if (fields[0].Contains("\\"))
                                {
                                    nnn = fields[0].Split('\\')[1];
                                }
                                List<Machine> accounts = new List<Machine>();
                                //FindByIdentitySid(fields[3]);
                                var b = new Machine(graph, Form.ViewModel)
                                {
                                    Name = fields[3],
                                    showName = nnn + "@" + fields[1], //fields[3],
                                                                      //Avatar = "./Avatars/Desktop.png",
                                    Avatar = "./Images/id.png",
                                    LegendKey = "Main SID"
                                };
                                if (LegendMachine == null) LegendMachine = b;
                                if (fields[5].ToUpper() != "FALSE" || Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[3]))
                                    b.Avatar = "./Images/id-privileged.png";

                                accounts.Add(b);
                                subGraph.AddVertex(b);
                                //FindByIdentitySid(fields[4]);
                                string nnnb = fields[12];
                                if (fields[12].Contains("\\"))
                                {
                                    nnnb = fields[12].Split('\\')[1];
                                }
                                var c = new Machine(graph, Form.ViewModel)
                                {
                                    Name = fields[12],//fields[4],
                                    showName = nnnb + "@" + fields[13], //fields[4],
                                                                        //Avatar = "./Avatars/SysAdmin.png",
                                    Avatar = "./Images/id-history-blue.png",
                                    LegendKey = "SID History"
                                };
                                // MAIN SID: if either it is marked or its sid is in the ACLight list
                                if (fields[15].ToUpper() != "FALSE" || Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[4]))
                                    c.Avatar = "./Images/id-history-red.png";

                                accounts.Add(c);
                                subGraph.AddVertex(c);
                                usersDict.Add(a, accounts);

                                graph.AddEdge(new Edge<INotifyPropertyChanged>(a, b, new Arrow() { Stroke = "Gray" })
                                {
                                    edgeColor = "Gray",
                                    edgeStrokeThickness = "1",
                                    Label = "",
                                    labelToolTip = ""
                                });
                                graph.AddEdge(new Edge<INotifyPropertyChanged>(a, c, new Arrow() { Stroke = "Gray" })
                                {
                                    edgeColor = "Gray",
                                    edgeStrokeThickness = "1",
                                    Label = "",
                                    labelToolTip = ""
                                });
                            } // endif new user name
                        } // endwhile
                    } // endusing
                } // endtry
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                    return 0;
                }

                // show legend... TESTING!
                if (Form.showLegend)
                    Form.ViewModel.addLegend(graph, LegendMachine.Name);

                Form.GraphLayout.Graph = graph;
                return 1;
            }
            catch( Exception eee)
            {
                MessageBox.Show("Error in SID History. Please Report to CyberArk\n" + eee.Message);
                return 0;
            }
        } // endfunc Run



        // SID must be in Security Descriptor Description Language (SDDL) format
        // The PrincipalSearcher can help you here too (result.Sid.ToString())
        static public void FindByIdentitySid(string sid)
        {
            var domains = Forest.GetCurrentForest().Domains.Cast<Domain>();
            PrincipalContext ctx = new PrincipalContext(ContextType.Machine);

            foreach (var domain in domains)
            {
                UserPrincipal user = UserPrincipal.FindByIdentity(
                ctx,
                IdentityType.Sid,
                sid /*"S-1-5-21-2422933499-3002364838-2613214872-12917"*/);

                DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://" + user.DistinguishedName);

                Console.WriteLine(user.DistinguishedName);
            }
        }


        static void haveNoData()
        {
            MainWindow _Form = Application.Current.Windows[0] as MainWindow;
            var _graph = new Graph<INotifyPropertyChanged>();
            var _subGraph = new SubGraph<INotifyPropertyChanged> { Label = "SID History", borderColor = "Red" };
            _graph.AddSubGraph(_subGraph);
            var a = new Person(_graph, _Form.ViewModel)
            {
                Name = "No Data",
                ShowName = "Scan Yielded No Data",
                Avatar = "./Images/generic.png",
                BackColor = "White",
                Domain = "Forest",
                Importance = 0,
                LegendKey = "Users"
            };
            _subGraph.AddVertex(a);
            _Form.GraphLayout.Graph = _graph;
        }
    } // endclass
} // end namespace
