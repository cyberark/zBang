
namespace Graphviz4Net.WPF
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using ViewModels;

    class ContentPresenterFactory : IWPFLayoutElementsFactory
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
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };
            }
            else
            {
                return new ContentPresenter { Content = arrowViewModel.Arrow };                
            }
        }

        public FrameworkElement CreateSubGraphBorder(BorderViewModel borderViewModel)
        {
            return new ContentPresenter { Content = borderViewModel };
        }

        public FrameworkElement CreateEdge(EdgeViewModel edgeViewModel)
        {
            return new ContentPresenter { Content = edgeViewModel };
        }

        public FrameworkElement CreateEdgeLabel(EdgeLabelViewModel labelViewModel)
        {
            return new ContentPresenter { Content = labelViewModel };
        }

        public FrameworkElement CreateEdgeArrowLabel(EdgeArrowLabelViewModel labelViewModel)
        {
            return new ContentPresenter { Content = labelViewModel };
        }
    }
}
