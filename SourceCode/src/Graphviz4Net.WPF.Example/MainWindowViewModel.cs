///////////////////////////////////////
// Make sure that group names and user names are more elaborate (e.g. andy.Users, Enterprise Admins.Users etc.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Threading;
using System.Windows;


namespace Graphviz4Net.WPF.Example
{
    using Graphs;
    using System.ComponentModel;
    using System.Windows.Media;
    using System.Windows.Threading;
    using System.Windows.Controls;
    using System.Xml;
    using System.Xml.Serialization;
    using Dot;
    using Graphviz4Net.Graphs;
    using System.Windows.Documents;
    using System.DirectoryServices.ActiveDirectory;
    using System.Windows.Media.Imaging;


    //    [DelimitedRecord(",")]
    public class ACL_Light_Record
    {
        public string Domain;
        public string AccountName;
        public string AccountGroup;
        public string ActiveDirectoryRights;
        public string ObjectRights;
        public string ObjectDN;
        public string ObjectOwner;
        public string ObjectClassCategory;
    }

    /****************************************************************/
    /*                  P E R S O N     C L A S S                   */
    /****************************************************************/
    public class Person : INotifyPropertyChanged
    {
        private readonly Graph<INotifyPropertyChanged> graph;
        public MainWindowViewModel modelView;

        public Person(Graph<INotifyPropertyChanged> graph, MainWindowViewModel view)
        {
            this.graph = graph;
            this.Avatar = "./Avatars/Desktop.png";
            this.ShowPictureImage = "./Images/ic-show-thumbnail.png";
            this.BackColor = "White";
            this.previousBackColor = "White";
            this.modelView = view;
            this.isDrillDownable = true;
            this.beenHere = false;
            this.Importance = 0;
            this.maxHeight = -1;
            this.isBoxVisible = "Hidden";
            this.toolTipText = null;
            this.toolTipHeader = null;
            this.toolTipEnabled = "Hidden";
            this.LegendKey = null;
            this.picPresent = "Hidden";
            this.thumbnail = null;

            if ( Name != null && view.securedDict.ContainsKey( Name + "@" + this.Domain))
            {
                this.checkBoxState = "true";
                this.isSecured = true;
            }
            else
            {
                this.checkBoxState = "false";
                this.isSecured = false;
            }

            this.emailWeight = "Normal";
        } // endof constructor of person

        public bool isDrillDownable { get; set; }
        public bool isSecured { get; set; }
        public string checkBoxState { get; set; }
        public string isBoxVisible { get; set; }
        public string picPresent { get; set; }
        private string name, showName;
        public string Domain;
        public bool beenHere;
        public string previousBackColor { get;  set; }
        public string LegendKey { get; set; }
        public int Importance { get; set; }

        public BitmapImage thumbnail;

        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                if (this.PropertyChanged != null) {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public string ShowName
        {
            get { return this.showName; }
            set
            {
                this.showName = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("ShowName"));
                }
            }
        }


        public string Avatar { get; set; }
        public string ShowPictureImage { get; set; }                    // magnifying glass... used to show that a picture is available for the user
        public string BackColor { get; set; }
        public int maxHeight { get; set; }
        public string toolTipHeader { get; set; }
        public string toolTipText { get; set; }
        public string toolTipImage { get; set; }
        public string toolTipEnabled { get; set; }
        public string Email
        {
            get
            {
                if( Domain == "Forest" )
                    return "";
                return /*this.Name.ToLower().Replace(' ', '.') +*/ "@" + Domain;
            }
        }
        public string emailWeight { get; set; }

        public ICommand RemoveCommand
        {
            get { return new RemoveCommandImpl(this); }
        }

        public ICommand DoubleClickCommand
        {
            get { return new DrillDownCommandImpl(this); }
        }
        public ICommand PersonClickCommand
        {
            get { return new showACLTipsCommandImpl( this ); }
        }

        public ICommand CheckboxClickedCommand
        {
            get { return new CheckboxClickedImpl(this); }
        }



        public ICommand MouseOverCard
        {
            get { return new MouseOverCommandImpl(this); }
        }

        private class RemoveCommandImpl : ICommand
        {
            private Person person;

            public RemoveCommandImpl(Person person)
            {
                this.person = person;
            }

            public void Execute(object parameter)
            {
                this.person.graph.RemoveVertexWithEdges(this.person);
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }

    	public event PropertyChangedEventHandler PropertyChanged;

        private class DrillDownCommandImpl : ICommand
        {
            private Person person;

            public DrillDownCommandImpl(Person person)
            {
                this.person = person;
            }

            public void Execute(object parameter)
            {
                // Is this the domains selection level
                if (person.Domain == "Forest" && person.modelView.onScreen == MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS)
                {
                    string selectedDomainName = person.name;
                    person.modelView.reformatCardsGraph( selectedDomainName, true);

                    // NS 23-1-2018 
                    MainWindow Form = Application.Current.Windows[0] as MainWindow;
                    Form.zoomControl.Mode = WPFExtensions.Controls.ZoomControlModes.Fill;
                }
                else if(person.Domain == "Forest" && person.modelView.onScreen == MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS_RISKY_SPN)
                {
                    riskySPNs.Run( person.name, person.modelView.RISKYSPNInputFile);
                    person.modelView.onScreen = MainWindowViewModel.ON_SCREEN.RISKYSPN_WITH_DOMAIN_SELECTION;
                    MainWindow Form = Application.Current.Windows[0] as MainWindow;
                    Form.backButton.Visibility = Visibility.Visible;
                }
                else
                {
                    person.modelView.drillDownFirstLevel(person);

                    // NS 23-1-2018 
                    MainWindow Form = Application.Current.Windows[0] as MainWindow;
                    Form.zoomControl.Mode = WPFExtensions.Controls.ZoomControlModes.Fill;
                }
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }


        private class showACLTipsCommandImpl : ICommand
        {
            private Person person;

            public showACLTipsCommandImpl(Person person)
            {
                this.person = person;
            }

            public void Execute(object parameter)
            {
                MainWindow Form = Application.Current.Windows[0] as MainWindow;
                if( Form.rtbACL.Visibility == Visibility.Visible )
                {
                    Form.rtbACL.Visibility = Visibility.Hidden;
                    return;
                }
                /*
                 * 
                 * 
                 * 
                 * 
                 * */
#if zero
                //! currently used by ACLight detailed display only
                if( person.modelView.onScreen != MainWindowViewModel.ON_SCREEN.ACLLIGHT_ON_SCREEN )
                    return;
                string curDir = Directory.GetCurrentDirectory();
                StreamReader sr = File.OpenText( String.Format( "{0}/../../ZBANG/ACLight Attack Path Update.html", curDir ) );
                Form.TextBox.Text = sr.ReadToEnd();
                sr.Close();
                Form.rtbACL.Visibility = Visibility.Visible;
#endif
                //Form.rtbEditor.Visibility = Visibility.Visible;

                //FileStream fs = new FileStream( "c:/temp/ACLight Attack Path Update.html", FileMode.Open );
                //Form.aclWebBrowser.NavigateToStream( fs);
                //Form.aclWebBrowser.Navigate( new Uri( String.Format( "file:///{0}/../../ZBANG/ACLight Attack Path Update.html", curDir )));

                /*
                // Is this the domains selection level
                if( person.Domain == "Forest" && person.modelView.onScreen == MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS )
                {
                    string selectedDomainName = person.name;
                    person.modelView.reformatCardsGraph( selectedDomainName, true );
                }
                else if( person.Domain == "Forest" && person.modelView.onScreen == MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS_RISKY_SPN )
                {
                    riskySPNs.Run( person.name, person.modelView.RISKYSPNInputFile );
                    person.modelView.onScreen = MainWindowViewModel.ON_SCREEN.RISKYSPN_WITH_DOMAIN_SELECTION;
                    MainWindow Form = Application.Current.Windows[0] as MainWindow;
                    Form.backButton.Visibility = Visibility.Visible;
                }
                else
                {
                    person.modelView.drillDownFirstLevel( person );
                }
                */
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        } // endclass




        private class MouseOverCommandImpl : ICommand
        {
            private Person person;

            public MouseOverCommandImpl(Person person)
            {
                this.person = person;
            }

            public void Execute(object parameter)
            {
                //this.person.graph.RemoveVertexWithEdges(this.person);
                person.modelView.highlightEdges(this.person);
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }

        private class CheckboxClickedImpl : ICommand
        {
            private Person person;

            public CheckboxClickedImpl(Person person)
            {
                this.person = person;
            }

            public void Execute(object parameter)
            {
                if (!person.isSecured)
                {
                    person.isSecured = true;
                    person.previousBackColor = person.BackColor;
                    person.BackColor = "LightGreen";
                    if( !person.LegendKey.Contains( "Managed"))
                        person.Avatar = "./Images/user_blue_cut.png";
                    person.checkBoxState = "true";
                    if( !person.modelView.securedDict.ContainsKey(person.Name + "@" + person.Domain))
                    {
                        person.modelView.securedDict.Add(person.Name + "@" + person.Domain, 1);
                    }
                    else
                    {
                        person.modelView.securedDict[person.Name + "@" + person.Domain] = 1;
                    }
                    
                }
                else
                {
                    person.isSecured = false;
                    person.BackColor = person.previousBackColor;
                    //person.Avatar = "./Images/ic-user-red-reject-noback.png";
                    // NS 31122017   ... person.Avatar = "./Images/unmanaged-priveleged-user-noback.png";
                    if (!person.LegendKey.Contains("Managed"))
                        person.Avatar = "./Images/ic-user-red-reject.png";


                    person.checkBoxState = "false";
                    if (!person.modelView.securedDict.ContainsKey(person.Name + "@" + person.Domain ) )
                    {
                        person.modelView.securedDict.Add(person.Name + "@" + person.Domain, 0);
                    }
                    else
                    {
                        person.modelView.securedDict[person.Name + "@" + person.Domain] = 0;
                    }
                }
                XmlSerializer serializer = new XmlSerializer(typeof(MainWindowViewModel.item[]), new XmlRootAttribute() { ElementName = "items" });
                FileStream fs;
                serializer.Serialize(fs = File.Open(person.modelView.ACLLightCache, FileMode.Open),
                                      person.modelView.securedDict.Select(kv => new MainWindowViewModel.item() { name = kv.Key, value = kv.Value }).ToArray());
                fs.Close();

                person.graph.UpdateGraphWithoutRereadingDot();
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }


    } // endclass Person

    /****************************************************************/
    /*            M A C H I N E    C L A S S                        */
    /****************************************************************/
    public class Machine : INotifyPropertyChanged
    {
        private readonly Graph<INotifyPropertyChanged> graph;
        private MainWindowViewModel modelView;

