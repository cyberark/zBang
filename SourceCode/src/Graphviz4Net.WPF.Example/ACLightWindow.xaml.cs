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
    using Graphs;
    using System.ComponentModel;

    /// <summary>
    /// Interaction logic for ACLightWindow.xaml
    /// </summary>
    public partial class ACLightWindow : Window
    {
        MainWindowViewModel mainWnd;
        public Graph<INotifyPropertyChanged> Graph { get; private set; }
        public ACLightWindow(MainWindowViewModel a)
        {
            mainWnd = a;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Person person = mainWnd.nameToPerson["RESEARCH\\Emily"];

            var graph2 = new Graph<INotifyPropertyChanged>();
            string searchName = person.Name;
            graph2.AddVertex(mainWnd.nameToPerson[searchName]);
            graph2.AddVertex(mainWnd.nameToPerson["RESEARCH\\andy"]);
            graph2.AddEdge(new Edge<INotifyPropertyChanged>(mainWnd.nameToPerson[searchName], mainWnd.nameToPerson["RESEARCH\\andy"], new Arrow()) { Label = "WriteDacl" });

            this.Graph = graph2;
        }
    }
}
