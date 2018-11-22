using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Graphviz4Net.WPF.Example
{
    /// <summary>
    /// Interaction logic for LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        public LicenseWindow()
        {
            InitializeComponent();

            this.ResizeMode = ResizeMode.NoResize;

            string ImagesPath = "pack://application:,,/Graphviz4Net.WPF.Example;component/Images/flash_bang1600.png";
            Uri uri = new Uri(ImagesPath, UriKind.RelativeOrAbsolute);
            ImageBrush bitmap = new ImageBrush( new BitmapImage(uri));
            rtbLicense.Background = bitmap;
            rtbLicense.Background.Opacity = 0.08;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string curDir = Directory.GetCurrentDirectory();
            //StreamReader sr = File.OpenText( String.Format( "{0}/../../ZBANG/ACLight Attack Path Update.html", curDir ) );
            StreamReader sr = File.OpenText(String.Format("{0}/../../ZBANG/ACLight-master/Zbang EULA.html", curDir));
            TextBox.Text = sr.ReadToEnd();
            sr.Close();

        }

        private void buttonDecline_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void buttonAgree_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }
    } // endclass

}