        public Machine(Graph<INotifyPropertyChanged> graph, MainWindowViewModel view)
        {
            this.graph = graph;
            this.Avatar = "./Avatars/avatarAnon.gif";
            this.BackColor = "White";
            this.modelView = view;
            this.LegendKey = null;
            this.ShowNameVisibile = "Hidden";
            this.NameVisible = "Visbile";

            this.toolTipText = null;
            this.toolTipHeader = null;
            this.toolTipEnabled = "Hidden";
        }

        private string name;
        public string BackColor { get; set; }
        public string realName { get; set; }
        public string LegendKey { get; set; }
        public string showName { get; set;}
        public bool shouldShowName { get; set; }
        public string NameVisible { get; set; }
        public string ShowNameVisibile { get; set; }
        public string toolTipHeader { get; set; }
        public string toolTipText { get; set; }
        public string toolTipImage { get; set; }
        public string toolTipEnabled { get; set; }


        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.realName = value;
                /*
                if(value != null && value.Contains("."))
                {
                    string temp = value.Split('.')[0];
                    int len = temp.Length;
                    this.name = temp.Substring(0, 7);
                }
                else
                */
                    this.name = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public string Avatar { get; set; }
        public string codeName { get; set; }

        public string Email
        {
            get
            {
                return /*this.Name.ToLower().Replace(' ', '.') +*/ "@gmail.com";
            }
        }

        public ICommand RemoveCommand
        {
            get { return new RemoveCommandImpl(this); }
        }
        public ICommand DoubleClickCommand
        {
            get { return new DrillDownCommandImpl(this); }
        }

        private class RemoveCommandImpl : ICommand
        {
            private Machine machine;

            public RemoveCommandImpl(Machine machine)
            {
                this.machine= machine;
            }

            public void Execute(object parameter)
            {
                this.machine.graph.RemoveVertexWithEdges(this.machine);
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }
        private class DrillDownCommandImpl : ICommand
        {
            private Machine machine;

            public DrillDownCommandImpl(Machine machine)
            {
                this.machine = machine;
            }

            public void Execute(object parameter)
            {
                //this.person.graph.RemoveVertexWithEdges(this.person);
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    } // endclass Machines


    public class DiamondArrow
    {
    }

    public class Arrow
    {
        private readonly IDictionary<string, string> attributes;
        public Arrow(
            IDictionary<string, string> attributes = null)
        {
            this.attributes = attributes ?? new Dictionary<string, string>();
        }
        public string Stroke
        {
            get { return this.Attributes.GetValue("ArrowStroke", "Black"); }
            set { this.Attributes["ArrowStroke"] = value; }
        }
        public string StrokeThickness
        {
            get { return this.Attributes.GetValue("StrokeThickness", "1"); }
            set { this.Attributes["StrokeThickness"] = value; }
        }
        public IDictionary<string, string> Attributes
        {
            get { return this.attributes; }
        }
    } // endclass Arrow


    /*
         _      ______ _____ ______ _   _ _____  
        | |    |  ____/ ____|  ____| \ | |  __ \ 
        | |    | |__ | |  __| |__  |  \| | |  | |
        | |    |  __|| | |_ |  __| | . ` | |  | |
        | |____| |___| |__| | |____| |\  | |__| |
        |______|______\_____|______|_| \_|_____/ 
    */
    /**
     * LEGEND CLASS 
     **/
    public class Legend : INotifyPropertyChanged
    {
        private readonly Graph<INotifyPropertyChanged> graph;

        public Legend(Graph<INotifyPropertyChanged> graph)
        {
            this.graph = graph;
            this.Avatar = "./Avatars/avatarAnon.gif";
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }
        }

        public string Avatar { get; set; }


        public ICommand RemoveCommand
        {
            get { return new RemoveCommandImpl(this); }
        }

        private class RemoveCommandImpl : ICommand
        {
            private Legend legend;

            public RemoveCommandImpl(Legend legend)
            {
                this.legend = legend;
            }

            public void Execute(object parameter)
            {
                this.legend.graph.RemoveVertexWithEdges(this.legend);
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    } // endclass legend





    public class BlindArrow
    {
    }


/*
    public class MyClass
    {
        static MyClass()
        {
            ResourceExtractor.ExtractResourceToFile( "Graphviz4Net.WPF.dll", "Graphviz4Net.WPF.dll" );
        }
    }

    public static class ResourceExtractor
    {
        public static void ExtractResourceToFile(string resourceName, string filename)
        {
            if( !System.IO.File.Exists( filename ) )
                using( System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream( resourceName ) )
                using( System.IO.FileStream fs = new System.IO.FileStream( filename, System.IO.FileMode.Create ) )
                {
                    byte[] b = new byte[s.Length];
                    s.Read( b, 0, b.Length );
                    fs.Write( b, 0, b.Length );
                }
        }
    }
*/

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // "c:\Users\nimrod\Documents\Visual Studio 2015\Projects\graphviz4net_b19bb0cdc8c6\src\ZbangGui\bin\version\ZBANG\ACLight-master\Results" 
        public string ACLLightInputFile     = "..\\..\\ZBANG\\ACLight-master\\Results\\Privileged Accounts - Final Report.csv";
        public string ACLLightCache         = "..\\..\\ZBANG\\acls.cache";
        public string RISKYSPNInputFile     = "..\\..\\ZBANG\\RiskySPN-master\\Results\\RiskySPNs-test.csv";
        public string mysticInputFile       = "..\\..\\ZBANG\\Mystique-master\\Results\\delegation_info.csv";
        //public string sidHistoryInputFile   = "..\\..\\ZBANG\\SIDHistory\\Results\\sidhistory-scan.csv";
        public string sidHistoryInputFile = "..\\..\\ZBANG\\SIDHistory\\Results\\Report.csv";
        public string skeletonFile          = "..\\..\\ZBANG\\SkeletonKey_Scanner\\Results\\SkeletonKeyResults.csv";


        // dictionary to serialize/deserialize the "IsSecured" status of ACL Light
        public class item
        {
            [XmlAttribute]
            public string name;
            [XmlAttribute]
            public int value;
        }
        public Dictionary<string, int> securedDict = new Dictionary<string, int>();



        public class objectDesignators
        {
            public string objectName;
            public int privileges;
            public bool beenHere = false;
            public string sid;
        }


        public Dictionary<string, int> ObjectRights = new Dictionary<string, int>();
        public Dictionary<string, int> PriviledgedUsers = new Dictionary<string, int>();
        public Dictionary<string, Person> nameToPerson = new Dictionary<string, Person>();
        public Dictionary<string, Person> nameToObject = new Dictionary<string, Person>();

        //public Dictionary<string, Machine> objectsDict = new Dictionary<string, Machine>();
        public Dictionary<string, Person> objectsDict = new Dictionary<string, Person>();
        public Dictionary<string, List<string>> userToObject = new Dictionary<string, List<string>>();
        public Dictionary<string, List<objectDesignators>> goingFromUsersToOjects = new Dictionary<string, List<objectDesignators>>();
        public Dictionary<string, string> shortPrivNames = new Dictionary<string, string>();
        public Dictionary<string, string> toolTipNames = new Dictionary<string, string>();

        public Dictionary<string, int> ACLightColumnFinder = new Dictionary<string, int>();

        public Dictionary<string, int> ACLightPrivilegedList = new Dictionary<string, int>();


        string myDomain;

        const int MAX_SHOWN_LETTERS = 100; //18;

        public ON_SCREEN onScreen;

        public enum ON_SCREEN
        {
            DOMAIN_CARDS,
            DOMAIN_CARDS_RISKY_SPN,
            CARDS_ON_SCREEN_WITH_DOMAIN_SELECTION,
            CARDS_ON_SCREEN_NO_DOMAIN_SELECTION,
            ACLLIGHT_ON_SCREEN,
            RISKYSPN_WITH_DOMAIN_SELECTION,
            RISKYSPN_ON_SCREEN,
            SKELETON_KEY_ON_SCREEN,
            SIDHISTORY_ON_SCREEN
        }
        int MAX_PERSON_HEIGHT_NO_CHECKBOX = 50;
        int MAX_PERSON_HEIGHT_WITH_CHECKBOX = 80;


        public MainWindowViewModel()
        {
            /*
             * 
             * CreateChild, DeleteChild, Self, WriteProperty, ExtendedRight, GenericRead, WriteDacl, WriteOwner
             * 
             */
            ObjectRights.Add("CreateChild", 1 << 0);
            ObjectRights.Add("DeleteChild", 1 << 1);
            ObjectRights.Add("Self", 1 << 2);
            ObjectRights.Add("WriteProperty", 1 << 3);
            ObjectRights.Add("GenericWrite", 1 << 4);
            ObjectRights.Add("ExtendedRight", 1 << 5);
            ObjectRights.Add("GenericRead", 1 << 6);
            ObjectRights.Add("WriteDacl", 1 << 7);
            ObjectRights.Add("WriteOwner", 1 << 8);
            ObjectRights.Add("GenericAll", 1 << 9);
            ObjectRights.Add("Delete", 1 << 10);
            ObjectRights.Add("ObjectOwner", 1 << 11);
            ObjectRights.Add("DeleteTree", 1 << 12);
            ObjectRights.Add("All", 1 << 13);
            ObjectRights.Add("User-Force-Change-Password", 1 << 14);
            ObjectRights.Add("Self-Membership", 1 << 15);
            ObjectRights.Add("ms-Exch-Dynamic-Distribution-List", 1 << 16);
            ObjectRights.Add("DS-Replication-Get-Changes-In-Filtered-Set", 1 << 17);
            ObjectRights.Add("DS-Replication-Get-Changes-All", 1 << 18);
            ObjectRights.Add("DS-Replication-Get-Changes", 1 << 19);
            ObjectRights.Add("Member-Of", 1 << 20);
            ObjectRights.Add("WriteMember", 1 << 21);
            ObjectRights.Add("WriteScript", 1 << 22);
            ObjectRights.Add("ReadProperty", 1 << 23);
            ObjectRights.Add("WritePropertyAll", 1 << 24);
            /*
permission filters inside ACLight:
1.	GenericAll -> GenericAll
2.	ExtendedRight + User-Force-Change-Password -> ChangePassword
3.	WriteDACL -> WriteDACL
4.	ObjectOwner -> Owner
5.	WriteOwner -> WriteOwner
6.	WriteProperty + Self-Membership -> WriteMembers ?? 
7.	GenericWrite -> GenericWrite
8.	WriteProperty + Script-Path -> WriteScript ??
9.	ExtendedRight + All -> ExtendedAll 
10.	ExtendedRight + (DS-Replication-Get-Changes\DS-Replication-Get-Changes-All\DS-Replication-Get-Changes-In-Filtered-Set) -> DCSync
             * 
             * 
             * */
            shortPrivNames.Add("CreateChild", "CrtChild");
            shortPrivNames.Add("DeleteChild", "DelChild");
            shortPrivNames.Add("Self", "Self");
            shortPrivNames.Add("WriteProperty", "WrtProp");
            shortPrivNames.Add("ExtendedRight", "Ext");
            shortPrivNames.Add("GenericRead", "Read");
            shortPrivNames.Add("GenericWrite", "GenericWrite");
            shortPrivNames.Add("WriteDacl", "WriteDACL");
            shortPrivNames.Add("WriteOwner", "WriteOwner");
            shortPrivNames.Add("GenericAll", "GenericAll");
            shortPrivNames.Add("Delete", "Del");
            shortPrivNames.Add("ObjectOwner", "Owner");
            shortPrivNames.Add("DeleteTree", "DelTree");
            shortPrivNames.Add("All", "ExtendedAll");
            shortPrivNames.Add("User-Force-Change-Password", "ResetPassword");
            shortPrivNames.Add("Self-Membership", "Member");
            shortPrivNames.Add("ms-Exch-Dynamic-Distribution-List", "Exch");
            shortPrivNames.Add("DS-Replication-Get-Changes-In-Filtered-Set", "DCsync");
            shortPrivNames.Add("DS-Replication-Get-Changes-All", "DCsync");
            shortPrivNames.Add("DS-Replication-Get-Changes", "DCsync");
            shortPrivNames.Add("Member-Of", "MemberOf");
            shortPrivNames.Add("WriteMember", "WriteMember");
            shortPrivNames.Add("WriteScript", "WriteScript");
            shortPrivNames.Add("ReadProperty", "Read");
            shortPrivNames.Add("WritePropertyAll", "WrtPropAll");





            MainWindow Form = Application.Current.Windows[0] as MainWindow;

            /*
            if (Form.Launch != null && Form.Launch.IsEnabled)
                reformatCardsGraph(false);
            */
        }



