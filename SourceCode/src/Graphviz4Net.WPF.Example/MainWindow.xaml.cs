namespace Graphviz4Net.WPF.Example
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Threading;
    using System.Windows.Controls;
    using System.Diagnostics;
    using System.Reflection;
    using System.IO;
    using System.IO.Compression;
    using System.Windows.Media.Imaging;
    using System.Threading.Tasks;
    using RichTextBlockSample.HtmlConverter;
    using System.Windows.Markup;
    using System.Windows.Documents;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Win32;
    using System.Windows.Media.Animation;
    using System.Management.Automation;
    using System.Collections.ObjectModel;
    using System.Windows.Media;
    using System.DirectoryServices;
    //using System.Drawing;

    //using System.Windows.Forms;


    public partial class MainWindow : Window
    {
        private MainWindowViewModel viewModel;
        public bool showLegend = false, firstTimeShowACL = false;
        public int whatToRun;
        public bool quitApplication = false;

        public string logFileName = "../../ZBANG/log.txt";
        string totalString = "";


        public MainWindowViewModel ViewModel { get; set; }

        public enum TABITEMS
        {
            ACLLIGHT,
            SKELETONKEY,
            SIDHISTORY,
            RISKYSPNS,
            MYSTIQUE,
            EASYPEASY
        }

        private static BitmapImage GetImage(string imageUri)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("pack://siteoforigin:,,,/" + imageUri, UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();
            return bitmapImage;
        }



        public MainWindow()
        {
            try
            {
                this.viewModel = new MainWindowViewModel();
                ViewModel = viewModel;
                this.DataContext = viewModel;
                this.showLegend = true;
                InitializeComponent();



                //MessageBox.Show("testing456");

                using (PowerShell PowerShellInstance = PowerShell.Create())
                {
                    // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                    // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
                    PowerShellInstance.AddScript("$PSVersionTable");
                    Collection<PSObject> PSOutput = PowerShellInstance.Invoke();
                    string outputlog = "-------------------------------------------\n";
                    foreach (PSObject outputItem in PSOutput)
                    {
                        // we should be getting a hashtable... dump it to the log file
                        if (outputItem != null)
                        {
                            //TODO: do something with the output item 
                            System.Collections.Hashtable hash = outputItem.BaseObject as System.Collections.Hashtable;
                            if (((System.Version)hash["PSVersion"]).Major < 3)
                                MessageBox.Show("The zBang Tool needs PowerShell version >= 3.0\nPlease download an updated version from\nhttps://www.microsoft.com/en-us/download/details.aspx?id=34595");
                            foreach (string key in hash.Keys)
                            {
                                if (hash[key].GetType() == typeof(System.Version[]))
                                {
                                    System.Version[] vers = (System.Version[])hash[key];
                                    string verstr = "";
                                    foreach (System.Version ver in vers)
                                    {
                                        verstr += ver.ToString() + ",";
                                    }

                                    string format = "{0,-25}\t{1,-10}";
                                    string result = string.Format(format, key, verstr);
                                    outputlog += /*key + "\t\t\t" + verstr +*/result + "\n";
                                }
                                else
                                {
                                    string format = "{0,-25}\t{1,-10}";
                                    string result = string.Format(format, key, hash[key]);
                                    outputlog += result + "\n";
                                }
                            }
                        }
                    }
                    outputlog += "-------------------------------------------\n";
                    File.AppendAllText( /*"..\\..\\ZBANG\\log.txt"*/logFileName, "\n\nzBang Launched at " + DateTime.Now.ToString() + "\n" + outputlog + "\n");
                } // endusing powershell
                if (!GetDotNetVersion.Get45PlusFromRegistry(logFileName))
                {
                    MessageBox.Show(".NET Version is below 4.5\nPlease download .NET 4.5 from microsoft.com...\nPlease download from https://www.microsoft.com/en-us/download/details.aspx?id=30653 \nAborting...", "Info", MessageBoxButton.OK, MessageBoxImage.Hand);
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
                //
                // report to temp log file all the domain names in the forest
                try
                {
                    List<string> domainNames;
                    int count = viewModel.enumerateDomainInForest(out domainNames);
                    string _outputlog = "\n" + count.ToString() + " domain(s) in forest:\n";
                    int c = 0;
                    foreach (string dom in domainNames)
                    {
                        c++;
                        _outputlog += "(" + c.ToString() + ") " + dom + "\n";
                    }
                    File.AppendAllText(logFileName, _outputlog);
                }
                catch
                {

                }

                /** LOAD IMAGES **/
                //string ImagesPath = "pack://application:,,/Graphviz4Net.WPF.Example;component/Images/aetosdios_trans.png";
                string ImagesPath = "./Images/flashlight.png";
                Uri uri = new Uri(ImagesPath, UriKind.RelativeOrAbsolute);
                BitmapImage bitmap = new BitmapImage(uri);
                imgACLight.Source = bitmap;
                imgSkeleton.Source = new BitmapImage(new Uri(@"Images/aetosdios_trans.png", UriKind.Relative));
                imgRisky.Source = new BitmapImage(new Uri(@"./Images/aetosdios_trans.png", UriKind.Relative));
                imgSID.Source = new BitmapImage(new Uri(@"./Images/aetosdios_trans.png", UriKind.Relative));
                imgMistique.Source = new BitmapImage(new Uri(@"./Images/aetosdios_trans.png", UriKind.Relative));
                bulletACL.Source = new BitmapImage(new Uri(@"./Images/flashlight.png", UriKind.Relative));
                bulletSkeleton.Source = new BitmapImage(new Uri(@"./Images/key.png", UriKind.Relative));
                bulletSID.Source = new BitmapImage(new Uri(@"./Images/theater.png", UriKind.Relative));
                bulletSPN.Source = new BitmapImage(new Uri(@"./Images/clerk.png", UriKind.Relative));//Source="pack://application:,,,/Images/bullet_grey.png" 
                bulletMystique.Source = new BitmapImage(new Uri(@"./Images/role-playing-game.png", UriKind.Relative));
                imgArrowLeft.Source = new BitmapImage(new Uri(@"./Images/arrow_left.png", UriKind.Relative));

                /* NS GITHUB
                imageLogo.Source = new BitmapImage(new Uri(@"./Images/LogoSeparator_b.png", UriKind.Relative));
                */
                ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject), new FrameworkPropertyMetadata(Int32.MaxValue));

                tabControl.Visibility = Visibility.Hidden;
                GraphLayout.Visibility = Visibility.Hidden;

                this.showmach.IsChecked = true;
                this.showmach2.IsChecked = true;


                /*
                this.AddNewEdge.Click += AddNewEdgeClick;
                this.AddNewPerson.Click += AddNewPersonClick;
			    this.UpdatePerson.Click += UpdatePersonClick;
                */

                zoomControl.MaxZoom = 3.0;
                zoomControl.ZoomDeltaMultiplier = 20.0;

                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.WindowState = WindowState.Maximized;

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;

                //this.Title = "Zbang Tool Suite, ver." + version + "       CyberArk 2018";
                this.lblCursorPosition.Text = "zBang Tool Suite, ver." + version /*+ "       CyberArk 2018" GITHUB */;

                // set background image for the grid
                string _ImagesPath = "pack://application:,,/Graphviz4Net.WPF.Example;component/Images/logo_color.png";

                ImageBrush myBrush = new ImageBrush();
                /* NS GITHUB
                                Image image = new Image();
                                image.Source = new BitmapImage( new Uri( _ImagesPath, UriKind.RelativeOrAbsolute ) );
                                myBrush.ImageSource = image.Source;
                                zoomControl.Background = myBrush;
                zoomControl.Background.Opacity = 0.09;
                */
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
        } // end MainWindow


#if zero
        using System;
        using System.IO;
        public class BasicTest
        {

            // try get images from DC
            static string GetUserPicture(string userName)
            {
                string LDAP = "LDAP://AETOSDIOS";
                //using (DirectoryEntry dirEntry = new DirectoryEntry(/*LDAP, null, null, AuthenticationTypes.Secure)*/)
                {
                    using (DirectorySearcher dsSearcher = new DirectorySearcher( /*dirEntry*/))
                    {
                        dsSearcher.Filter = "(&(objectCategory=person)(sAMAccountName=" + userName + "))";
                        SearchResult result = dsSearcher.FindOne();

                        using (DirectoryEntry user = new DirectoryEntry(result.Path))
                        {
                            byte[] data = user.Properties["thumbnailPhoto"].Value as byte[];

                            if (data != null)
                            {
                                using (MemoryStream s = new MemoryStream(data))
                                {
                                    byte[] byteArr = s.ToArray();
                                    string binStr = Convert.ToBase64String(byteArr);
                                    return binStr;
                                }
                            }

                            return null;
                        }
                    }
                }
            } // endfunc
        }

#endif






        void runLaunchWindow()
        {

            selectTogglesForms atoggle = new selectTogglesForms();
            bool dresult = (bool)atoggle.ShowDialog();
            if (!dresult)
            {
                //System.Windows.Application.Current.Shutdown();
                return;
            }
            toggleACLight.IsChecked = false;
            toggleSkeleton.IsChecked = false;
            toggleSIDHistory.IsChecked = false;
            toggleRisky.IsChecked = false;
            toggleMystique.IsChecked = false;

            int aq = atoggle.whoIsSelected & (1 << (int)MainWindow.TABITEMS.ACLLIGHT);
            if (aq != 0)
                toggleACLight.IsChecked = true;
            aq = atoggle.whoIsSelected & (1 << (int)MainWindow.TABITEMS.SKELETONKEY);
            if (aq != 0)
                toggleSkeleton.IsChecked = true;
            aq = atoggle.whoIsSelected & (1 << (int)MainWindow.TABITEMS.SIDHISTORY);
            if (aq != 0)
                toggleSIDHistory.IsChecked = true;
            aq = atoggle.whoIsSelected & (1 << (int)MainWindow.TABITEMS.RISKYSPNS);
            if (aq != 0)
                toggleRisky.IsChecked = true;
            aq = atoggle.whoIsSelected & (1 << (int)MainWindow.TABITEMS.MYSTIQUE);
            if (aq != 0)
                toggleMystique.IsChecked = true;

            if (atoggle.isLaunched == 1)
            {
                this.LaunchZbang.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else if (atoggle.isLaunched == 0)
                this.Launch.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            else if (atoggle.isLaunched == 2)
            {
                this.ImportButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        } // endfunc launch window


        void UpdatePersonClick(object sender, RoutedEventArgs e)
        {
            /*
			this.viewModel.UpdatePersonName = (string) this.UpdatePersonName.SelectedItem;
			this.viewModel.UpdatePerson();
            */
        }

        private void AddNewPersonClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.CreatePerson();
        }

        private void AddNewEdgeClick(object sender, RoutedEventArgs e)
        {
            /*
            this.viewModel.NewEdgeStart = (string) this.NewEdgeStart.SelectedItem;
            this.viewModel.NewEdgeEnd = (string)this.NewEdgeEnd.SelectedItem;
            this.viewModel.CreateEdge();
            */
        }
        int pressed = 0;

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            rtbACL.Visibility = Visibility.Hidden;                      // hide the rich text ACL view
            pressed++;
            Launch.IsEnabled = true;
            LaunchZbang.IsEnabled = true;
            /*
            ToggleButton tb = (ToggleButton)sender;
            tb.Background = System.Windows.Media.Brushes.LightGreen;
            tb.UpdateLayout();
            */
        }
        private void ToggleButton_UnChecked(object sender, RoutedEventArgs e)
        {
            rtbACL.Visibility = Visibility.Hidden;                      // hide the rich text ACL view
            pressed--;
            if (pressed <= 0)
            {
                Launch.IsEnabled = false;
                LaunchZbang.IsEnabled = false;
            }

            /*
            ToggleButton tb = (ToggleButton)sender;
            tb.Background = System.Windows.Media.Brushes.LightGreen;
            tb.UpdateLayout();
            */
        }

        private void launchButton_clicked(object sender, RoutedEventArgs e)
        {
            rtbACL.Visibility = Visibility.Hidden;                      // hide the rich text ACL view
            zoomControl.MaxZoom = 3.0;
            zoomControl.Mode = WPFExtensions.Controls.ZoomControlModes.Fill;

            tabControl.Visibility = Visibility.Visible;
            GraphLayout.Visibility = Visibility.Visible;

            //---> disable all the tabitems not in the launch game
            if (!(bool)toggleACLight.IsChecked)
                ACLLight.IsEnabled = false;
            else
                ACLLight.IsEnabled = true;

            if (!(bool)toggleSkeleton.IsChecked)
                SkeletonItem.IsEnabled = false;
            else
                SkeletonItem.IsEnabled = true;

            if (!(bool)toggleSIDHistory.IsChecked)
                SIDHistoryItem.IsEnabled = false;
            else
                SIDHistoryItem.IsEnabled = true;

            if (!(bool)toggleRisky.IsChecked)
                SPNItem.IsEnabled = false;
            else
                SPNItem.IsEnabled = true;
            if (!(bool)toggleMystique.IsChecked)
                MystiqueItem.IsEnabled = false;
            else
                MystiqueItem.IsEnabled = true;
            /*
            if (!(bool)toggleEasy.IsChecked)
                EasyPeasy.IsEnabled = false;
            else
                EasyPeasy.IsEnabled = true;
            */
            // if acl light button pressed?
            if ((bool)toggleACLight.IsChecked)
            {
                if (!ACLLight.IsSelected)
                    firstTimeShowACL = true;

                ACLLight.IsSelected = true;
                if (viewModel.scanACLoutputForDomains(null, "ACLight Discovered Domains") == 1)
                    this.viewModel.reformatCardsGraph(null, true);
                else
                    viewModel.onScreen = MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS;
            }
            else if ((bool)toggleSkeleton.IsChecked)
            {
                SkeletonItem.IsSelected = true;
                //this.viewModel.showSkeletonKeyResults(true);
            }
            else if ((bool)toggleSIDHistory.IsChecked)
            {
                SIDHistoryItem.IsSelected = true;
            }
            else if ((bool)toggleRisky.IsChecked)
            {
                SPNItem.IsSelected = true;
            }
            else if ((bool)this.toggleMystique.IsChecked)
            {
                MystiqueItem.IsSelected = true;
            }
            /*
            else if ((bool)toggleEasy.IsChecked)
            { }
            */
        } // endfunc

        private void menuShowMachines(object sender, RoutedEventArgs e)
        {
            if (sender == showmach || sender == showmach2)
            {
                showmach.IsChecked = !showmach.IsChecked;
                showmach2.IsChecked = !showmach2.IsChecked;
                showLegend = showmach.IsChecked;
                //this.viewModel.reformatCardsGraph();
            }
        }

        private void menuRefresh(object sender, RoutedEventArgs e)
        {
        }

        private void tabItemACL_Clicked(object sender, MouseButtonEventArgs e)
        {
        }
        private void tabItemSkeleton_Clicked(object sender, MouseButtonEventArgs e)
        {
        }
        private void tabItemSID_Clicked(object sender, MouseButtonEventArgs e)
        {
        }
        private void tabItemSPN_Clicked(object sender, MouseButtonEventArgs e)
        {
        }
        private void tabItemEasy_Clicked(object sender, MouseButtonEventArgs e)
        {
        }
        private void tabItemMystique_Clicked(object sender, MouseButtonEventArgs e)
        {
        }

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            rtbACL.Visibility = Visibility.Hidden;                      // hide the rich text ACL view
            zoomControl.MaxZoom = 3.0;
            zoomControl.Mode = WPFExtensions.Controls.ZoomControlModes.Fill;



            int sel = tabControl.SelectedIndex;
            if (sel == (int)TABITEMS.ACLLIGHT && Launch.IsEnabled && !firstTimeShowACL)
            {
                if (viewModel.scanACLoutputForDomains(null, "ACLight Discovered Domains") == 1)
                {
                    this.viewModel.reformatCardsGraph(null, true);
                }
                else
                    viewModel.onScreen = MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS;
            }
            else if (sel == (int)TABITEMS.SKELETONKEY && Launch.IsEnabled)
            {
                this.viewModel.showSkeletonKeyResults(true);
            }

            else if (sel == (int)TABITEMS.MYSTIQUE && Launch.IsEnabled)
            {
                viewModel.onScreen = MainWindowViewModel.ON_SCREEN.RISKYSPN_ON_SCREEN;
                Mystique.Run(null, this.viewModel.mysticInputFile);
            }
            else if (sel == (int)TABITEMS.SIDHISTORY && Launch.IsEnabled)
            {
                viewModel.onScreen = MainWindowViewModel.ON_SCREEN.SIDHISTORY_ON_SCREEN;
                SIDHistory.Run(null, this.viewModel.sidHistoryInputFile);
            }
            else if (sel == (int)TABITEMS.RISKYSPNS && Launch.IsEnabled)
            {
                int ret;
                if ((ret = viewModel.scanACLoutputForDomains(this.viewModel.RISKYSPNInputFile, "RiskySPNs Discovered Domains")) <= 1)
                {
                    viewModel.onScreen = MainWindowViewModel.ON_SCREEN.RISKYSPN_ON_SCREEN;
                    riskySPNs.Run(null, this.viewModel.RISKYSPNInputFile);
                }
                else
                    viewModel.onScreen = MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS_RISKY_SPN;
            }
            firstTimeShowACL = false;
        } // endfunc

        private void labelMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (rtbACL.Visibility == Visibility.Visible)
            {
                rtbACL.Visibility = Visibility.Hidden;
                return;
            }

            if (sender.GetType() == typeof(TextBlock))
            {
                TextBlock block = (TextBlock)sender;
                string selectedText = block.Text;

                if (this.ViewModel.onScreen != MainWindowViewModel.ON_SCREEN.ACLLIGHT_ON_SCREEN)
                    return;
                string curDir = Directory.GetCurrentDirectory();
                StreamReader sr = File.OpenText(String.Format("{0}/../../ZBANG/ACLight Attack Path Update.html", curDir));
                TextBox.Text = sr.ReadToEnd();
                sr.Close();
                /*
                // select the required text
                string richText = new TextRange( rtbACL.Document.ContentStart, rtbACL.Document.ContentEnd ).Text;

                int iii = richText.IndexOf( selectedText );
                TextPointer text = rtbACL.Document.ContentStart;
                TextPointer startPos = text.GetPositionAtOffset( iii );
                TextPointer endPos = text.GetPositionAtOffset( iii + 40 );
                var textRange = new TextRange( startPos, endPos );
                textRange.ApplyPropertyValue( TextElement.FontWeightProperty, FontWeights.Bold );
                */


                rtbACL.Visibility = Visibility.Visible;
            }
        }

        private Person highlightedPerson = null;
        private DateTime showtime;
        public int allowMouseLeave;
        private void Border_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender.GetType() == typeof(System.Windows.Shapes.Path))
            {
                System.Windows.Shapes.Path pth = (System.Windows.Shapes.Path)sender;
                pth.Stroke = System.Windows.Media.Brushes.Gold;
                return;
            }
            if (sender.GetType() == typeof(System.Windows.Controls.TextBlock))
            {
                System.Windows.Controls.TextBlock pth = (System.Windows.Controls.TextBlock)sender;
                pth.Background = System.Windows.Media.Brushes.Gold;
                return;
            }

            Person ppp = (Person)(((System.Windows.Controls.Border)sender).DataContext);
            if (ppp != highlightedPerson && this.viewModel.onScreen == MainWindowViewModel.ON_SCREEN.ACLLIGHT_ON_SCREEN)
            {
                this.viewModel.highlightEdges(ppp);
                highlightedPerson = ppp;
                showtime = DateTime.Now;
                allowMouseLeave = 0;
            }
            else if (ppp == highlightedPerson && allowMouseLeave == 0)
                allowMouseLeave = 1;
        }

        private void OnGraphLayoutUpdated(object sender, EventArgs e)
        {
            /*
            if ((DateTime.Now - showtime).TotalMilliseconds < 2000)
                allowMouseLeave = 1;
            */
        }

        private void LabelClicked(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }




        private void Border_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender.GetType() == typeof(System.Windows.Shapes.Path))
            {
                System.Windows.Shapes.Path pth = (System.Windows.Shapes.Path)sender;
                pth.Stroke = System.Windows.Media.Brushes.Black;
                return;
            }
            if (sender.GetType() == typeof(System.Windows.Controls.TextBlock))
            {
                System.Windows.Controls.TextBlock pth = (System.Windows.Controls.TextBlock)sender;
                pth.Background = System.Windows.Media.Brushes.Transparent;
                return;
            }

            if (allowMouseLeave != 1)
                return;

            Person ppp = (Person)(((System.Windows.Controls.Border)sender).DataContext);
            if (/*ppp == highlightedPerson &&*/ this.viewModel.onScreen == MainWindowViewModel.ON_SCREEN.ACLLIGHT_ON_SCREEN)
            {
                this.viewModel.dehighlightEdges(ppp);
                highlightedPerson = null;
            }
        }

        private void GoBackButton(object sender, RoutedEventArgs e)
        {
            rtbACL.Visibility = Visibility.Hidden;                      // hide the rich text ACL view
            highlightedPerson = null;
            allowMouseLeave = 0;
            showtime = new DateTime(0);
            if (this.viewModel.onScreen == MainWindowViewModel.ON_SCREEN.ACLLIGHT_ON_SCREEN)
                this.viewModel.reformatCardsGraph(null, true);
            else if (this.viewModel.onScreen == MainWindowViewModel.ON_SCREEN.CARDS_ON_SCREEN_WITH_DOMAIN_SELECTION)
            {
                this.viewModel.scanACLoutputForDomains(null, "ACLight Discovered Domains");
                viewModel.onScreen = MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS;
            }
            else if (this.viewModel.onScreen == MainWindowViewModel.ON_SCREEN.RISKYSPN_WITH_DOMAIN_SELECTION)
            {
                if (viewModel.scanACLoutputForDomains(this.viewModel.RISKYSPNInputFile, "RiskySPN Discovered Domains") == 1)
                {
                    viewModel.onScreen = MainWindowViewModel.ON_SCREEN.RISKYSPN_ON_SCREEN;
                    riskySPNs.Run(null, this.viewModel.RISKYSPNInputFile);
                    backButton.Visibility = Visibility.Hidden;
                }
                else
                {
                    viewModel.onScreen = MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS_RISKY_SPN;
                    backButton.Visibility = Visibility.Hidden;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox a = (CheckBox)sender;
            object der = (object)a.Parent;
            if (typeof(Person) == der.GetType())
            {
                Person person = (Person)der;
                person.BackColor = "LightGreen";
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {

        }


        /**
         * @brief   LAUNCH POWERSHELL MODULES
         **/
        ProgressWindow prg;
        private void launchPowershellButton_clicked(object sender, RoutedEventArgs e)
        {
            rtbACL.Visibility = Visibility.Hidden;                      // hide the rich text ACL view

            /*
                        . $PSScriptRoot\SourceFiles\RiskySPN-master\Get-PotentiallyCrackableAccounts.ps1
                        Report-PotentiallyCrackableAccounts -Type CSV -Path $PSScriptRoot\Results\RiskySPNs -DoNotOpen -Name RiskySPNs -test
                        */

            if (whatToRun == 0)
            {
                prg = new ProgressWindow();
                prg.Show();
                prg.textBlockPowershell.Text += "\n";

                if ((bool)toggleSIDHistory.IsChecked)
                    whatToRun |= (1 << 2);
                if ((bool)toggleSkeleton.IsChecked)
                    whatToRun |= (1 << 1);
                if ((bool)toggleRisky.IsChecked)
                    whatToRun |= (1 << 3);
                if ((bool)toggleMystique.IsChecked)
                    whatToRun |= (1 << 4);
                if ((bool)this.toggleACLight.IsChecked)
                    whatToRun |= (1 << 0);
            }
            // END OF ROUND
            else if (whatToRun == -1)
            {
                prg.label1.Content = "*** FINISHED ***";
                prg.progressBar1.Visibility = Visibility.Hidden;

                // press the click on the launch button
                Launch.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                whatToRun = 0;
                //MessageBox.Show( logString, "Log Output for Launch at " + DateTime.Now.ToString(), MessageBoxButton.OK );

                // NS 0.27 23/5/18
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.FileVersion;

                string machineName = System.Environment.MachineName;
                string username = System.Environment.UserName;

                PerformanceCounter cpuCounter;
                PerformanceCounter ramCounter;
                cpuCounter = new PerformanceCounter(
                    "Processor",
                    "% Processor Time",
                    "_Total",
                    true
                    );

                ramCounter = new PerformanceCounter("Memory", "Available MBytes", true);

                File.AppendAllText(logFileName/*"..\\..\\ZBANG\\log.txt"*/, "Log Output for Launch at " + DateTime.Now.ToString() + ", version " + version + "\n" +
                                                                                "Free RAM: " + ramCounter.NextValue().ToString() + "MB\n" +
                                                                                "CPU Usage: " + cpuCounter.NextValue().ToString() + "%\n" +
                                                                                "Computer Name: " + machineName + ", user Name: " + username + "\n" +
                                                                                logString + "\n\n\n\n");
                ExportButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                totalString = "";
                prg.Close();
                return;
            }

            if ((whatToRun & 1) != 0)                  // aclight
            {
                //string args = "/K \"powershell.exe -ExecutionPolicy Bypass -noprofile -command \"Import-Module './ACLight.psm1' -force ; Start-ACLsAnalysis\"\"";
                string args = "-ExecutionPolicy Bypass -noprofile -command \"Import-Module './ACLight.ps1' -force ; Start-ACLsAnalysis\"";
                // testing
                ////////// ????   windowRunPowerShell("Import-Module './../../ZBANG/ACLight-master/ACLight.psm1 ; Start-ACLsAnalysis'");

                //List<string> domainNames;
                //int count = viewModel.enumerateDomainInForest(out domainNames);
                //if (count > 1)
                //{
                //    domainSelection ds = new domainSelection(domainNames, "ACLight");
                //    bool dialogresult = (bool)ds.ShowDialog();
                //    //
                //    if (ds.selection != -1)
                //    {
                //        args = args.Substring(0, args.Length - 1) + " -domain '" + domainNames[ds.selection] + "'\"";
                //        // NO NEED !!!  MessageBox.Show( args, "error" );
                //    }
                //}


                modifyArgsByDomainSelections(ref args, "ACLight");

#if zero
                // start by scanning all comain in forest and if more than one domain is present pop up the question
                int count = viewModel.enumerateDomainInForest();
                if( count >= 1)
                {
                    borderACLightQ.Visibility = Visibility.Visible;
                    DoubleAnimation myDoubleAnimation = new DoubleAnimation();
                    myDoubleAnimation.From = 0.0;
                    myDoubleAnimation.To = 1.0;
                    myDoubleAnimation.Duration = new Duration( TimeSpan.FromMilliseconds( 1000 ) );

                    // Configure the animation to target the button's Width property.
                    Storyboard.SetTargetName( myDoubleAnimation, borderACLightQ.Name );
                    Storyboard.SetTargetProperty( myDoubleAnimation, new PropertyPath( Border.OpacityProperty ) );

                    // Create a storyboard to contain the animation.
                    Storyboard myWidthAnimatedButtonStoryboard = new Storyboard();
                    myWidthAnimatedButtonStoryboard.Children.Add( myDoubleAnimation );
                    myWidthAnimatedButtonStoryboard.Begin( borderACLightQ );
                    return;
                }
#endif
                //prg.Show();
                prg.progressBar1.Value = 50;
                prg.label1.Content = "Now running ACLight test...";

                logString = "";
                logString += "\n--------------------------------------------\nPowershell - ACLight:\n--------------------------------------------\n\n";
                totalString += "\n\n\n" + DateTime.Now.ToString() + "> Now Running ACLight\n";
                File.AppendAllText( /*"..\\..\\ZBANG\\log.txt"*/logFileName, logString);


                try
                {
                    File.Delete("../../ZBANG/ACLight-master/Results/Privileged Accounts - Final Report.csv");
                }
                catch { }




                var processStartInfo = new ProcessStartInfo
                {
                    //FileName = @"../../ZBANG/ACLight-master/Execute-ACLight.bat",
                    FileName = "powershell.exe", //@"cmd.exe",
                    Arguments = args,
                    WorkingDirectory = "../../ZBANG/ACLight-master",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var process = Process.Start(processStartInfo);
                process.OutputDataReceived += CaptureOutput;
                process.ErrorDataReceived += CaptureOutput;
                Thread runThread = new Thread(this.runThread);
                runThread.Start(process);
            } // endof aclight
            else if ((whatToRun & (1 << 1)) != 0)                  // skeleton
            {
                //prg.Show();
                prg.progressBar1.Value = 50;
                prg.label1.Content = "Now running Skeleton Key test...";
                totalString += "\n\n\n" + DateTime.Now.ToString() + "> Now Running SkeletonKey\n";

                try
                {
                    File.Delete("../../ZBANG/SkeletonKey_Scanner/Results/SkeletonKeyResults.csv");
                }
                catch { }

                logString = "\n--------------------------------------------\nPowershell - SkeletonKey:\n--------------------------------------------\n\n";
                File.AppendAllText( /*"..\\..\\ZBANG\\log.txt"*/logFileName, logString);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"Powershell.exe",
                    Arguments = "-ExecutionPolicy bypass -noprofile -command \"./SkeletonCheck.ps1\"",
                    WorkingDirectory = "../../ZBANG/SkeletonKey_Scanner",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var process = Process.Start(processStartInfo);
                process.OutputDataReceived += CaptureOutput;
                process.ErrorDataReceived += CaptureOutput;
                Thread runThread = new Thread(this.runThread);
                runThread.Start(process);
            }
            else if ((whatToRun & (1 << 2)) != 0)                   // SID History
            {
                //prg.Show();
                prg.progressBar1.Value = 50;
                prg.label1.Content = "Now running SID History test...";
                totalString += "\n\n\n" + DateTime.Now.ToString() + "> Now Running SID History\n";

                logString = "\n--------------------------------------------\nPowershell - SID History:\n--------------------------------------------\n\n";
                File.AppendAllText( /*"..\\..\\ZBANG\\log.txt"*/logFileName, logString);

                string args = "-ExecutionPolicy Bypass -noprofile -command \"Import-Module '.\\SIDHistory_Scanner.ps1' -force ; Report-UsersWithSIDHistory\"";
                //string args = "-ExecutionPolicy Bypass -noprofile -Command \"& { ./SIDHistory_Scanner.ps1 }\"";
                modifyArgsByDomainSelections(ref args, "SID History");

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"Powershell.exe",
                    //Arguments = "-ExecutionPolicy Bypass -noprofile -Command \"& . ./SIDHistory_Scanner.ps1; Report-UsersWithSIDHistory '-Type \"CSV\"' '-Path \"Results\"' '-DoNotOpen'\"",
                    //Arguments = "-ExecutionPolicy Bypass -noprofile -Command \"& { ./SIDHistory_Scanner.ps1; Report-UsersWithSIDHistory -Type CSV -Path Results -DoNotOpen }\"",

                    //Arguments = "-ExecutionPolicy Bypass -noprofile -Command \"& { ./SIDHistory_Scanner.ps1 }\"",
                    Arguments = args,

                    WorkingDirectory = "../../ZBANG/SIDHistory",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var process = Process.Start(processStartInfo);
                process.OutputDataReceived += CaptureOutput;
                process.ErrorDataReceived += CaptureOutput;
                Thread runThread = new Thread(this.runThread);
                runThread.Start(process);
            }
            else if ((whatToRun & (1 << 3)) != 0)                   // RiskySPN
            {
                //prg.Show();
                prg.progressBar1.Value = 50;

                try
                {
                    File.Delete("../../ZBANG/RiskySPN-Master/Results/RiskySPNs-test.csv");
                }
                catch { }


                logString = "\n--------------------------------------------\nPowershell - RiskySPN:\n--------------------------------------------\n\n";
                File.AppendAllText( /*"..\\..\\ZBANG\\log.txt"*/logFileName, logString);

                prg.label1.Content = "Now running RiskySPN test...";
                totalString += "\n\n\n" + DateTime.Now.ToString() + "> Now Running RiskySPN\n";

                //string args = "-ExecutionPolicy Bypass -noprofile -command \"& { ./Find-PotentiallyCrackableAccounts.ps1 }\"";
                string args = "-ExecutionPolicy Bypass -noprofile -command \"Import-Module '.\\Find-PotentiallyCrackableAccounts.ps1' -force ; Export-PotentiallyCrackableAccounts\"";
                modifyArgsByDomainSelections(ref args, "RiskySPN");

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"Powershell.exe",
                    //Arguments = "-ExecutionPolicy Bypass -noprofile -command \"./Get-PotentiallyCrackableAccounts.ps1; Report-PotentiallyCrackableAccounts -Type 'CSV' -DoNotOpen -Path 'Results/' -Name 'RiskySPNs-test'\"",
                    //Arguments = "-ExecutionPolicy Bypass -noprofile -Command \"& { .\\Get-PotentiallyCrackableAccounts.ps1; Report-PotentiallyCrackableAccounts -Type CSV -DoNotOpen -Path Results/ -Name RiskySPNs-test}\"",
                    //Arguments = "-ExecutionPolicy Bypass -noprofile -Command \"& { ./Get-PotentiallyCrackableAccounts.ps1 }",

                    //##### 26/12/2017      Arguments = "-ExecutionPolicy Bypass -noprofile -command \"Import-Module './RiskySPNs.psm1' -force\"",
                    Arguments = args,
                    //Arguments = args,

                    //string args = ;

                    /*-Type 'CSV' -Path '..\\..\\ZBANG\\RiskySPN-master\\Results\\' -DoNotOpen -Name 'RiskySPNs-test'",*/
                    WorkingDirectory = "../../ZBANG/RiskySPN-Master",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var process = Process.Start(processStartInfo);
                process.OutputDataReceived += CaptureOutput;
                process.ErrorDataReceived += CaptureOutput;
                Thread runThread = new Thread(this.runThread);
                runThread.Start(process);
            }
            else if ((whatToRun & (1 << 4)) != 0)
            {
                //prg.Show();
                prg.label1.Content = "Now running Mystique test...";
                totalString += "\n\n\n" + DateTime.Now.ToString() + "> Now Running Mystique\n";

                logString = "\n--------------------------------------------\nPowershell - Mistique:\n--------------------------------------------\n\n";
                File.AppendAllText( /*"..\\..\\ZBANG\\log.txt"*/logFileName, logString);

                try
                {
                    File.Delete("../../ZBANG/Mystique-Master/Results/delegation_info.csv");
                }
                catch { }

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"Powershell.exe",
                    Arguments = "-noprofile -ExecutionPolicy Bypass -command \"& { ./Mystique.ps1 }\"",
                    WorkingDirectory = "../../ZBANG/Mystique-Master",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var process = Process.Start(processStartInfo);
                process.OutputDataReceived += CaptureOutput;
                process.ErrorDataReceived += CaptureOutput;
                Thread runThread = new Thread(this.runThread);
                runThread.Start(process);
            }
        } // endfunc


        /**
         * @brief  Runs powershell within C# to get the different tools running
         * 
         * @param  string args - arguments to run
         * 
         **/
        void windowRunPowerShell(string args)
        {
            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                Directory.SetCurrentDirectory("../../ZBANG/ACLight-master");
                // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
                PowerShellInstance.AddScript("Set-ExecutionPolicy Bypass");
                string script = File.ReadAllText("ACLight.ps1");
                PowerShellInstance.AddScript(script);
                //PowerShellInstance.AddCommand("Import-Module").AddArgument("ACLight.psm1");
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

                string outputlog = "-------------------------------------------\n";
                foreach (PSObject outputItem in PSOutput)
                {
                }
                PowerShellInstance.Commands.Clear();
                PowerShellInstance.AddCommand("Start-ACLsAnalysis");

                IAsyncResult result = PowerShellInstance.BeginInvoke();

                // do something else until execution has completed.
                // this could be sleep/wait, or perhaps some other work
                while (result.IsCompleted == false)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                    // might want to place a timeout here...
                }
                Console.WriteLine("Finished!");

                outputlog += "-------------------------------------------\n";
                File.AppendAllText( /*"..\\..\\ZBANG\\log.txt"*/logFileName, "\n\nzBang Launched at " + DateTime.Now.ToString() + "\n" + outputlog + "\n");
            } // endusing powershell
        } // endfunc run powershell



        void CaptureOutput(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(delegate
            {
                totalString += e.Data + "\n";
                prg.textBlockPowershell.Text += e.Data + "\n";
                prg.textBlockPowershell.ScrollToEnd();
                File.AppendAllText( /*"..\\..\\ZBANG\\log.txt"*/logFileName, e.Data + "\n");


                prg.textBlockPowershell.Select(prg.textBlockPowershell.Text.Length, 0);

                prg.myScroll.LineDown();
                prg.myScroll.LineDown();
                prg.myScroll.LineDown();
                prg.myScroll.LineDown();
                prg.myScroll.LineDown();
                prg.textBlockPowershell.ScrollToEnd();
            }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// 

        string logString = "";
        public void runThread(object process)
        {
            Process prc = (Process)process;
            prc.BeginOutputReadLine();
            //prc.BeginErrorReadLine();

            while (!prc.HasExited)
            {
                if (quitApplication)
                {
                    prc.Close();
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        //prg.Close();
                    }));
                    return;
                }
            }
            //prc.WaitForExit();


            Dispatcher.BeginInvoke(new Action(delegate
            {
                //prg.Hide();
                string output = "";
                /*
                try
                {
                    output = prc.StandardOutput.ReadToEnd();
                }
                catch
                {
                }
                */

                if ((whatToRun & (1 << 0)) != 0)
                {
                    whatToRun = whatToRun & (~1);

                }
                else if ((whatToRun & (1 << 1)) != 0)
                {
                    whatToRun = whatToRun & (~2);
                    //logString += "Powershell - Skeleton Key:\n" + output + "\n\n\n";
                }
                else if ((whatToRun & (1 << 2)) != 0)
                {
                    whatToRun = whatToRun & (~4);
                    //logString += "Powershell - SID History:\n" + output + "\n\n\n";
                }
                else if ((whatToRun & (1 << 3)) != 0)
                {
                    whatToRun = whatToRun & (~(1 << 3));
                    //logString += "Powershell - RiskySPN:\n" + output + "\n\n\n";
                }
                else if ((whatToRun & (1 << 4)) != 0)
                {
                    whatToRun = whatToRun & (~(1 << 4));
                    //logString += "Powershell - Mystique:\n" + output + "\n\n\n";
                }
                if (whatToRun == 0) whatToRun--;
                LaunchZbang.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }));

            return;
#if zero
            if( (bool)toggleSkeleton.IsChecked )
            {
                prg.Show();
                prg.label1.Content = "Now running Skeleton Key test...";
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"Powershell.exe",
                    Arguments = "-ExecutionPolicy bypass ../../ZBANG/SkeletonKey_Scanner/SkeletonCheck.ps1",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var process = Process.Start( processStartInfo );
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                prg.Hide();
                System.Windows.MessageBox.Show( output, "Powershell - SkeletonKey" );
            }
            if( (bool)toggleSIDHistory.IsChecked )
            {
                prg.Show();
                prg.label1.Content = "Now running SID History test...";
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"Powershell.exe",
                    Arguments = "-noprofile -ExecutionPolicy Bypass Import-Module '../../ZBANG/SIDHistory/SIDHistory_Scanner.ps1' -force ; Report-UsersWithSIDHistory -Type 'CSV' -Path '../../ZBANG/SIDHistory/Results/' -DoNotOpen",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var process = Process.Start( processStartInfo );
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                prg.Hide();
                System.Windows.MessageBox.Show( output, "Powershell - SID History" );
            }
            if( (bool)toggleRisky.IsChecked )
            {
                prg.Show();
                prg.label1.Content = "Now running RiskySPN test...";
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"Powershell.exe",
                    Arguments = "-noprofile -ExecutionPolicy Bypass Import-Module '../../ZBANG/RiskySPN-Master/Get-PotentiallyCrackableAccounts.ps1' -force ; Report-PotentiallyCrackableAccounts -Type 'CSV' -DoNotOpen -Path '../../ZBANG/RiskySPN-master/Results' -Name 'RiskySPNs-test'",
                    /*-Type 'CSV' -Path '..\\..\\ZBANG\\RiskySPN-master\\Results\\' -DoNotOpen -Name 'RiskySPNs-test'",*/
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var process = Process.Start( processStartInfo );
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                prg.Hide();
                System.Windows.MessageBox.Show( output, "Powershell - RiskySPN" );
            }
            if( (bool)toggleMystique.IsChecked )
            {
                prg.Show();
                prg.label1.Content = "Now running Mystique test...";
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"Powershell.exe",
                    Arguments = "-noprofile -ExecutionPolicy Bypass Import-Module '../../ZBANG/Mystique-Master/Mystique.ps1' -force ; Find-DelegationAccounts",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                var process = Process.Start( processStartInfo );
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                prg.Hide();
                System.Windows.MessageBox.Show( output, "Powershell - Mystique" );
            }
#endif
        }

        /**
         * @brief   Called when the application window is quitting
         * 
         **/
        private void DataWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            quitApplication = true;
            System.Environment.Exit(0);
        }

        private void Export_clicked(object sender, RoutedEventArgs e)
        {
            //string getfile = null;
            /*
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if( openFileDialog.ShowDialog() == true )
                getfile = openFileDialog.FileName;
            else return;
            */
            string zipname = "../../../../ZBANG" + (DateTime.Now.Year - 2000).ToString("D02") + DateTime.Now.Month.ToString("D02") + DateTime.Now.Day.ToString("D02") + "-" + DateTime.Now.Hour.ToString("D02") + DateTime.Now.Minute.ToString("D02") + ".zip";
            if (File.Exists(zipname))
            {
                MessageBoxResult mbr = MessageBox.Show("Export file exists in directory.\nThis will overwrite it. Are you sure?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mbr == MessageBoxResult.No)
                    return;
                File.Delete(zipname);
            }
            {
                //extract the contents of the file we created
                //ZipFile.ExtractToDirectory( "../../zbang.zip", "../../ZBANG");            


                // move the scanner.zip file to another location, zip everything and then replace it back to its location
                try
                {
                    File.Copy("../../ZBANG/SkeletonKey_Scanner/scanner.zip", "../../scanner.zip");
                    File.Delete("../../ZBANG/SkeletonKey_Scanner/scanner.zip");
                }
                catch
                {
                    MessageBox.Show("v0.27 cannot move scanner.zip file...");
                }
                ZipFile.CreateFromDirectory("../../Zbang", zipname);

                try
                {
                    File.Copy("../../scanner.zip", "../../ZBANG/SkeletonKey_Scanner/scanner.zip");
                    File.Delete("../../scanner.zip");
                }
                catch
                {
                    MessageBox.Show("v0.27 cannot move scanner.zip file BACK...");
                }

                MessageBox.Show("Zbang File have been exported successfuly", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Import_clicked(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("This will overwrite current zBang Data. Are you sure?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.No)
            {
                runLaunchWindow();
                return;
            }

            string getfile = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select Import File";
            openFileDialog.Filter = "zBang Files|*.zip";
            if (openFileDialog.ShowDialog() == true)
                getfile = openFileDialog.FileName;
            else return;

            try
            {
                using (System.IO.Compression.ZipArchive zip1 = ZipFile.OpenRead(getfile))
                {
                    // here, we extract every entry, but we could extract conditionally
                    // based on entry name, size, date, checkbox status, etc.  
                    foreach (ZipArchiveEntry file in zip1.Entries)
                    {
                        if (file.Name == "")
                        {// Assuming Empty for Directory
                            continue;
                        }
                        file.ExtractToFile("../../ZBANG/" + file.FullName, true);
                    }
                }
                // extract the contents of the file we created
                //ZipFile.ExtractToDirectory( getfile, "../../ZBANG" );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // NS 03012018   ... . . . . . runLaunchWindow();

            toggleACLight.IsChecked = true;
            toggleSkeleton.IsChecked = true;
            toggleSIDHistory.IsChecked = true;
            toggleRisky.IsChecked = true;
            toggleMystique.IsChecked = true;
            this.Launch.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        } // endclass

        private void helpButton_clicked(object sender, RoutedEventArgs e)
        {
            if (rtbACL.Visibility == Visibility.Visible)
            {
                rtbACL.Visibility = Visibility.Hidden;
                return;
            }

            if (this.ViewModel.onScreen != MainWindowViewModel.ON_SCREEN.ACLLIGHT_ON_SCREEN &&
                this.ViewModel.onScreen != MainWindowViewModel.ON_SCREEN.DOMAIN_CARDS &&
                this.ViewModel.onScreen != MainWindowViewModel.ON_SCREEN.CARDS_ON_SCREEN_WITH_DOMAIN_SELECTION &&
                this.ViewModel.onScreen != MainWindowViewModel.ON_SCREEN.CARDS_ON_SCREEN_NO_DOMAIN_SELECTION)
                return;
            string curDir = Directory.GetCurrentDirectory();
            StreamReader sr = File.OpenText(String.Format("{0}/../../ZBANG/ACLight Attack Path Update.html", curDir));
            TextBox.Text = sr.ReadToEnd();
            sr.Close();
            rtbACL.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //zoomControl.ZoomXLoc = this.Width - 100;
            //MessageBox.Show("testing123");


            /* This has been removed at 01_01 22/11/2018
                        LicenseWindow eula = new LicenseWindow();
                        bool rsult = (bool)eula.ShowDialog();
                        if( !rsult)
                        {
                            System.Windows.Application.Current.Shutdown();
                            return;
                        }
            */
            runLaunchWindow();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
                zoomControl.ZoomXLoc = e.NewSize.Width - 100;
            if (e.HeightChanged)
                zoomControl.ZoomYLoc = e.NewSize.Height - 420;

        }

        //! WHEN MAGNIFYING GLASS IS CLICKED
        private void magnifyingGlassClicked(object sender, MouseButtonEventArgs e)
        {
            if (popupPicture.IsOpen)
            {
                popupPicture.IsOpen = false;
                return;
            }
            Image img = (Image)sender;
            Person person = (Person)img.DataContext;
            //person.Avatar = 
            popupPicture.IsOpen = true;
            popupPicture.PlacementTarget = (UIElement)sender;
            imgPopup.Source = person.thumbnail;

            // now, somehow show picture....
        }

        private void relaunch_clicked(object sender, RoutedEventArgs e)
        {
            runLaunchWindow();
        }

        private void modifyArgsByDomainSelections(ref string args, string scriptName)
        {
            List<string> domainNames;
            int count = viewModel.enumerateDomainInForest(out domainNames);
            if (count > 1)
            {
                domainSelection ds = new domainSelection(domainNames, scriptName);
                bool dialogresult = (bool)ds.ShowDialog();
                //
                if (ds.selection != -1)
                {
                    //"-ExecutionPolicy Bypass -noprofile -command \"Import-Module './ACLight.ps1' -force ; Start-ACLsAnalysis\""
                    //string test = args.Substring(args.Length - 3);

                    args = args.Substring(0, args.Length - 1) + " -Domain '" + domainNames[ds.selection] + "'\"";
                    // NO NEED !!!  MessageBox.Show( args, "error" );

                    //if (scriptName == "ACLight")
                    //{
                    //    args = args.Substring(0, args.Length - 1) + " -Domain '" + domainNames[ds.selection] + "'\"";
                    //    // NO NEED !!!  MessageBox.Show( args, "error" );
                    //}
                    //else
                    //{
                    //    //"-ExecutionPolicy Bypass -noprofile -Command \"& { ./SIDHistory_Scanner.ps1 }\""
                    //    args = args.Substring(0, args.Length - 2) + " -Domain '" + domainNames[ds.selection] + "'}\"";
                    //}
                }
            }
        }

    } // endclass

    //*****************************************************************************************************************
    // from codeprpoject: https://www.codeproject.com/Articles/1097390/Displaying-HTML-in-a-WPF-RichTextBox
    //*****************************************************************************************************************
    public class HtmlRichTextBoxBehavior : DependencyObject
    {
        public static readonly DependencyProperty TextProperty =
          DependencyProperty.RegisterAttached("Text", typeof(string),
          typeof(HtmlRichTextBoxBehavior), new UIPropertyMetadata(null, OnValueChanged));

        public static string GetText(RichTextBox o) { return (string)o.GetValue(TextProperty); }

        public static void SetText(RichTextBox o, string value) { o.SetValue(TextProperty, value); }

        private static void OnValueChanged(DependencyObject dependencyObject,
          DependencyPropertyChangedEventArgs e)
        {
            var richTextBox = (RichTextBox)dependencyObject;
            var text = (e.NewValue ?? string.Empty).ToString();
            var xaml = HtmlToXamlConverter.ConvertHtmlToXaml(text, true);
            var flowDocument = XamlReader.Parse(xaml) as FlowDocument;
            HyperlinksSubscriptions(flowDocument);
            richTextBox.Document = flowDocument;
        }

        private static void HyperlinksSubscriptions(FlowDocument flowDocument)
        {
            if (flowDocument == null) return;
            GetVisualChildren(flowDocument).OfType<Hyperlink>().ToList()
                     .ForEach(i => i.RequestNavigate += HyperlinkNavigate);
        }

        private static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject root)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
            {
                yield return child;
                foreach (var descendants in GetVisualChildren(child)) yield return descendants;
            }
        }

        private static void HyperlinkNavigate(object sender,
         System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    } // endclass

    public static class GetDotNetVersion
    {
        public static bool Get45PlusFromRegistry(string logFileName)
        {
            const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

            using (RegistryKey ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
            {
                if (ndpKey != null && ndpKey.GetValue("Release") != null)
                {
                    Console.WriteLine(".NET Framework Version: " + CheckFor45PlusVersion((int)ndpKey.GetValue("Release")));
                    File.AppendAllText(logFileName, ".NET Framework Version: " + CheckFor45PlusVersion((int)ndpKey.GetValue("Release")));
                    return true;
                }
                else
                {
                    Console.WriteLine(".NET Framework Version 4.5 or later is not detected.");
                    File.AppendAllText(logFileName, ".NET Framework Version 4.5 or later is not detected.");
                    return false;
                }
            }
        } // endfunc

        // Checking the version using >= will enable forward compatibility.
        private static string CheckFor45PlusVersion(int releaseKey)
        {
            if (releaseKey >= 460798)
                return "4.7 or later";
            if (releaseKey >= 394802)
                return "4.6.2";
            if (releaseKey >= 394254)
            {
                return "4.6.1";
            }
            if (releaseKey >= 393295)
            {
                return "4.6";
            }
            if ((releaseKey >= 379893))
            {
                return "4.5.2";
            }
            if ((releaseKey >= 378675))
            {
                return "4.5.1";
            }
            if ((releaseKey >= 378389))
            {
                return "4.5";
            }
            // This code should never execute. A non-null release key should mean
            // that 4.5 or later is installed.
            return "No 4.5 or later version detected";
        }
    }
    // This example displays output like the following:
    //       .NET Framework Version: 4.6.1





} // endnamespace

