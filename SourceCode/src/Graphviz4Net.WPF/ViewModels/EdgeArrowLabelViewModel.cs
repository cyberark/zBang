
namespace Graphviz4Net.WPF.ViewModels
{
    using Graphs;

    public class EdgeArrowLabelViewModel
    {
        public EdgeArrowLabelViewModel(string label, IEdge edge, object arrow)
        {
            this.Label = label;
            this.Edge = edge;
            this.Arrow = arrow;
        }

        public string Label { get; private set; }

        public IEdge Edge { get; private set; }

        public object Arrow { get; private set; }
    }
}