        /**
         * @brief       scans for domains and display the domains as card if present.
         * 
         * @returns     number of domains found. If more than 1 displays it. If==1 does not display cards at all
         **/ 
        public int scanACLoutputForDomains(string __ACLLightInputFile, string nameToShow)
        {
            string ccolor = "Gray";

            if (__ACLLightInputFile == null)
                __ACLLightInputFile = ACLLightInputFile;

            if( !File.Exists( __ACLLightInputFile))
            {
                MessageBox.Show( __ACLLightInputFile + " file not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
                return 0;
            }

            /**
             * IF ACL LIGHT IS RUN -- read the dictionary from disk and serialize it
             **/
            if( __ACLLightInputFile == ACLLightInputFile)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(item[]), new XmlRootAttribute() { ElementName = "items" });
                Object thisLock = new Object();
                FileStream fs = null;
                try
                {
                    //lock(thisLock)
                    {
                        securedDict = ((item[])serializer.Deserialize(fs = File.Open(ACLLightCache, FileMode.OpenOrCreate))).ToDictionary(i => i.name, i => i.value);
                        fs.Close();
                    }

                }
                catch
                {
                    if (fs != null)
                        fs.Close();
                }
                ccolor = "#b9cced";//"LightBlue";
            }
            if (__ACLLightInputFile == ACLLightInputFile)
                ACLightPrivilegedList.Clear();

            Dictionary<string, int> domainDict = new Dictionary<string, int>();
            var graph = new Graph<INotifyPropertyChanged>();

            lastSelectedDomain = null;
            string vertexName = null;

            var subGraph = new SubGraph<INotifyPropertyChanged> { Label = nameToShow, borderColor = ccolor /*"LightBlue"*/ };
            graph.AddSubGraph(subGraph);

            int lines = 0;
            using (TextFieldParser parser = new TextFieldParser(__ACLLightInputFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    lines++;
                    string[] fields = parser.ReadFields();
                    if( lines == 1 )
                    {
                        // first, find the indexes of all the columns by name
                        findACLightColumns( fields );
                        continue;
                    }
                    string domainColumnHeader = "Domain";
                    if( __ACLLightInputFile == ACLLightInputFile)
                    {
                        if( !ACLightColumnFinder.ContainsKey( domainColumnHeader ) )
                        {
                            MessageBox.Show( "Missing 'Domain' column in file. Are you using the correct ACLight version??", "Error", MessageBoxButton.OK , MessageBoxImage.Exclamation );
                            return 0;
                        }

                        //!!!! READ ALL PRIVILEGED SIDs INTO ACLightPrivilegedList
                        if (!ACLightPrivilegedList.ContainsKey(fields[ACLightColumnFinder["IdentitySID"]]))
                            ACLightPrivilegedList.Add(fields[ACLightColumnFinder["IdentitySID"]], 1);
                    }
                    else if( __ACLLightInputFile == this.RISKYSPNInputFile)
                    {
                        domainColumnHeader = "DomainName";
                    }
                    
                    int dominx = ACLightColumnFinder[domainColumnHeader];
                    string domainName = fields[dominx];
                    myDomain = domainName;
                    if (!domainDict.ContainsKey(myDomain))
                        domainDict.Add(myDomain, 1);
                    else
                        domainDict[myDomain]++;
                } // endwhile
            } // endusing
            if( domainDict.Count <= 1 )
            {
                if (domainDict.Count == 0) return 0; // MessageBox.Show( "No Data on File!", "Error", MessageBoxButton.OK, MessageBoxImage.Hand );
                return domainDict.Count;
            }

            foreach( string keys in domainDict.Keys)
            {
                //NS 2712 var a = new Person(graph, this) { Name = keys, ShowName = keys, Avatar = "./Avatars/avatar1.jpg", BackColor = "DeepSkyBlue", Domain = "Forest", LegendKey = "Discovered Domain" };
                var a = new Person(graph, this) { Name = keys, ShowName = keys, Avatar = "./Images/ic-domain.png", BackColor = /*"DeepSkyBlue"*/"#b9cced", Domain = "Forest", LegendKey = "Discovered Domain" };
                a.maxHeight = MAX_PERSON_HEIGHT_NO_CHECKBOX;

                if (vertexName == null)
                    vertexName = a.Name;
                
                subGraph.AddVertex( a);
            } // endfor each
            // *******************
            // *** L E G E N D ***
            // *******************
            MainWindow Form = Application.Current.Windows[0] as MainWindow;
            if (!Form.showLegend && __ACLLightInputFile == ACLLightInputFile)
                addLegend(graph, vertexName);
            else if(Form.showLegend)
                addLegend(graph, vertexName);

            this.Graph = graph;
            this.Graph.Changed += GraphChanged;
            Form.GraphLayout.Graph = graph;

            if (domainDict.Count < 6)
            {
                //Form.zoomControl.MaxZoom = 3.0;
                //Form.zoomControl.Mode = WPFExtensions.Controls.ZoomControlModes.Fill;
                //Form.zoomControl.Mode = WPFExtensions.Controls.ZoomControlModes.Custom;
                //Form.zoomControl.Zoom = 3.0;
            }

            return domainDict.Count;
        } // end of scan for domains


        public string lastSelectedDomain = null, legendStartName = null;

