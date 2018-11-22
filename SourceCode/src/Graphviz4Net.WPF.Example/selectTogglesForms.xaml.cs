using System;
using System.Collections.Generic;
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
    /// Interaction logic for selectTogglesForms.xaml
    /// </summary>
    public partial class selectTogglesForms : Window
    {
        public int whoIsSelected = 0, anyselected = 0;

        public int isLaunched = 0;          // 1=launch, 0 = reload, 2 = import

        public selectTogglesForms()
        {
            InitializeComponent();

            //string ImagesPath = "pack://application:,,/Graphviz4Net.WPF.Example;component/Images/aetosdios_trans.png";
            string ImagesPath = "./Images/flashlight.png";
            Uri uri = new Uri( ImagesPath, UriKind.RelativeOrAbsolute );
            BitmapImage bitmap = new BitmapImage( uri );
            imgACLight.Source = bitmap;
            imgSkeleton.Source = new BitmapImage( new Uri( @"Images/key.png", UriKind.Relative ) );
            imgRisky.Source = new BitmapImage( new Uri( @"./Images/clerk.png", UriKind.Relative ) );
            imgSID.Source = new BitmapImage( new Uri( @"./Images/theater.png", UriKind.Relative ) );
            imgMistique.Source = new BitmapImage( new Uri( @"./Images/role-playing-game.png", UriKind.Relative ) );

            zbangimg.Source = new BitmapImage( new Uri( @"./Images/bang2.png", UriKind.Relative ) );

            this.buttonReload.IsEnabled = false;
            this.buttonLaunch.IsEnabled = false;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            whoIsSelected = 0;
            if( (bool)toggleACLight.IsChecked )
                whoIsSelected |= (1 << (int)MainWindow.TABITEMS.ACLLIGHT);
            if( (bool)this.toggleMystique.IsChecked )
                whoIsSelected |= (1 << (int)MainWindow.TABITEMS.MYSTIQUE);
            if( (bool)this.toggleRisky.IsChecked )
                whoIsSelected |= (1 << (int)MainWindow.TABITEMS.RISKYSPNS);
            if( (bool)this.toggleSIDHistory.IsChecked )
                whoIsSelected |= (1 << (int)MainWindow.TABITEMS.SIDHISTORY);
            if( (bool)this.toggleSkeleton.IsChecked )
                whoIsSelected |= (1 << (int)MainWindow.TABITEMS.SKELETONKEY);
            anyselected++;
            if( anyselected > 0 )
            {
                buttonLaunch.IsEnabled = true;
                buttonReload.IsEnabled = true;
            }
        }

        private void ToggleButton_UnChecked(object sender, RoutedEventArgs e)
        {
            anyselected--;
            if( anyselected == 0 )
            {
                buttonLaunch.IsEnabled = false;
                buttonReload.IsEnabled = false;
            }
        }

        private void buttonLaunch_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            isLaunched = 1;
        }

        private void buttonReload_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            isLaunched = 0;

        }

        private void buttonImport_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            isLaunched = 2;
        }
    }
}
