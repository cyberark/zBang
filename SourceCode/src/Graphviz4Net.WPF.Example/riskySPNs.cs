using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows;
using Microsoft.VisualBasic.FileIO;
using System.Threading;
using System.ComponentModel;
using System.Windows.Media;


namespace Graphviz4Net.WPF.Example
{
    using Graphs;
    using System.DirectoryServices.AccountManagement;

    public class riskySPNs
    {
        public static void Run(string selectedDomainName, string filename)
        {
            if (!File.Exists(filename))
            {
                //MessageBox.Show("RiskySPNs File Name Not Found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                haveNoData();
                return;
            }
            try
            {
                FileStream fs = File.OpenRead(filename);
                if (fs.Length < 100)
                {
                    fs.Close();
                    //MessageBox.Show( "No data in RiskySPN output file. Aborting!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    haveNoData();
                    return;
                }
                fs.Close();
            }
            catch
            {
                haveNoData();
                return;
            }
            // ------ start by reading the file to memory ------
            Dictionary<string, List<Person>> domainDict = new Dictionary<string, List<Person>>();           // domain ->
            Dictionary<Person, List<Person>> servicesDict = new Dictionary<Person, List<Person>>();         // services ->
            Dictionary<Person, Machine> machinesDict = new Dictionary<Person, Machine>();                     // -> machine

            MainWindow Form = Application.Current.Windows[0] as MainWindow;
            var graph = new Graph<INotifyPropertyChanged>();

            var subGraph = new SubGraph<INotifyPropertyChanged> { Label = "RiskySPNs", borderColor = "Gray" };
            graph.AddSubGraph(subGraph);
            var subGraphTarget = new SubGraph<INotifyPropertyChanged> { Label = "Target", borderColor = "Red" };
            graph.AddSubGraph(subGraphTarget);

            string reasonRisky = "";

            int lines = 0;
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

                        string accntSenseColor = "LightYellow";
                        string avatarName = "./Images/user_blue_cut.png";
                        string lkey = "Users/Computers";


                        // check SID against aclight
                        if (fields[2].ToUpper() != "FALSE" || Form.ViewModel.ACLightPrivilegedList.Count > 0 && Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[15]))
                        {
                            // is it delegation??? if yes, disregard it... watch it over the Mystique part
                            if (fields[11].ToUpper() != "TRUE" || fields[12].ToUpper() != "TRUE")
                            {
                                accntSenseColor = "#fee7ea"; // crimson  "Crimson";
                                                             // NS 31122017    avatarName = "./Images/unmanaged-priveleged-user-noback.png";
                                avatarName = "./Images/ic-user-red-reject.png";
                                lkey = "Risky User";

                                if (Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[15]))
                                    reasonRisky = "Admin via ACLight";
                                else
                                    reasonRisky = "Admin Account";

                            }
                        }
                        // NS 24/12 12:51
                        // check if domain\\user name exists in the privileged list of ACLight
                        /*
                        if (Form.ViewModel.ACLightPrivilegedList.ContainsKey(fields[1] + "\\" + fields[0]))
                        {
                            accntSenseColor = "#fee7ea"; // crimson "Crimson";
                                                         // NS 31122017 avatarName = "./Images/unmanaged-priveleged-user-noback.png";
                            avatarName = "./Images/ic-user-red-reject.png";
                            lkey = "Risky User";
                        }
                        */

                        var a = new Person(graph, Form.ViewModel)
                        {
                            Name = fields[0] + "\\" + fields[1],
                            ShowName = fields[0],
                            Avatar = avatarName,
                            BackColor = accntSenseColor,
                            Domain = domainname,
                            Importance = 0,
                            LegendKey = lkey
                        };