        /**
         * @brief       This function reads the output file of ACLLight and displays all 
         *              users in a cards-like display
         *              
         * @input        selectedDomainName - either the domain name or null for all domains on file
         *               needsRefresh - true if should refresh the graph
         *               
         **/
        public void reformatCardsGraph(string selectedDomainName, bool needsRefresh)
        {
            var graph = new Graph<INotifyPropertyChanged>();

            if(( selectedDomainName == null && lastSelectedDomain != null) || selectedDomainName != null)
            {
                if( selectedDomainName == null)
                    selectedDomainName = lastSelectedDomain;
                onScreen = ON_SCREEN.CARDS_ON_SCREEN_WITH_DOMAIN_SELECTION;
            }
            else 
                onScreen = ON_SCREEN.CARDS_ON_SCREEN_NO_DOMAIN_SELECTION;

            objectsDict.Clear();
            PriviledgedUsers.Clear();
            nameToPerson.Clear();
            goingFromUsersToOjects.Clear();

            List<string> Objects = new List<string>();

            int objectnumber = 0, removeobj = 0;

            var subGraphGroup = new SubGraph<INotifyPropertyChanged> { Label = "Privileged via Groups", borderColor = "DarkGray" };
            graph.AddSubGraph( subGraphGroup );
            var subGraphShadow = new SubGraph<INotifyPropertyChanged> { Label = "Shadow Admins", borderColor = "Red" };
            graph.AddSubGraph( subGraphShadow );

            //=======================================================================================
            // show progress bar
            /*
            ((MainWindow)Application.Current.Windows[0]).Progress.Visibility = Visibility.Visible;
            ((MainWindow)Application.Current.Windows[0]).Progress.Value = 0;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            worker.RunWorkerAsync();
            */
            ((MainWindow)Application.Current.Windows[0]).PleaseWaitText.Visibility = Visibility.Visible;
            //=======================================================================================

            if (selectedDomainName == null)
                selectedDomainName = lastSelectedDomain;
            else
                lastSelectedDomain = selectedDomainName;


            if( !File.Exists( ACLLightInputFile))
            {
                MessageBox.Show( ACLLightInputFile + " File Name Not Found", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
                return;
            }

            DateTime start = DateTime.Now;
            //    using (TextFieldParser parser = new TextFieldParser("..\\..\\ACLight\\Privileged Accounts - Final Report.csv"))

            try
            {
                int inx = 0;
                using (TextFieldParser parser = new TextFieldParser(ACLLightInputFile))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        inx++;
                        string[] fields = parser.ReadFields();
                        if (inx == 1)
                        {
                            findACLightColumns(fields);
                            continue;
                        }
                        if (!ACLightColumnFinder.ContainsKey("Domain"))
                        {
                            return;
                        }

                        string domainName = fields[ACLightColumnFinder["Domain"]];
                        if (selectedDomainName != null && domainName != selectedDomainName)
                            continue;

                        myDomain = domainName;

                        /* NS 14/12/17
                        if (subGraph.Label != domainName)
                            subGraph.Label = domainName;
                        */
                        string name = fields[ACLightColumnFinder["AccountName"]];   //fields[2];                                // ACCOUNT NAME

                        // delete the domain name from the name???
                        if (name.Contains("\\"))
                        {
                            if (name.ToLower().Contains(myDomain.ToLower().Split('.')[0]))
                                name = name.Split('\\')[1];
                        }

                        ////////////////////////////////////
                        //      P R I V I L E G E S       //
                        ////////////////////////////////////
                        string perms = fields[ACLightColumnFinder["ActiveDirectoryRights"]];  // fields[4];
                        string[] individualPerms = perms.Split(',');


                        int accname = ACLightColumnFinder["AccountName"];
                        int accgrp = ACLightColumnFinder["AccountGroup"];


                        int ind = 0, foundind = 0;

                        // ****************************************************************************
                        // scan through all privs and see list by Asaf
                        // NOTE: second parameter is what's actual written in the csv file [col 5]
                        // ****************************************************************************
                        foreach (string _prv in individualPerms)
                        {
                            string prv = _prv.Trim();
                            int prvinx = ACLightColumnFinder["ObjectRights"];
                            if (prv == "WriteProperty" && (fields[prvinx/*5*/].Contains("Self-Membership") || fields[prvinx/*5*/].Contains("SelfMembership")))
                            {
                                ind = 1 << 21;
                                foundind = 1;
                                break;
                            }
                            if (prv == "WriteProperty" && fields[prvinx/*5*/].Contains("Script-Path"))
                            {
                                ind = 1 << 22;
                                foundind = 1;
                                break;
                            }
                            //if (prv == "WriteProperty" && fields[prvinx].Contains("All"))
                            //{
                            //    ind = 1 << 24;
                            //    foundind = 1;
                            //    break;
                            //}
                            if (prv == "ExtendedRight" && fields[prvinx].Contains("User-Force-Change-Password"))
                            {
                                ind = 1 << 14;
                                foundind = 1;
                                break;
                            }
                            if (prv == "ExtendedRight" && fields[prvinx].Contains("Replication"))
                            {
                                ind = 1 << 17;
                                foundind = 1;
                                break;
                            }
                            if (prv == "ExtendedRight" && fields[prvinx].Contains("All"))
                            {
                                ind = 1 << 13;
                                foundind = 1;
                                break;
                            }
                            if (prv == "WriteDacl")
                            {
                                if ((ind & 0xFFFFFFDEEF) != 0)
                                    ind = 0;
                                ind |= 1 << 13;
                                foundind = 1;
                            }
                            if (prv == "GenericWrite")
                            {
                                if ((ind & 0xFFFFFFDEEF) != 0)
                                    ind = 0;
                                ind |= 1 << 4;
                                foundind = 1;
                            }
                            if (prv == "WriteOwner")
                            {
                                if ((ind & 0xFFFFFFDEEF) != 0)
                                    ind = 0;
                                ind |= 1 << 8;
                                foundind = 1;
                            }
                        } // endfor each

                        if (foundind == 0)
                        {
                            for (int i = 0; i < individualPerms.Length; i++)
                            {
                                ind |= ObjectRights[individualPerms[i].Trim()];
                            } // endfor all perms
                        }
                        // remove the domain name
                        if (name.Contains('\\'))
                            name = name.Split('\\')[1];


                        if (!PriviledgedUsers.ContainsKey(name))
                        {
                            /*if (PriviledgedUsers.Count == 1)
                                break;
                                */

                            string showname = name.Substring(0, name.Length > MAX_SHOWN_LETTERS ? MAX_SHOWN_LETTERS : name.Length);

                            // if GROUP
                            string _BackColor = "#fee7ea"; // crimson  "#FF8080";
                            int importantAccount = 1000;
                            if (fields[accname /*2*/] != fields[accgrp /*3*/])
                            {
                                _BackColor = "#D0D0D0";
                                //ind = 1 << 20;
                                importantAccount = 0;
                            }
                            else
                                showname += ""; // ".Users";

                            PriviledgedUsers.Add(name, ind);
                            string ava = "./Images/user_blue_cut.png";
                            string legkey = "Managed Privileged User";
                            if (importantAccount != 0)
                            {
                                //ava = "./Images/unmanaged-priveleged-user-noback.png";
                                ava = "./Images/ic-user-red-reject.png";
                                legkey = "Unmanaged Privileged User";
                            }


                            //var a = new Person(graph, this) { Name = name, ShowName = showname, Avatar = "./Avatars/avatar1.jpg", BackColor = _BackColor, Domain = myDomain, Importance = importantAccount };
                            var a = new Person(graph, this)
                            {
                                Name = name,
                                ShowName = showname,
                                Avatar = /*"./Avatars/avatar1.jpg"*/ava,
                                BackColor = _BackColor,
                                Domain = myDomain,
                                Importance = importantAccount,
                                maxHeight = 80,
                                isBoxVisible = "Visible",
                                LegendKey = legkey

                            };

                            // NS 02/01/2018 ....   GET THE IMAGE OUT
                            try
                            {
                                if (fields[12].Length > 1)
                                {
                                    a.thumbnail = ConvertStringtoImage(fields[12]);
                                    a.picPresent = "Visible";
                                }
                            }
                            catch
                            {
                                a.thumbnail = null;
                            }

                            // check the secured dictionary
                            if (securedDict.ContainsKey(a.Name + "@" + myDomain))
                            {
                                if (securedDict[a.Name + "@" + myDomain] == 1)
                                {
                                    a.checkBoxState = "true";
                                    a.previousBackColor = a.BackColor;
                                    a.BackColor = "LightGreen";
                                    a.isSecured = true;
                                }
                            }

                            if (!nameToPerson.ContainsKey(name))
                            {
                                nameToPerson.Add(name, a);
                                //
                                // NOT IMPORTANT ?
                                if (a.Importance == 0)
                                    subGraphGroup.AddVertex(a);
                                else
                                    subGraphShadow.AddVertex(a);
                            }
                            else
                                throw (new Exception("probably same objects per user!"));
                        }
                        else
                        {
                            if (fields[accname /*2*/] != fields[accgrp /*3*/])
                            {
                                //ind = 1 << 20;
                                PriviledgedUsers[name] = ind;
                            }
                            else
                                PriviledgedUsers[name] |= ind;
                        }




                        // read the Group column. If != user name ---> add another person card to the dictionary with the group's name
                        string groupNames = fields[accgrp /*3*/];
                        // delete the domain name from the name???
                        if (groupNames.Contains("\\") && !groupNames.StartsWith("BUILTIN"))
                        {
                            if (groupNames.ToLower().Contains(myDomain.ToLower().Split('.')[0]))
                                groupNames = groupNames.Split('\\')[1];
                        }
                        // if name is a REAL group name
                        bool isAttachedToGroup = false;
                        if (groupNames != name)
                        {
                            isAttachedToGroup = true;

                            if (!PriviledgedUsers.ContainsKey(groupNames))
                            {
                                PriviledgedUsers.Add(groupNames, ind);
                                string showname = groupNames.Substring(0, groupNames.Length > MAX_SHOWN_LETTERS ? MAX_SHOWN_LETTERS : groupNames.Length);
                                var a = new Person(graph, this)
                                {
                                    Name = groupNames,
                                    ShowName = showname,
                                    //Avatar = "./Avatars/avatar2.gif",
                                    //Avatar = "./Images/ic-group-blue-noback.png",
                                    Avatar = "./Images/new-group.png",      // group cut by Asaf 31/12/2017
                                    BackColor = "#b6b8b1", //"#808080",     // group gray
                                    Domain = myDomain,
                                    isDrillDownable = false,
                                    isBoxVisible = "Visible",
                                    LegendKey = "Group"
                                };
                                if (!nameToPerson.ContainsKey(groupNames))
                                {
                                    nameToPerson.Add(groupNames, a);
                                    removeobj++;
                                    // don't want to show these on screen at this point... subGraph.AddVertex(a);
                                }
                                else
                                    throw (new Exception("probably same group objects per user!"));
                            }
                        }
                        else
                        {
                            groupNames = name /*+ ".Users"*/;
                        }

                        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-
                        // read the objects column and place in namesToPerson
                        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-
                        int objdn = ACLightColumnFinder["ObjectDN"];
                        string[] computerNames = fields[objdn/*6*/].Split(',');                  // OBJECTS LIST ---> "COMPUTERS"
                        string cName = computerNames[0].Substring(3) /*+ "." + computerNames[1].Substring(3)*/;
                        if (!nameToObject.ContainsKey(cName) && !nameToPerson.ContainsKey(cName))
                        {
                            string showname = computerNames[0].Substring(3) + "." + computerNames[1].Substring(3);
                            var a = new Person(graph, this)
                            {
                                Name = cName,
                                ShowName = showname,
                                //Avatar = "./Avatars/avatar1.jpg",
                                Avatar = "./Images/new-group.png",      // group cut by Asaf 31/12/2017 NS 14/06
                                BackColor = "#A0A0A0",
                                Domain = myDomain,
                                isBoxVisible = "Visible",
                                LegendKey = "Managed Privileged User"
                            };
                            // NS 02/01/2018 ....   GET THE IMAGE OUT
                            try
                            {
                                if (fields[12].Length > 1)
                                {
                                    a.thumbnail = ConvertStringtoImage(fields[12]);
                                    a.picPresent = "Visible";
                                }
                            }
                            catch
                            {
                                a.thumbnail = null;
                            }

                            nameToObject.Add(cName, a);
                        }



                        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-==-=-=-=-=-=-

                        // if have a group here... add an edge from name-card to group and from group to object
                        if (isAttachedToGroup)
                        {
                            string objSID = fields[ACLightColumnFinder["ObjectSID"]];
                            string idSID = fields[ACLightColumnFinder["IdentitySID"]];
                            string objectAccnt = "";
                            try
                            {
                                objectAccnt = fields[ACLightColumnFinder["ObjectAccount"]];
                                if (objectAccnt.StartsWith("S-1-5"))
                                    objectAccnt = "";
                                else if (objectAccnt.Contains('\\') && !objectAccnt.StartsWith("BUILTIN"))
                                    objectAccnt = objectAccnt.Split('\\')[1];
                                if (objectAccnt != "")
                                    //groupNames = objectAccnt;
                                    cName = objectAccnt;
                            }
                            catch { } 

                            addNewEdge(name, idSID, groupNames, objSID, (1 << 20) /*ind*/);
                            addNewEdge(groupNames, idSID, cName, objSID, ind);
                        }
                        else // !group
                        {
                            string objSID = fields[ACLightColumnFinder["ObjectSID"]];
                            string idSID = fields[ACLightColumnFinder["IdentitySID"]];
                            string objectAccnt = "";
                            try
                            {
                                objectAccnt = fields[ACLightColumnFinder["ObjectAccount"]];
                                if (objectAccnt.StartsWith("S-1-5"))
                                    objectAccnt = "";
                                else if (objectAccnt.Contains('\\') && !objectAccnt.StartsWith("BUILTIN"))
                                    objectAccnt = objectAccnt.Split('\\')[1];

                                if (objectAccnt != "")
                                    cName = objectAccnt;
                            }
                            catch { }

                            addNewEdge(name, idSID, cName, objSID, ind);
                        }
                        ind = 0;
#if zero
                    if (!goingFromUsersToOjects.ContainsKey( name))
                    {
                        if (isGroup)
                        {
                            if (!goingFromUsersToOjects.ContainsKey(name))
                            {
                                // make an edge from the user to the group and then from the group to the object
                                List<objectDesignators> objectNamesPerUser = new List<objectDesignators>();
                                objectDesignators od1 = new objectDesignators();
                                od1.objectName = groupNames;
                                od1.privileges = ind;
                                objectNamesPerUser.Add(od1);
                                goingFromUsersToOjects.Add(name, objectNamesPerUser);
                            }
                            else
                            {
                                objectDesignators od = new objectDesignators();
                                od.objectName = groupNames;          // object name
                                od.privileges = ind;
                                List<objectDesignators> _objectNamesPerUser;
                                _objectNamesPerUser = goingFromUsersToOjects[name];
                                _objectNamesPerUser.Add(od);
                            }
                        }
                        // add the user name
                        if (!goingFromUsersToOjects.ContainsKey(cName))
                        {
                            List<objectDesignators> objectNamesPerUser = new List<objectDesignators>();
                            objectDesignators od = new objectDesignators();
                            od.objectName = cName;
                            od.privileges = ind;
                            objectNamesPerUser.Add(od);
                            goingFromUsersToOjects.Add(cName, objectNamesPerUser);
                        }
                        else
                        {
                            objectDesignators od = new objectDesignators();
                            od.objectName = cName;          // object name
                            od.privileges = ind;
                            List<objectDesignators> _objectNamesPerUser;
                            _objectNamesPerUser = goingFromUsersToOjects[cName];
                            _objectNamesPerUser.Add(od);
                        }
                        ind = 0;
                    }
                    else
                    {
                        if( isGroup)
                        {
                            if (!goingFromUsersToOjects.ContainsKey(name))
                            {
                                List<objectDesignators> objectNamesPerUser = new List<objectDesignators>();
                                objectDesignators od1 = new objectDesignators();
                                od1.objectName = groupNames;
                                od1.privileges = ind;
                                objectNamesPerUser.Add(od1);
                                goingFromUsersToOjects.Add(name, objectNamesPerUser);
                            }
                            else
                            {
                                objectDesignators od = new objectDesignators();
                                od.objectName = groupNames;          // object name
                                od.privileges = ind;
                                List<objectDesignators> _objectNamesPerUser;
                                _objectNamesPerUser = goingFromUsersToOjects[name];
                                _objectNamesPerUser.Add(od);
                            }
                        }

                        if (!goingFromUsersToOjects.ContainsKey(cName))
                        {
                            objectDesignators od = new objectDesignators();
                            od.objectName = cName;          // object name
                            od.privileges = ind;
                            List<objectDesignators> objectNamesPerUser;
                            objectNamesPerUser = goingFromUsersToOjects[groupNames];
                            objectNamesPerUser.Add(od);
                        }
                        else
                        {
                            objectDesignators od = new objectDesignators();
                            od.objectName = cName;          // object name
                            od.privileges = ind;
                            List<objectDesignators> _objectNamesPerUser;
                            _objectNamesPerUser = goingFromUsersToOjects[cName];
                            _objectNamesPerUser.Add(od);
                        }
                    }

                    if (!objectsDict.ContainsKey(cName))
                    {
                        if (computerNames[0].StartsWith("CN=") && computerNames[1].StartsWith("CN="))
                        {
                            if (computerNames[1].Substring(3).ToLower() == "users")
                            {
                                if (!objectsDict.ContainsKey(computerNames[0].Substring(3) + "." + computerNames[1].Substring(3)))
                                    objectsDict.Add(computerNames[0].Substring(3) + "." + computerNames[1].Substring(3), null);
                                continue;
                            }
                        }
                        // NS 0312 changed back to people.... var b = new Machine(graph, this) { Name = /*objectnumber.ToString()*/computerNames[0].Substring(3) + "." + computerNames[1].Substring(3)/*fields[5]*/, codeName = fields[5] };

                        string ggggg = computerNames[0].Substring(3) + "." + computerNames[1].Substring(3);
                        string showname = (ggggg).Substring(0, ggggg.Length > MAX_SHOWN_LETTERS ? MAX_SHOWN_LETTERS : ggggg.Length);

                        var b = new Person(graph, this) { Name = computerNames[0].Substring(3) + "." + computerNames[1].Substring(3), ShowName = showname, Avatar = "./Avatars/avatar1.jpg", BackColor = "LightYellow", Domain = "" };

                        objectnumber++;
                        if (!objectsDict.ContainsKey(computerNames[0].Substring(3) + "." + computerNames[1].Substring(3)))
                            objectsDict.Add(computerNames[0].Substring(3) + "." + computerNames[1].Substring(3), b);
                        //subGraph.AddVertex( b);
                    }
#endif
                        //graph.AddEdge(new Edge<INotifyPropertyChanged>(nameToPerson[name], objectsDict[fields[5]], new Arrow()) { Label = "A" });
                    } // endwhile
                } // endusing
            } // end try
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }

