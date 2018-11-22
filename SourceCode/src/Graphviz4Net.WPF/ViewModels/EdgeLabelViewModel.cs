
namespace Graphviz4Net.WPF.ViewModels
{
    using Graphs;

    public class EdgeLabelViewModel
    {
        public EdgeLabelViewModel(string label, IEdge edge)
        {
            this.Label = label;
            this.Edge = edge;
            this.labelToolTip = "";
            this.labelColor = edge.edgeColor;
        }
        public EdgeLabelViewModel(string label, string toolTip, IEdge edge)
        {
            this.Label = label;
            this.Edge = edge;
            this.labelToolTip = toolTip;
            this.labelColor = edge.edgeColor;
        }

        public string Label { get; private set; }
        public string labelToolTip { get; private set; }
        
        public string labelColor { get; private set; }

        public IEdge Edge { get; private set; }
    }
}
