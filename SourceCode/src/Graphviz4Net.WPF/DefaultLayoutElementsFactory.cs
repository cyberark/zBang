
namespace Graphviz4Net.WPF
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using ViewModels;

    internal class DefaultLayoutElementsFactory : IWPFLayoutElementsFactory
    {
        public FrameworkElement CreateVertex(object vertex)
        {
            return new ContentPresenter { Content = vertex };
        }

        public FrameworkElement CreateEdgeArrow(EdgeArrowViewModel arrowViewModel)
        {
            if (arrowViewModel.Arrow == null)
            {
                return new Line
                {
                    X1 = 0,
                    Y1 = 0,
                    X2 = 0,
                    Y2 = 10,
                    Stroke =
                        new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
            }

            return new ContentPresenter { Content = arrowViewModel.Arrow };
        }

        public FrameworkElement CreateSubGraphBorder(BorderViewModel borderViewModel)
        {
            return new Border
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(10, 0, 0, 0),
                Child = new TextBlock { Text = borderViewModel.Label }
            };
        }

        public FrameworkElement CreateEdge(EdgeViewModel edgeViewModel)
        {
            return new Path
            {
                Data = edgeViewModel.Data,
                Stroke = new SolidColorBrush( Colors.Blue /*edgeViewModel.color*/),
                StrokeThickness = 1,
            };
        }

        public FrameworkElement CreateEdgeLabel(EdgeLabelViewModel labelViewModel)
        {
            return new TextBlock { Text = labelViewModel.Label };
        }

        public FrameworkElement CreateEdgeArrowLabel(EdgeArrowLabelViewModel labelViewModel)
        {
            return new TextBlock { Text = labelViewModel.Label };
        }
    }
}