            // ((MainWindow)Application.Current.Windows[0]).Progress.Visibility = Visibility.Hidden;


            double end2 = (DateTime.Now - start).TotalMilliseconds;



            /////////////////////////////////////////////////////////////////////////////////////
            //                      S E T    C A R D S   I N    V I E W 
            /////////////////////////////////////////////////////////////////////////////////////
            // ADD BLIND EDGES TO SPLIT GROUP TO TABLE - UPTO 4 CARDS in A ROW

            fixCardInSubgraph( graph, subGraphGroup, removeobj );
            fixCardInSubgraph( graph, subGraphShadow, 0 );




            this.Graph = graph;
            this.Graph.Changed += GraphChanged;

            headPerson = null;
            drillGroup = null;


            MainWindow Form = Application.Current.Windows[0] as MainWindow;

            Form.zoomControl.MaxZoom = 2.5;
            if (needsRefresh)
            {
                graph.UpdateGraphRereadingDot();
                Form.GraphLayout.Graph = graph;
            }
            if (onScreen == ON_SCREEN.CARDS_ON_SCREEN_NO_DOMAIN_SELECTION)
                Form.backButton.Visibility = Visibility.Hidden;
            else
                Form.backButton.Visibility = Visibility.Visible;



            double end = (DateTime.Now - start).TotalMilliseconds;

            ((MainWindow)Application.Current.Windows[0]).PleaseWaitText.Visibility = Visibility.Hidden;

        } // endfunc


        /**
         * @brief   This function fixes edges between cards in such a way that they would seem 
         *          organized in a list of card
         *          
         **/
        void fixCardInSubgraph( Graph<INotifyPropertyChanged> grf, SubGraph<INotifyPropertyChanged>sgrf, int removeobj)
        {
            int tickets = sgrf.Vertices.Count();//nameToPerson.Count - removeobj;
            int TICKETS_PER_COLUMN = 4;
            int percolumn = tickets / TICKETS_PER_COLUMN;
            if( percolumn > 0 && tickets > 0)
            {
                List<Person> persons = new List<Person>();
                tickets = 0;
                foreach( var vertex in sgrf.Vertices )
                {
                    if( ((Person)vertex).isDrillDownable /* && ((Person)vertex).Importance == 0 */)
                    {
                        persons.Add( (Person)vertex );
                        tickets++;
                    }
                } // end foreach
                percolumn = tickets / TICKETS_PER_COLUMN;


                int ggg = 0;
                for( int i = 0; i < tickets - 1; i++ )
                {
                    grf.AddEdge( new Edge<INotifyPropertyChanged>( persons[i], persons[i + 1], new BlindArrow() ) { Label = "blind" } );
                    ggg++;
                    if( ggg == percolumn )
                    {
                        i++;
                        ggg = 0;
                    }
                } // endfor
            } // endif
            else if( tickets == 0)
            {
                grf.RemoveSubGraph( sgrf );
            }
        } // endfunc


