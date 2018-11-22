using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ProgressWindow.xaml
    /// </summary>
    public partial class ProgressWindow : Window
    {
        public ProgressWindow()
        {
            InitializeComponent();
            label1.HorizontalContentAlignment = HorizontalAlignment.Center;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            progressBar1.IsIndeterminate = true;
        }

    }
}