                        if (!domainDict.ContainsKey(domainname))
                        {
                            List<Person> accounts = new List<Person>();
                            accounts.Add(a);
                            domainDict.Add(domainname, accounts);
                        }
                        else
                        {
                            List<Person> accnts = domainDict[domainname];
                            bool foundPerson = false;
                            foreach (Person pers in accnts)
                                if (pers.Name == a.Name)
                                    foundPerson = true;
                            if (!foundPerson)
                            {
                                accnts.Add(a);
                            }
                        }

                        // next read the services and connect to user account names
                        string spns = fields[14];
                        string[] Spns = spns.Split('|');

                        for (int i = 0; i < Spns.Length; i++)
                        {
                            string names = trimSpnNames(Spns[i]);
                            string domm = names;
                            string splitted = "";
                            if (names.IndexOf("/") >= 0)
                            {
                                domm = names.Split('/')[1];
                                splitted = names.Split('/')[1];
                            }
                            int pos = domm.IndexOf('.');
                            if (pos > 0)
                            {
                                domm = domm.Substring(0, pos) + '\n' + domm.Substring(pos + 1);
                            }

                            string srvColor = "White";
                            if (accntSenseColor == "#fee7ea")
                                srvColor = accntSenseColor; // "Crimson";

                            var b = new Person(graph, Form.ViewModel)
                            {
                                ShowName = names.Split('/')[0],
                                Name = names.Split('/')[0] + "\n" + splitted,
                                BackColor = srvColor,  //"LightGreen",
                                Avatar = "./Avatars/Gears.png",
                                Domain = domm,
                                LegendKey = "Services"
                            };
                            if (!servicesDict.ContainsKey(a))
                            {
                                List<Person> servicePersons = new List<Person>();
                                servicePersons.Add(b);
                                servicesDict.Add(a, servicePersons);
                            }
                            else
                            {
                                List<Person> servicePersons = servicesDict[a];
                                servicePersons.Add(b);
                            }

                            //  ------   R U N S    U N D E R  --------
                            // now deal with COMPUTERS or SERVERS...
                            string[] servers = fields[13].Split('|');
                            foreach (string server in servers)
                            {
                                string snames = trimSpnNames(server);
                                string[] sGoNames = snames.Substring(2, snames.Length - 3).Split(';');
                                string serverName = "unknown", serviceName = "unknown";
                                bool isAccessible = false;

                                foreach (string s in sGoNames)
                                {
                                    if (s.Split('=')[0].TrimStart() == "Server")
                                        serverName = s.Split('=')[1];
                                    if (s.Split('=')[0].TrimStart() == "Service")
                                        serviceName = s.Split('=')[1];
                                    if (s.Split('=')[0].TrimStart() == "IsAccessible")
                                        isAccessible = (s.Split('=')[1] == "Yes" ? true : false);
                                }

                                string serviceNameServer = names;
                                if (names.IndexOf("/") > 0)
                                {
                                    serviceNameServer = names.Split('/')[1].Split(':')[0];
                                }
                                if (serverName.ToLower() == serviceNameServer.ToLower())
                                {
                                    if (!machinesDict.ContainsKey(b))
                                    {
                                        var d = new Machine(graph, Form.ViewModel)
                                        {
                                            Name = serverName,
                                            showName = serverName,
                                            //!ShowName = serverName,
                                            Avatar = "./Avatars/servers.png",
                                            BackColor = (isAccessible ? "LightBlue" : "LightGray"),
                                            LegendKey = "Server Machines"
                                            //!Domain = domainname
                                        };
                                        machinesDict.Add(b, d);
                                    } // endif don't have service in dictionary
                                    else
                                        throw (new Exception("RiskySPNS => probably same objects per user!"));
                                } // endforeach
                            } // endforeach all servers
                        } // endfor all Spns
                    } // endwhile
                } // endusing
            } // endtry
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }


            Machine LegendMachine = null;
            Person LegendPerson = null;

            // now plug everything to the graph
            foreach (string key in domainDict.Keys)
            {
                var a = new Person(graph, Form.ViewModel) {
                    Name = key,
                    ShowName = key,
                    //Avatar = "./Avatars/avatar1.jpg",
                    Avatar = "./Images/New target.png",
                    BackColor = "#fee7ea" /*"Red"*/,
                    Domain = "Forest",
                    Importance = 0,
                    LegendKey = "Domain Targets"
                };
                subGraphTarget.AddVertex(a);
                LegendPerson = a;

                foreach (Person accnt in domainDict[key])
                {
                    subGraph.AddVertex(accnt);
                    if (accnt.BackColor.ToLower() == "#fee7ea" /*"crimson"*/)
                    {
                        graph.AddEdge(new Edge<INotifyPropertyChanged>(accnt, a, new Arrow() { Stroke = "Black" })
                        {
                            edgeColor = "Black",
                            edgeStrokeThickness = "1",
                            Label = reasonRisky,
                            labelToolTip = "This account is a member of the domain\nIf the account's color is red then it is also privileged!"
                        });
                    }
                    else
                    {
                        graph.AddEdge(new Edge<INotifyPropertyChanged>(accnt, a, new BlindArrow())
                        {
                            edgeColor = "Black",
                            edgeStrokeThickness = "1",
                            Label = "blind",
                            labelToolTip = "This account is a member of the domain\nIf the account's color is red then it is also privileged!"
                        });
                    }

                    List<Person> servicesList = servicesDict[accnt];
                    foreach (Person p in servicesList)
                    {
                        subGraph.AddVertex(p);
                        graph.AddEdge(new Edge<INotifyPropertyChanged>(p, accnt, new Arrow() { Stroke = "Black" })
                        {
                            edgeColor = "Black",
                            edgeStrokeThickness = "1",
                            Label = "",
                            labelToolTip = "Don't know what?!"
                        });

                        if (machinesDict.ContainsKey(p))
                        {
                            Machine machine = machinesDict[p];
                            if (LegendMachine == null)
                                LegendMachine = machine;

                            bool foundm = false;
                            foreach (Person m in graph.Vertices)
                            {
                                if( m.Name == machine.Name)
                                {
                                    graph.AddEdge(new Edge<INotifyPropertyChanged>( m, p, new Arrow() { Stroke = "Black" })
                                    {
                                        edgeColor = "Black",
                                        edgeStrokeThickness = "1",
                                        Label = "",
                                        labelToolTip = "Don't know what?!"
                                    });
                                    foundm = true;
                                    break;
                                }
                            } // endforeach all vertices
                            if (!foundm)
                            {
                                subGraph.AddVertex(machine);
                                graph.AddEdge(new Edge<INotifyPropertyChanged>( machine, p, new Arrow() { Stroke = "Black" })
                                {
                                    edgeColor = "Black",
                                    edgeStrokeThickness = "1",
                                    Label = "",
                                    labelToolTip = "Don't know what?!"
                                });
                            }
                        }
                    } // endforeach persons in services lists
                } // endfor each
            } // endforeach domains added

            // show legend... TESTING!
            if( Form.showLegend)
                Form.ViewModel.addLegend(graph, /*LegendMachine.Name*/LegendPerson.Name);
            
            Form.GraphLayout.Graph = graph;
        } // endfunc Run

        /// <summary>
        ///
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        static string trimSpnNames( string input)
        {
            string output = input;
            if (output.StartsWith(">"))
                output = output.Substring(1);
            if (output.EndsWith("\r<"))
                output = output.Substring(0, output.Length - 2);
            else if( output.EndsWith( "<"))
                output = output.Substring(0, output.Length - 1);


            return output;

        }


       
        static void haveNoData()
        {
            MainWindow _Form = Application.Current.Windows[0] as MainWindow;
            var _graph = new Graph<INotifyPropertyChanged>();
            var _subGraph = new SubGraph<INotifyPropertyChanged> { Label = "RiskySPNs", borderColor = "Gray" };
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