        public int enumerateDomainInForest( out List<string> domainNames)
        {
            int count = 0;
            domainNames = new List<string>();

            try
            {
                using (var forest = Forest.GetCurrentForest())
                {
                    foreach (Domain domain in forest.Domains)
                    {
                        //Console.WriteLine( domain.Name );
                        domainNames.Add(domain.Name);
                        domain.Dispose();
                        count++;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error enumerating domains in forest");
            }
            return count;
        } // endfunc


        public void highlightEdges(Person requestPerson)
        {
            var graph = this.Graph;

            // NS removed 06-12 graph.changeEdgeColorStart();
            graph.changeEdgeColorStart();

            foreach (IEdge ege in graph.Edges)
            {
                if (ege.Source == requestPerson)
                {
                    ege.edgeColor = "Blue";
                    if (typeof(Arrow) == ege.DestinationArrow.GetType())
                    {
                        ((Arrow)ege.DestinationArrow).Stroke = "Blue";
                        ((Arrow)ege.DestinationArrow).StrokeThickness = "2";
                    }
                }
                else
                {
                    ege.edgeColor = "LightGray";
                    if (typeof(Arrow) == ege.DestinationArrow.GetType())
                    {
                        ((Arrow)ege.DestinationArrow).Stroke = "LightGray";
                        ((Arrow)ege.DestinationArrow).StrokeThickness = "0.5";
                    }
                }
            }


            graph.resetDotGraph = false;
            this.Graph = graph;
            this.Graph.Changed += GraphChanged;
            MainWindow Form = Application.Current.Windows[0] as MainWindow;

            // NOTE: Graph object is not changing here, so no event is raised... needs another way...
            Form.GraphLayout.Graph = graph;

            graph.EndChangesDontRereadDot();

        }


        public void dehighlightEdges(Person requestPerson)
        {
            var graph = this.Graph;

             graph.changeEdgeColorStart();

            foreach (IEdge ege in graph.Edges)
            {
                object arrow = ege.DestinationArrow;
                ege.edgeColor = "Black";
                if (typeof(Arrow) == arrow.GetType())
                {
                    ((Arrow)ege.DestinationArrow).Stroke = "Black";
                    ((Arrow)ege.DestinationArrow).StrokeThickness = "1";
                }
            }
            MainWindow Form = Application.Current.Windows[0] as MainWindow;
            graph.resetDotGraph = false;
            this.Graph = graph;
            this.Graph.Changed += GraphChanged;

            // NOTE: Graph object is not changing here, so no event is raised... needs another way...
            Form.GraphLayout.Graph = graph;
            graph.EndChangesDontRereadDot();
        } // endfunc



        public void addNewEdge(string fromName, string idSID, string toName, string objSID, int privileges)
        {
            if (fromName.Contains('\\'))
                fromName = fromName.Split('\\')[1];
            if (toName.Contains('\\'))
                toName = toName.Split('\\')[1];

            if (!goingFromUsersToOjects.ContainsKey(fromName))
            {
                List<objectDesignators> objectNamesPerUser = new List<objectDesignators>();
                objectDesignators od = new objectDesignators();
                od.objectName = toName;
                od.privileges = privileges;
                od.sid = objSID;
                objectNamesPerUser.Add(od);

                goingFromUsersToOjects.Add(fromName, objectNamesPerUser);
            }
            else
            {
                objectDesignators od = new objectDesignators();
                od.objectName = toName;
                od.privileges = privileges;
                od.sid = objSID;
                List<objectDesignators> objectNamesPerUser;
                objectNamesPerUser = goingFromUsersToOjects[fromName];
                bool foundSame = false;
                foreach (objectDesignators obj in objectNamesPerUser)
                {
                    if (obj.objectName == toName || obj.sid == objSID)
                    {
                        obj.privileges |= privileges;
                        foundSame = true;
                        break;
                    }
                }
                if (!foundSame)
                    objectNamesPerUser.Add(od);
            }
        } // endfunc add edge




        public void showSkeletonKeyResults(bool needsRefresh)
        {
            Person legendStart = null;
            var graph = new Graph<INotifyPropertyChanged>();

            List<string> Objects = new List<string>();

            int objectnumber = 0;

            // "c:\Users\nimrod\Documents\Visual Studio 2015\Projects\graphviz4net_b19bb0cdc8c6\src\ZbangGui\bin\version\ZBANG\SkeletonKey_Scanner\Results\SkeletonKeyResults.csv" 
            if( !File.Exists( skeletonFile))
            {
                MessageBox.Show( skeletonFile + " File Name Not Found", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation );
                return;
            }

            var subGraph = new SubGraph<INotifyPropertyChanged> { Label = "Skeleton Key Results", borderColor = "LightGreen" };
            graph.AddSubGraph(subGraph);

            try
            {

                using (TextFieldParser parser = new TextFieldParser(skeletonFile))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (fields[0] == "DomainName") continue;

                        string domainName = fields[0];
                        string functionalLevel = fields[1];
                        string dcName = fields[2];
                        string infected = fields[3];
                        string AvatarStr = "./Images/dc-clean.png";
                        //string AvatarStr = "./Images/unnamed-update.png";

                        string legendStr = "Clean DC";

                        string status = "Not Infected";
                        string p = "LightGreen";
                        if (infected != "No")
                        {
                            p = "OrangeRed";
                            status = "INFECTED!";
                            AvatarStr = "./Avatars/Bug.png";
                            legendStr = "Infected DC";
                        }
                        string showname = dcName.Substring(0, dcName.Length > MAX_SHOWN_LETTERS ? MAX_SHOWN_LETTERS : dcName.Length);

                        var a = new Person(graph, this)
                        {
                            Name = dcName,
                            ShowName = showname,
                            Avatar = AvatarStr,
                            BackColor = p,
                            Domain = domainName + "\n" + status,
                            isDrillDownable = false,
                            LegendKey = legendStr
                        };
                        if (legendStart == null) legendStart = a;
                        subGraph.AddVertex(a);
                        objectnumber++;
                    } // endwhile
                } // endusing
            } // end of try
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
                return;
            }



            // ADD BLIND EDGES TO SPLIT GROUP TO TABLE - UPTO 4 CARDS in A ROW
            int tickets = objectnumber;
            int TICKETS_PER_COLUMN = 4;
            int percolumn = tickets / TICKETS_PER_COLUMN;
            if (percolumn > 0)
            {
                List<Person> persons = new List<Person>();
                foreach (var vertex in subGraph.Vertices)
                    persons.Add((Person)vertex);

                int ggg = 0;
                for (int i = 0; i < tickets - 1; i++)
                {
                    graph.AddEdge(new Edge<INotifyPropertyChanged>(persons[i], persons[i + 1], new BlindArrow()) { Label = "blind" });
                    ggg++;
                    if (ggg == percolumn)
                    {
                        legendStart = persons[i + 1];
                        i++;
                        ggg = 0;
                    }
                } // endfor
            } // endif
            // *******************
            // *** L E G E N D ***
            // *******************
            MainWindow Form = Application.Current.Windows[0] as MainWindow;
            if( Form.showLegend )
                addLegend( graph, legendStart.Name);

            this.Graph = graph;
            this.Graph.Changed += GraphChanged;
            this.NewPersonName = "Enter new name";
            this.UpdatePersonNewName = "Enter new name";

            if (needsRefresh)
            {
                Form.GraphLayout.Graph = graph;
            }
        } // endfunc




        /**
         * @brief       Tries to find all paths from a specific user (card) 
         *              to the main two objects: research.com (root domain) and AdminSDHolder
         *  
         *  
         **/
        Dictionary<string, List<objectDesignators>> entitiesToEdges = new Dictionary<string, List<objectDesignators>>();

        public void findPathFromUserToObject(Person initialPerson)
        {
            if (!initialPerson.isDrillDownable)
            {
                //MessageBox.Show("Cannot drill down a non-drillable object!", "Error", MessageBoxButton.OK);
                return;
            }
            // we have a er
            entitiesToEdges.Clear();
            greenRoute.Clear();
            greenRoutePrivileges.Clear();
            entities.Clear();
            greenRoute.Add(initialPerson.Name);
            greenRoutePrivileges.Add(0);
            SearchByRecursion(initialPerson.Name, myDomain.Split('.')[0], initialPerson);

            greenRoute.Clear();
            entities.Clear();
            greenRoutePrivileges.Clear();
            greenRoute.Add(initialPerson.Name);
            greenRoutePrivileges.Add(0);
            SearchByRecursion(initialPerson.Name, "AdminSDHolder", initialPerson);

            if(entitiesToEdges.Count == 0)
            {
                //MessageBox.Show("No further data for this account!", "Info", MessageBoxButton.OK);
                return;
            }


            onScreen = ON_SCREEN.ACLLIGHT_ON_SCREEN;

            MainWindow _Form = Application.Current.Windows[0] as MainWindow;
            _Form.backButton.Visibility = Visibility.Visible;


            // !!!! make a new graph according to recursion scan !!!!
            Dictionary<string, Person> cardEntities = new Dictionary<string, Person>();
            var graph = new Graph<INotifyPropertyChanged>();

            var subGraph = new SubGraph<INotifyPropertyChanged> { Label = "Targets" };
            graph.AddSubGraph(subGraph);

            foreach (string card in entitiesToEdges.Keys)
            {
                Person showPerson = null;
                if (nameToPerson.ContainsKey(card))
                {
                    showPerson = nameToPerson[card];
                }
                else if (nameToObject.ContainsKey(card))
                {
                    showPerson = nameToObject[card];
                }
                if (!cardEntities.ContainsKey(card))
                {
                    cardEntities.Add(card, showPerson);
                    if (card == "AdminSDHolder" || card == myDomain.Split('.')[0])
                    {
                        showPerson.maxHeight = MAX_PERSON_HEIGHT_NO_CHECKBOX;
                        showPerson.BackColor = "#fee7ea";       // red
                        showPerson.Avatar = "./Images/New target.png";
                        showPerson.LegendKey = "Domain Targets";
                        subGraph.AddVertex(showPerson);
                        legendStartName = showPerson.Name;
                    }
                    else
                    {
                        showPerson.maxHeight = MAX_PERSON_HEIGHT_NO_CHECKBOX;
                        graph.AddVertex(showPerson);
                    }
                }
                foreach (objectDesignators desig in entitiesToEdges[card])
                {
                    string edgeTarget = desig.objectName;
                    if (nameToPerson.ContainsKey(edgeTarget) || nameToObject.ContainsKey(edgeTarget))
                    {
                        // have target on graph already?
                        if (!cardEntities.ContainsKey(edgeTarget))
                        {
                            Person targetPerson = null;
                            if (nameToPerson.ContainsKey(edgeTarget))
                            {
                                targetPerson = nameToPerson[edgeTarget];
                                cardEntities.Add(edgeTarget, targetPerson);
                            }
                            else if (nameToObject.ContainsKey(edgeTarget))
                            {
                                targetPerson = nameToObject[edgeTarget];
                                cardEntities.Add(edgeTarget, targetPerson);
                            }
                            if (edgeTarget == "AdminSDHolder" || edgeTarget == myDomain.Split('.')[0])
                            {
                                targetPerson.maxHeight = MAX_PERSON_HEIGHT_NO_CHECKBOX;
                                targetPerson.BackColor = "#fee7ea"; //"Red";
                                targetPerson.Avatar = "./Images/New target.png";
                                targetPerson.LegendKey = "Domain Targets";
                                subGraph.AddVertex(targetPerson);
                                legendStartName = targetPerson.Name;
                            }
                            else
                            {
                                targetPerson.maxHeight = MAX_PERSON_HEIGHT_NO_CHECKBOX;
                                graph.AddVertex(targetPerson);
                            }
                        }
                        List<string> privnames = getPrivilegeNames(desig.privileges);
                        string outprivs = "";
                        for( int ax = 0; ax < privnames.Count; ax++ )
                        {
                            outprivs += privnames[ax];
                            if( ax < privnames.Count - 1 )
                                outprivs += "\n";
                        }

                        string Edgecolor = "black";
                        graph.AddEdge(new Edge<INotifyPropertyChanged>(showPerson, cardEntities[edgeTarget], new Arrow() { Stroke = Edgecolor })
                        {
                            edgeColor = Edgecolor,
                            edgeStrokeThickness = "1",
                            Label = /*privnames[0]*/outprivs,
                            labelToolTip = getLongPrivilegeNames(desig.privileges)
                        });
                    }
                } // endforeach objectdesignator edges for key
            } // endforeach key in entitiesToEdges
            MainWindow Form = Application.Current.Windows[0] as MainWindow;

            if (!Form.showLegend)
                addLegend(graph, legendStartName);

            graph.UpdateGraphRereadingDot();
            Form.GraphLayout.Graph = graph;

            this.Graph = graph;
            this.Graph.Changed += GraphChanged;

        } // endfunc


        Dictionary<string, int> entities = new Dictionary<string, int>();
        List<string> greenRoute = new List<string>();
        List<int> greenRoutePrivileges = new List<int>();


