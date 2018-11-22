
using Graphviz4Net.Graphs;

namespace Graphviz4Net.WPF.ViewModels
{
    public class BorderViewModel
    {
        public BorderViewModel(string label, ISubGraph subGraph)
        {
            this.Label = label;
            this.SubGraph = subGraph;
            borderColor = subGraph.borderColor;
            borderThickness = subGraph.borderThickness;
        }

        public string Label { get; private set; }

        public ISubGraph SubGraph { get; private set; }

        public string borderColor { get; set; }
        public string borderThickness { get; set; }
    }
}
