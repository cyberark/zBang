
namespace Graphviz4Net.WPF
{
    using System.Windows;
    using ViewModels;

    public interface IWPFLayoutElementsFactory
    {
        FrameworkElement CreateVertex(object vertex);

        FrameworkElement CreateEdgeArrow(EdgeArrowViewModel arrowViewModel);

        FrameworkElement CreateSubGraphBorder(BorderViewModel borderViewModel);

        FrameworkElement CreateEdge(EdgeViewModel edgeViewModel);

        FrameworkElement CreateEdgeLabel(EdgeLabelViewModel labelViewModel);

        FrameworkElement CreateEdgeArrowLabel(EdgeArrowLabelViewModel labelViewModel);
    }
}