        int SearchByRecursion(string fromName, string searchString, Person person)
        {

            // NS 12/06/18    extract domain name...
            string _domainName = person.Domain;
            if (person.Domain.Contains('.'))
                _domainName = person.Domain.Split('.')[0];
            if (fromName.Contains('\\'))
                fromName = fromName.Split('\\')[1];
            //
            // if the name is not present in the edges Dictionary - no more looking...
            if (!person.modelView.goingFromUsersToOjects.ContainsKey(fromName))
            {
                greenRoute.RemoveAt(greenRoute.Count - 1);    // remove last and try again
                greenRoutePrivileges.RemoveAt(greenRoutePrivileges.Count - 1);
                return 0;
            }
            // get the list of all edges going out from name
            List<objectDesignators> objectsList = person.modelView.goingFromUsersToOjects[fromName];

            for (int i = 0; i < objectsList.Count; i++)
            {
                string objectn = objectsList[i].objectName;

                greenRoute.Add(objectn);
                greenRoutePrivileges.Add(objectsList[i].privileges);


                // if have that key in the entities then we've been at THIS PERSON before
                if (entities.ContainsKey(greenRoute[greenRoute.Count - 1]))
                {
                    // if previously _found_ entities contain the previous object 
                    // and we've been here before then there is no point in traversing
                    // the tree - we found another path...
                    if (entities[greenRoute[greenRoute.Count - 1]] > 1 || objectn == searchString)
                    {
                        writeResultsToFile();

                    }
                    greenRoute.RemoveAt(greenRoute.Count - 1);    // remove last and try again
                    greenRoutePrivileges.RemoveAt(greenRoutePrivileges.Count - 1);
                    continue;
                } // endif have key in green route already -- have we visited this entity??


                //objectsList[i].beenHere = true;               // this is for edges
                //
                // try marking using card name (Person entities only)
                //
                if (!entities.ContainsKey(objectn))
                {
                    entities.Add(objectn, 1);
                }

                // if current object == domain target
                if (objectn == searchString /*|| objectn == "DomainSDHolder"*/)
                {
                    writeResultsToFile();

                    greenRoute.RemoveAt(greenRoute.Count - 1);    // remove last and try again
                    greenRoutePrivileges.RemoveAt(greenRoutePrivileges.Count - 1);
                    continue;
                }
                else if (fromName != objectn)   // dont let the recursion go out of bounds...
                {
                    int ret = SearchByRecursion(objectn, searchString, person);
                    continue;
                }
                else
                {
                    greenRoute.RemoveAt(greenRoute.Count - 1);    // remove last and try again
                    greenRoutePrivileges.RemoveAt(greenRoutePrivileges.Count - 1);
                }
            } // endwhile
            if (greenRoute.Count > 0)
            {
                greenRoute.RemoveAt(greenRoute.Count - 1);    // remove last and try again
                greenRoutePrivileges.RemoveAt(greenRoutePrivileges.Count - 1);
            }
            return (0);
        } // endfunc SearchByRecursion


        /**
         * @brief   Writes results to file out.txt
         * 
         **/
        void writeResultsToFile()
        {
            Dictionary<string, int> recurrences = new Dictionary<string, int>();
            foreach (string g in greenRoute)
            {
                // don't write if have a recurrence of results

                if (recurrences.ContainsKey(g))
                    return;
                recurrences.Add(g, 1);
            } // end foreach

            for (int ai = 0; ai < greenRoute.Count; ai++)
            {
                if (entities.ContainsKey(greenRoute[ai]))
                    entities[greenRoute[ai]]++;
                else
                    entities.Add(greenRoute[ai], 1);
            } // endfor found

            StreamWriter wr = new StreamWriter("out.txt", true);
            for (int ai = 0; ai < greenRoute.Count; ai++)
                wr.Write(greenRoute[ai] + " > ");

            wr.WriteLine();
            wr.Close();
            //
            // also, update entitiesToEdges dictionary
            for (int a = 0; a < greenRoute.Count - 1; a++)
            {
                string name = greenRoute[a];
                string nameTo = greenRoute[a + 1];
                if (entitiesToEdges.ContainsKey(name))
                {
                    List<objectDesignators> edges = entitiesToEdges[name];
                    bool foundedge = false;
                    for (int inx = 0; inx < edges.Count; inx++)
                    {
                        if (edges[inx].objectName == nameTo)
                        {
                            edges[inx].privileges |= greenRoutePrivileges[a + 1];
                            foundedge = true;
                            break;
                        }
                    } // endfor scan all existing edges
                    if (!foundedge)
                    {
                        objectDesignators obj = new objectDesignators();
                        obj.objectName = nameTo;
                        obj.privileges = greenRoutePrivileges[a + 1];
                        obj.beenHere = false;
                        edges.Add(obj);
                    }
                } // endfor all winning routes
                else
                {
                    // entitiesToEdges does not contain the name found
                    objectDesignators obj = new objectDesignators();
                    obj.objectName = nameTo;
                    obj.privileges = greenRoutePrivileges[a + 1];
                    obj.beenHere = false;
                    List<objectDesignators> edges = new List<objectDesignators>();
                    edges.Add(obj);
                    entitiesToEdges.Add(name, edges);
                }
            } // end for
        } // endfunc 


        /**
         * @brief   drill down into one of the privileged accounts shown in the first 
         *          view
         *          
         * @param   Person
         * @return  None
         */

        private Person headPerson = null, drillGroup = null;
        public void drillDownFirstLevel(Person person)
        {
            MainWindow Form = Application.Current.Windows[0] as MainWindow;
            Form.allowMouseLeave = 0;

            findPathFromUserToObject(person);
            return;
#if zero
            if (!person.isDrillDownable && headPerson != null)
                drillGroup = person;

            if (headPerson == null)
            {
                headPerson = person;
            }
            if (!person.isDrillDownable && headPerson == null)
            {
                MessageBox.Show("Cannot drill down a non-drillable object!", "Error");
                headPerson = null;
                return;
            }

            Dictionary<string, Person> nonDrillableUsers = new Dictionary<string, Person>();

            //ACLightWindow win1 = new ACLightWindow( this);
            //win1.ShowDialog();
            var graph = new Graph<INotifyPropertyChanged>();
            graph.AddVertex(headPerson);
            string startName = headPerson.Name;

            dumpAllUsers(startName, headPerson, graph, nonDrillableUsers);

            MainWindow Form = Application.Current.Windows[0] as MainWindow;
            Form.GraphLayout.Graph = graph;
#endif
        }

#if zero

        /**
         * @brief   Dumps all users (People and Machines to the Graph
         **/
        public int dumpAllUsers(string startName, Person person, Graph<INotifyPropertyChanged>graph, Dictionary<string, Person>nonDrillables)
        {
            List<objectDesignators> objectsList = person.modelView.goingFromUsersToOjects[startName];
            bool goOut;

            for (int i = 0; i < objectsList.Count; i++)
            {
                goOut = false;
                string objectn = objectsList[i].objectName;
                // *** is it a person?
                if (/*objectn.Contains("Users") &&*/ nameToPerson.ContainsKey(objectn.Split('.')[0]))
                {
                    string username = objectn.Split('.')[0];
                    foreach( var vertex in graph.AllVertices)
                    {
                        if( vertex != null && vertex.GetType() == typeof(Person) && ((Person)vertex).Name == username)
                        {
                            int priv = objectsList[i].privileges;
                            List<string>privnames = getPrivilegeNames(priv);

                            string outprivs = "";
                            for (int ax = 0; ax < privnames.Count; ax++)
                                outprivs += privnames[ax] + "\n";

                            graph.AddEdge(new Edge<INotifyPropertyChanged>(person, nameToPerson[username], new Arrow()) { Label = /*privnames[0]*/outprivs, labelToolTip = getLongPrivilegeNames(priv) });
                            goOut = true;
                            break;
                        }
                    }
                    if (!goOut && nameToPerson[username].isDrillDownable)
                    {
                        int priv = objectsList[i].privileges;
                        List<string> privnames = getPrivilegeNames(priv);

                        string outprivs = "";
                        for (int ax = 0; ax < privnames.Count; ax++)
                            outprivs += privnames[ax] + "\n";

                        graph.AddVertex(nameToPerson[username]);
                        graph.AddEdge(new Edge<INotifyPropertyChanged>(person, nameToPerson[username], new Arrow()) { Label = /*privnames[0]*/outprivs, labelToolTip = getLongPrivilegeNames(priv), color = "green" });
                        dumpAllUsers(username, nameToPerson[username], graph, nonDrillables);
                    }
                    else if( !goOut && !nameToPerson[username].isDrillDownable)
                    {
                        int priv = objectsList[i].privileges;
                        List<string> privnames = getPrivilegeNames(priv);

                        string outprivs = "";
                        for (int ax = 0; ax < privnames.Count; ax++)
                            outprivs += privnames[ax] + "\n";

                        graph.AddVertex(nameToPerson[username]);
                        graph.AddEdge(new Edge<INotifyPropertyChanged>(person, nameToPerson[username], new Arrow()) { Label = /*privnames[0]*/outprivs, labelToolTip = getLongPrivilegeNames(priv), color = "green" });

                        if( !goOut && drillGroup != null && username == drillGroup.Name)
                           dumpAllUsers(username, drillGroup, graph, nonDrillables);
                        //dumpAllUsers(username, nameToPerson[username], graph, nonDrillables);
                    }
                    /*
                    else if( goOut && drillGroup != null && username == drillGroup.Name)
                        dumpAllUsers(username, nameToPerson[username], graph, nonDrillables);
                    */
                }
                else /*if( objectn.Contains("Users"))*/
                {
                    string username = objectn.Split('.')[0];
                    if (!nonDrillables.ContainsKey(username))
                    {
                        string showname = username.Substring(0, username.Length > MAX_SHOWN_LETTERS ? MAX_SHOWN_LETTERS : username.Length);
                        var a = new Person(graph, this) { Name = username, ShowName=/*username.Substring(0, 6)*/showname, Avatar = "./Avatars/AvatarAnon.gif", BackColor = "LightGray", isDrillDownable = false, Domain = myDomain };
                        graph.AddVertex(a);


                        int priv = objectsList[i].privileges;
                        List<string> privnames = getPrivilegeNames(priv);

                        string outprivs = "";
                        for (int ax = 0; ax < privnames.Count; ax++)
                            outprivs += privnames[ax] + "\n";

                        graph.AddEdge(new Edge<INotifyPropertyChanged>(person, a, new Arrow()) { Label = /*privnames[0]*/outprivs, labelToolTip = getLongPrivilegeNames(priv), color = "red" });
                        nonDrillables.Add(username, a);
                    }
                    else
                    {
                        var a = nonDrillables[username];
                        int priv = objectsList[i].privileges;
                        List<string> privnames = getPrivilegeNames(priv);

                        string outprivs = "";
                        for (int ax = 0; ax < privnames.Count; ax++)
                            outprivs += privnames[ax] + "\n";

                        graph.AddEdge(new Edge<INotifyPropertyChanged>(person, a, new Arrow()) { Label = /*privnames[0]*/outprivs, labelToolTip = getLongPrivilegeNames( priv) });
                    }
                }
#if zero
                //// ------- MACHINE -------
                else 
                {
                    MainWindow Form = Application.Current.Windows[0] as MainWindow;
                    if (Form.showMachines)
                    {
                        goOut = false;
                        foreach (var vertex in graph.AllVertices)
                        {
                            //if (vertex != null && vertex.GetType() == typeof(Machine) && ((Machine)vertex).realName == objectn)
                            if (vertex != null && vertex.GetType() == typeof(Person) && ((Person)vertex).Name == objectn)
                            {
                                int priv2 = objectsList[i].privileges;
                                List<string> privnames2 = getPrivilegeNames(priv2);
                                graph.AddEdge(new Edge<INotifyPropertyChanged>(person, objectsDict[objectn], new Arrow()) { Label = privnames2[0], labelToolTip = getLongPrivilegeNames(priv2) });
                                goOut = true;
                                break;
                            }
                        }
                        if (goOut)
                            continue;

                        // NS Machine m = objectsDict[objectn];
                        Person m = objectsDict[objectn];
                        graph.AddVertex(m);

                        int priv = objectsList[i].privileges;
                        List<string> privnames = getPrivilegeNames(priv);
                        graph.AddEdge(new Edge<INotifyPropertyChanged>(person, m, new Arrow()) { Label = privnames[0], labelToolTip = getLongPrivilegeNames(priv) });
                    }
                } // endif show machines
#endif
            } // endfor all objects
            return 0;
        }
#endif // zero

