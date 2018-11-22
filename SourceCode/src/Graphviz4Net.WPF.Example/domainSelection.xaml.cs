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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Graphviz4Net.WPF.Example
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class domainSelection : Window
    {
        List<string>domainNames;
        public int selection = -1;                             // -1 == all, otherwise it is the index in domainNames List
        public domainSelection( List<string> inNames)
        {
            InitializeComponent();
            domainNames = inNames;
            foreach( string domainName in domainNames)
                listBoxPartialScan.Items.Add( domainName );

            // show the dialog box with animation...
            this.Visibility = Visibility.Visible;
#if zero
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0.0;
            myDoubleAnimation.To = 1.0;
            myDoubleAnimation.Duration = new Duration( TimeSpan.FromMilliseconds( 1000 ) );

            // Configure the animation to target the button's Width property.
            Storyboard.SetTargetName( myDoubleAnimation, this.Name );
            Storyboard.SetTargetProperty( myDoubleAnimation, new PropertyPath( this.op this.Opacity ) );

            // Create a storyboard to contain the animation.
            Storyboard myWidthAnimatedButtonStoryboard = new Storyboard();
            myWidthAnimatedButtonStoryboard.Children.Add( myDoubleAnimation );
            myWidthAnimatedButtonStoryboard.Begin( this );
#endif

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            selection = -1;
            this.Close();
        }

        private void fadeCompleted(object sender, EventArgs e)
        {

        }

        private void listBoxPartialScan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            buttonPartial.IsEnabled = true;
            selection = listBoxPartialScan.SelectedIndex;
        }

        private void buttonPartial_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }
    }
}
