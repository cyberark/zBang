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

    public class Mystique
    {
        static int startIndex = 5;

        public enum  DELEGATION_TYPE
        {
            TYPE_UNCONSTRAINED,
            TYPE_PROTOCOL_TRANSITION,
            TYPE_CONSTRAINED
        }
        class delegationServices
        {
            public string serviceName;
            //public Person srv;
            public Machine srv;
            public DELEGATION_TYPE type;
        };

        public static void Run(string selectedDomainName, string filename)
        {
            if (!File.Exists(filename))
            {
                //MessageBox.Show("Mystique File Name Not Found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            Dictionary<string, List<Person>> accountDict = new Dictionary<string, List<Person>>();
            Dictionary<Person, List<delegationServices>> servicesDict = new Dictionary<Person, List<delegationServices>>();
            Dictionary<string, Machine> serviceNames = new Dictionary<string, Machine>();

            MainWindow Form = Application.Current.Windows[0] as MainWindow;
            var graph = new Graph<INotifyPropertyChanged>();

            int stopTheColorParade = 1;


            var subGraph = new SubGraph<INotifyPropertyChanged> { Label = "Mystique", borderColor = "Blue" };
            graph.AddSubGraph(subGraph);

            // just a test
            graph.Rankdir = RankDirection.TopToBottom;

            string syy = subGraph.GetAttributes();
            string serviceLegedName = null;

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
                        string accountName = fields[0 + startIndex].Split('@')[0];
                        string domainname = fields[0 + startIndex].Split('@')[1];

                        /*
                        if (domainname != selectedDomainName && selectedDomainName != null)
                            continue;
                        */
                        DELEGATION_TYPE type = DELEGATION_TYPE.TYPE_CONSTRAINED;
                        string avatarName = "./Images/user_blue_cut.png";
                        string accntSenseColor = "LightGray";
                        string legKey = "User Account";

                        if (fields[startIndex + 1] == "Computer")
                        {
                            avatarName = "./Images/computer.png";
                            legKey = "Computer Account";
                        }
                        if (fields[startIndex + 2].ToUpper().Contains("UNCONSTRAINED"))
                        {
                            accntSenseColor = "#fee7ea"; // crimson  "#FF8080";
                                                         //avatarName = "./Images/unmanaged-priveleged-user-noback.png";
                                                         //avatarName = "./Images/ic-user-red-reject.png";

                            //legKey = "Unconstrained User";
                            legKey = "User Account";
                            type = DELEGATION_TYPE.TYPE_UNCONSTRAINED;
                            if (fields[startIndex + 1] == "Computer")
                            {
                                //avatarName = "./images/computer with alert.png";
                                //legKey = "Unconstrained Computer";
                                legKey = "Computer Account";
                            }
                        }
                        else if (fields[startIndex + 2].ToUpper().Contains("PROTOCOL"))
                        {
                            accntSenseColor = "#ff9c00";    // CU3 gold
                            type = DELEGATION_TYPE.TYPE_PROTOCOL_TRANSITION;
                            //avatarName = "./Images/ic-user-red-reject.png";
                            //legKey = "Protocol Transition User";
                            legKey = "User Account";

                            if (fields[startIndex + 1] == "Computer")
                            {
                                //avatarName = "./images/computer with alert.png";
                                //legKey = "Protocol Transition Computer";
                                legKey = "Computer Account";
                            }
                        }
                        /*
                                            if (fields[startIndex + 1] == "Computer")
                                            {
                                                legKey = "Machine Accounts";
                                                accntSenseColor = "LightGray";
                                                avatarName = "./Images/Operating-system.png";
                                            }
                                            */
                        // check if this account is already in dictionary
                        if (!accountDict.ContainsKey(fields[0 + startIndex]))
                        {
                            var a = new Person(graph, Form.ViewModel)
                            {
                                Name = fields[0 + startIndex],
                                ShowName = accountName,
                                Avatar = avatarName,
                                BackColor = accntSenseColor,
                                Domain = domainname,
                                Importance = 0,
                                LegendKey = legKey
                            };
                            List<Person> accounts = new List<Person>();
                            accounts.Add(a);
                            accountDict.Add(fields[0 + startIndex], accounts);
                        }

                        // next read the services and connect to user account names
                        string spns = fields[startIndex + 3];
                        string[] spnnames = spns.Split(',');

                        string showServices = "", add = "";
                        int count = 0;
                        foreach (string srvv in spnnames)
                        {
                            count++;
                            if (count < 4)
                                showServices += srvv + "\n";
                        }
                        if (count > 3)
                        {
                            showServices = showServices.Substring(0, showServices.Length - 2);
                            showServices += "...\n";
                            //showServices += "\n\n<bold>And " + (count - 4).ToString() + " more</bold>";
                            domainname = "And " + (count - 4).ToString() + " more";
                        }

                        Person accnt = accountDict[fields[0 + startIndex]][0];
                        // new account here
                        if (!servicesDict.ContainsKey(accnt))
                        {
                            List<delegationServices> lista = new List<delegationServices>();
                            string spnNames = "";
                            foreach (string n in spnnames)
                            {
                                spnNames += n + "\n";
                            }
                            delegationServices dele = new delegationServices();
                            //Person serviceP;
                            Machine serviceM;
                            // if such a service already exists, don't recreate a Person for it...
                            if (serviceNames.ContainsKey(spnNames))
                                //serviceP = serviceNames[spnNames];
                                serviceM = serviceNames[spnNames];
                            else
                            {
#if zero
                            serviceP = new Person(graph, Form.ViewModel) { Name = spnNames, ShowName = showServices, Avatar = "./Avatars/Gears.png", BackColor = "LightGreen", Domain = domainname, Importance = 0, LegendKey = "Services" };
                            if (serviceLegedName == null) serviceLegedName = serviceP.Name;
                            serviceNames.Add(spnNames, serviceP);

                            serviceP.toolTipText = spnNames;
                            serviceP.toolTipHeader = "Services List:";
                            //serviceP.toolTipImage = "c:\\Users\\nimrod\\Documents\\Visual Studio 2015\\Projects\\graphviz4net_b19bb0cdc8c6\\src\\Graphviz4Net.WPF.Example\\Avatars\\Gears.png";
                            serviceP.toolTipImage = "./Avatars/Gears.png";
                            serviceP.toolTipEnabled = "Visible";
                            serviceP.emailWeight = "Bold";
#else

                                string newNames = "";
                                if (spnnames.Length == 1)
                                {
                                    newNames = spnnames[0];
                                    if (newNames == "")               // if no spn  name in list..,
                                        newNames = "Service Not Available";
                                }
                                else
                                {
                                    int[] srv = new int[5];
                                    string[] specialServices = { "ldap", "cifs", "sql", "http", "exchange" };
                                    foreach (string s in spnnames)
                                    {
                                        int o = 0;
                                        foreach (string sq in specialServices)
                                        {
                                            if (s.ToLower().Contains(sq))
                                                srv[o]++;
                                            o++;
                                        }
                                    } // endfor each
                                    for (int i = 0; i < specialServices.Length; i++)
                                    {
                                        if (srv[i] != 0)
                                            newNames += srv[i].ToString() + " " + specialServices[i] + "\n";
                                    } // endfor


                                    // if NO voulnerable services -- show Service Not Available
                                    if (spnnames.Length == 0)
                                        newNames = "Service Not Available";

                                }


                                serviceM = new Machine(graph, Form.ViewModel)
                                {
                                    Name = spnNames,
                                    Avatar = "./Avatars/Gears.png",
                                    BackColor = "LightGreen",
                                    ShowNameVisibile = "Visible",
                                    NameVisible = "Hidden",
                                    LegendKey = "Services",
                                    showName = newNames

                                };
                                if (newNames == "Service Not Available")
                                    serviceM.Avatar = "./Avatars/gears-inactive.png";

                                serviceM.toolTipText = spnNames;
                                serviceM.toolTipHeader = "Services List:";
                                //serviceP.toolTipImage = "c:\\Users\\nimrod\\Documents\\Visual Studio 2015\\Projects\\graphviz4net_b19bb0cdc8c6\\src\\Graphviz4Net.WPF.Example\\Avatars\\Gears.png";
                                serviceM.toolTipImage = "./Avatars/Gears.png";
                                serviceM.toolTipEnabled = "Hidden";//"Visible";
                                                                   //serviceM.emailWeight = "Bold";

#endif
                                if (serviceLegedName == null) serviceLegedName = serviceM.Name;
                                serviceNames.Add(spnNames, serviceM);
                            }
                            dele.srv = /*serviceP;*/serviceM;
                            dele.type = type;
                            dele.serviceName = spnNames;
                            lista.Add(dele);
                            servicesDict.Add(accnt, lista);
                        } // new account
                        else
                        {
                            string spnNames = "";
                            foreach (string _n in spnnames)
                            {
                                spnNames += _n + "\n";
                            }

                            List<delegationServices> lista = servicesDict[accnt];
                            foreach (string n in spnnames)
                            {
                                bool found = false;
                                foreach (delegationServices ddd in lista)
                                {
                                    if (ddd.serviceName == spnNames /*&& ddd.type == type*/)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    delegationServices dele = new delegationServices();

                                    Person serviceP;
                                    Machine serviceM;
                                    // if such a service already exists, don't recreate a Person for it...
                                    if (serviceNames.ContainsKey(spnNames))
                                    {
                                        serviceM = serviceNames[spnNames];
                                        //serviceP = serviceNames[spnNames];
                                    }
                                    else
                                    {
#if zero
                                    serviceP = new Person( graph, Form.ViewModel ) { Name = spnNames, ShowName = showServices, Avatar = "./Avatars/gears.png", BackColor = "LightGreen", Domain = domainname, Importance = 0, LegendKey = "Services" };
                                    if( serviceLegedName == null ) serviceLegedName = serviceP.Name;
                                    serviceNames.Add( spnNames, serviceP );

                                    serviceP.toolTipText = spnNames;
                                    serviceP.toolTipHeader = "Services List:";
                                    serviceP.toolTipImage = "./Avatars/Gears.png";
                                    serviceP.toolTipEnabled = "Visible";
                                    serviceP.emailWeight = "Bold";
#else
                                        serviceM = new Machine(graph, Form.ViewModel)
                                        {
                                            Name = spnNames,
                                            Avatar = "./Avatars/Gears.png",
                                            BackColor = "LightGreen",
                                            LegendKey = "Services",
                                            ShowNameVisibile = "Visible",
                                            NameVisible = "Hidden",
                                            showName = count.ToString() + " Service(s)"

                                        };
                                        serviceM.toolTipText = spnNames;
                                        serviceM.toolTipHeader = "Services List:";
                                        //serviceP.toolTipImage = "c:\\Users\\nimrod\\Documents\\Visual Studio 2015\\Projects\\graphviz4net_b19bb0cdc8c6\\src\\Graphviz4Net.WPF.Example\\Avatars\\Gears.png";
                                        serviceM.toolTipImage = "./Avatars/Gears.png";
                                        serviceM.toolTipEnabled = "Hidden";
                                        if (serviceLegedName == null) serviceLegedName = serviceM.Name;
                                        serviceNames.Add(spnNames, serviceM);
#endif
                                    }
                                    dele.srv = /*serviceP*/serviceM;
                                    dele.type = type;
                                    dele.serviceName = spnNames;
                                    lista.Add(dele);
                                }
                                // if found first service... dont continue just move to next csv line
                                else break;

                            } // endforeach
                        } // endif
                    } // endwhile parser
                }
            } // endtry
            catch( Exception ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }

            Dictionary<string, /*Person*/Machine> onGraphserviceNames = new Dictionary<string, Machine>();

            // now plug everything to the graph
            foreach (string key in accountDict.Keys)
            {
                // (1) place the account as a vertex
                subGraph.AddVertex(accountDict[key][0]);

                List<delegationServices> lista = servicesDict[accountDict[key][0]];
                foreach (delegationServices dele in lista)
                {
                    if( !onGraphserviceNames.ContainsKey(dele.serviceName))
                    {
                        onGraphserviceNames.Add(dele.serviceName, dele.srv);
                        subGraph.AddVertex(dele.srv);
                    }
                    string strokeColor = "Black";
                    if (dele.type == DELEGATION_TYPE.TYPE_UNCONSTRAINED)
                        strokeColor = "Red";
                    else if (dele.type == DELEGATION_TYPE.TYPE_PROTOCOL_TRANSITION)
                        strokeColor = "#ff9c00";//"Gold";

                    graph.AddEdge(new Edge<INotifyPropertyChanged>(accountDict[key][0], dele.srv, new Arrow() { Stroke = strokeColor })
                    {
                        edgeColor = strokeColor,
                        edgeStrokeThickness = "1.5",
                        //Label = dele.type.ToString()
                        Label = (dele.type == DELEGATION_TYPE.TYPE_CONSTRAINED) ? "Type:Constrained" : (dele.type == DELEGATION_TYPE.TYPE_PROTOCOL_TRANSITION) ? "Type:Protocol Transition" : "Type:Unconstrained"
                    });
                } // endforeach
            } // endfor each

            // show legend... TESTING!
            if (Form.showLegend)
                Form.ViewModel.addLegend(graph, serviceLegedName);

            Form.GraphLayout.Graph = graph;
        } // endfunc Run

        static void haveNoData()
        {
            MainWindow _Form = Application.Current.Windows[0] as MainWindow;
            var _graph = new Graph<INotifyPropertyChanged>();
            var _subGraph = new SubGraph<INotifyPropertyChanged> { Label = "Mystique Deleat", borderColor = "Blue" };
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



        /// <summary>
        ///
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        static string trimSpnNames(string input)
        {
            string output = input;
            if (output.StartsWith(">"))
                output = output.Substring(1);
            if (output.EndsWith("\r<"))
                output = output.Substring(0, output.Length - 2);

            return output;
        }
    } // endclass
} // end namespace