        /**
         * @brief   find the names of the privileges and return an array of their names
         * 
         */
        public List<string> getPrivilegeNames(int privsData)
        {
            int k = 0;
            List<string> retStrings = new List<string>();
            foreach (var privName in ObjectRights.Keys)
            {
                if ((privsData & ObjectRights[privName]) != 0 && shortPrivNames.ContainsKey(privName))
                {
                    retStrings.Add(shortPrivNames[privName]);
                }
                k++;
            } // endfor k
            if (retStrings.Count == 0)
                retStrings.Add("EMPTY");
            return retStrings;
        } // endfunction


        /**
         * @brief   returns a string with the long names of the privileges 
         * 
         */
        public string getLongPrivilegeNames(int privsData)
        {
            int k = 0;
            string retStrings = "";
            foreach (var privName in ObjectRights.Keys)
            {
                if ((privsData & ObjectRights[privName]) != 0)
                {
                    retStrings += privName + "\n";
                }
                k++;
            } // endfor k
            return retStrings;
        } // endfunction


        public Graph<INotifyPropertyChanged> Graph { get; private set; }

        public string NewPersonName { get; set; }

        public string UpdatePersonName { get; set; }

        public string UpdatePersonNewName { get; set; }

        public IEnumerable<string> PersonNames
        {
            get { return this.Graph.AllVertices.OfType<Person>().Select(x => x.Name); }
        }

        public string NewEdgeStart { get; set; }

        public string NewEdgeEnd { get; set; }

        public string NewEdgeLabel { get; set; }

        public void CreateEdge()
        {
            if (string.IsNullOrWhiteSpace(this.NewEdgeStart) ||
                string.IsNullOrWhiteSpace(this.NewEdgeEnd))
            {
                return;
            }

            this.Graph.AddEdge(
                new Edge<INotifyPropertyChanged>
                    (this.GetPerson(this.NewEdgeStart),
                    this.GetPerson(this.NewEdgeEnd))
                {
                    Label = this.NewEdgeLabel
                });
        }

        public void CreatePerson()
        {
            if (this.PersonNames.Any(x => x == this.NewPersonName))
            {
                // such a person already exists: there should be some validation message, but 
                // it is not so important in a demo
                return;
            }

            var p = new Person(this.Graph, this) { Name = this.NewPersonName };
            this.Graph.AddVertex(p);
        }

        public void UpdatePerson()
        {
            if (string.IsNullOrWhiteSpace(this.UpdatePersonName))
            {
                return;
            }

            this.GetPerson(this.UpdatePersonName).Name = this.UpdatePersonNewName;
            this.RaisePropertyChanged("PersonNames");
            this.RaisePropertyChanged("Graph");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GraphChanged(object sender, GraphChangedArgs e)
        {
            this.RaisePropertyChanged("PersonNames");
        }

        private void RaisePropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private Person GetPerson(string name)
        {
            return this.Graph.AllVertices.OfType<Person>().First(x => string.CompareOrdinal(x.Name, name) == 0);
        }



        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!e.Cancel)
            {
                Thread.Sleep(100);
            }

            /*
        for (int i = 0; i < 100; i++)
        {
            (sender as BackgroundWorker).ReportProgress(i);
            Thread.Sleep(100);
        }
        */
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //((MainWindow)Application.Current.Windows[0]).Progress.Value = e.ProgressPercentage;
        }

        /**
         * @brief       Build a legend using a Graph and a vertex name inside that graph
         * 
         **/
        Person fromVertexP;
        Machine fromVertexM;
        Dictionary<string, int> dict;
        SubGraph<INotifyPropertyChanged> subGraph;

        public void addLegend(Graph<INotifyPropertyChanged> grf, string fromVertexName)
        {
            dict = new Dictionary<string, int>();
            subGraph = new SubGraph<INotifyPropertyChanged>();

            fromVertexP = null;
            fromVertexM = null;
            subGraph.Label = "Legend";
            subGraph.borderColor = "Transparent";
            // find the from vertex
            bool foundv = false;
            foreach (var vertex in grf.Vertices)
            {
                if (typeof(Person) == vertex.GetType())
                {
                    Person person = (Person)vertex;
                    if (person.Name == fromVertexName)
                    {
                        foundv = true;
                        fromVertexP = person;
                        break;
                    }
                }
                else if (typeof(Machine) == vertex.GetType())
                {
                    Machine machine = (Machine)vertex;
                    if (machine.Name == fromVertexName)
                    {
                        foundv = true;
                        fromVertexM = machine;
                        break;
                    }
                }
            } // endof foreach
            if( !foundv)
            {
                foreach (var subgraph in grf.SubGraphs)
                {
                    foreach (var vertex in subgraph.Vertices)
                    {
                        if (typeof(Person) == vertex.GetType())
                        {
                            Person person = (Person)vertex;
                            if (person.Name == fromVertexName)
                            {
                                foundv = true;
                                fromVertexP = person;
                                break;
                            }
                        }
                        else if (typeof(Machine) == vertex.GetType())
                        {
                            Machine machine = (Machine)vertex;
                            if (machine.Name == fromVertexName)
                            {
                                foundv = true;
                                fromVertexM = machine;
                                break;
                            }
                        }
                    } // endforeach verices
                } // endof foreach subgraph
            } // endif !found vertex
            if (!foundv)
                return;

            foreach (var vertex in grf.Vertices)
            {
                Edge<INotifyPropertyChanged> edge = addLegendHelper(grf, vertex);
                if (edge != null)
                    grf.AddEdge(edge);
            }


            foreach (var subg in grf.SubGraphs)
            {
                foreach (var vertex in subg.Vertices)
                {
                    Edge<INotifyPropertyChanged> edge = addLegendHelper( grf, vertex);
                    if (edge != null)
                        grf.AddEdge(edge);
                }
            } // endforeach
            if (subGraph.Vertices.Count() > 0)
            {
                grf.AddSubGraph(subGraph);
            }
        } // endfunc add legend


        /**
         * @brief   Helper for the above legend function
         * 
         **/
        Edge<INotifyPropertyChanged> addLegendHelper(Graph<INotifyPropertyChanged> grf, object vertex)
        {
            if (typeof(Person) == vertex.GetType())
            {
                Person person = (Person)vertex;
                if (!dict.ContainsKey("P_" + person.LegendKey) && person.LegendKey != null)
                {
                    dict.Add("P_" + person.LegendKey, 1);
                    Legend leg = new Legend(grf);
                    leg.Avatar = person.Avatar;
                    leg.Name = person.LegendKey;

                    subGraph.AddVertex(leg);
                    if (fromVertexP != null)
                        return(new Edge<INotifyPropertyChanged>(fromVertexP, leg, new BlindArrow()) { Label = "blind" });
                    else if (fromVertexM != null)
                        return(new Edge<INotifyPropertyChanged>(fromVertexM, leg, new BlindArrow()) { Label = "blind" });
                }
            }
            else if (typeof(Machine) == vertex.GetType())
            {
                Machine machine = (Machine)vertex;

                if (machine.LegendKey == null)
                    return null;
                if (!dict.ContainsKey("M_" + machine.LegendKey))
                {
                    dict.Add("M_" + machine.LegendKey, 1);
                    Legend leg = new Legend(grf);
                    leg.Avatar = machine.Avatar;
                    leg.Name = machine.LegendKey;

                    subGraph.AddVertex(leg);

                    if (fromVertexP != null)
                        return( new Edge<INotifyPropertyChanged>(fromVertexP, leg, new BlindArrow()) { Label = "blind" });
                    else if (fromVertexM != null)
                        return( new Edge<INotifyPropertyChanged>(fromVertexM, leg, new BlindArrow()) { Label = "blind" });
                } // endif
            } // endif
            return null;
        } // endfunc


        /**
         * @brief       Finds the correct column numbers according to dictioary
         * 
         **/
        void findACLightColumns( string[] firstLine)
        {
            ACLightColumnFinder.Clear();

            for( int i = 0; i < firstLine.Length; i++ )
            {
                if( !ACLightColumnFinder.ContainsKey( firstLine[i] ))
                    ACLightColumnFinder.Add( firstLine[i], i );
            } // endfor
            return;
        }


        public BitmapImage ConvertStringtoImage(string commands)
        {

            byte[] _BinaryData = Convert.FromBase64String(commands);

            BitmapImage _ImageBitmap = new BitmapImage();
            _ImageBitmap.BeginInit();
            _ImageBitmap.StreamSource = new MemoryStream(_BinaryData);
            _ImageBitmap.EndInit();
            return _ImageBitmap;
        }



    } // endclass
}
