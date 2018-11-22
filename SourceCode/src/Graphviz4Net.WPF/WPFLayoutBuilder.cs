
namespace Graphviz4Net.WPF
{
    using System.Globalization;
    using System.Windows.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Dot;
    using Graphs;
    using ViewModels;

    /// <summary>
    /// Builds the graph layout out of WPF elements, which are positioned into the WPF canvas.
    /// The WPF elements used are determined by the <see cref="IWPFLayoutElementsFactory"/> abstract factory object.
    /// </summary>
    public class WPFLayoutBuilder : LayoutBuilder<int>
    {
        private readonly Canvas canvas;

        private readonly IWPFLayoutElementsFactory elementsFactory;

        private readonly IDictionary<object, FrameworkElement> verticesElements =
            new Dictionary<object, FrameworkElement>();

        private readonly IDictionary<object, Size> verticesSizes =
            new Dictionary<object, Size>();

        private IGraph graph;

        public WPFLayoutBuilder(Canvas canvas, IWPFLayoutElementsFactory elementsFactory)
        {
            this.canvas = canvas;
            this.elementsFactory = elementsFactory;
        }

        public override void Start(IGraph graph)
        {
            this.graph = graph;
            foreach (var vertex in graph.GetAllVertices())
            {
                var element = this.elementsFactory.CreateVertex(vertex);
                if (element == null)
                {
                    throw new InvalidOperationException(
                        string.Format("WPFLayoutBuilder.GetSize: for vertex {0} WPF control factory returned null.", vertex));
                }

                // add it to canvas:
                this.canvas.Children.Add(element);
                this.verticesElements.Add(vertex, element);

                // measure it:
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double width = element.DesiredSize.Width * (1.0 / 72.0);
                double height = element.DesiredSize.Height * (1.0 / 72.0);
                this.verticesSizes.Add(vertex, new Size(width, height));

                // hide it till we finish:
#if SILVERLIGHT
                element.Visibility = Visibility.Collapsed;
#else
                element.Visibility = Visibility.Hidden;
#endif
            }
        }

        public override Size GetSize(object vertex)
        {
            return this.verticesSizes[vertex];
        }

        public override void Finish()
        {
            foreach (var element in this.verticesElements)
            {
                element.Value.Visibility = Visibility.Visible;
            }
        }

        public override void BuildGraph(double width, double height, IGraph original, DotGraph<int> dotGraph)
        {
            this.canvas.Width = width;
            this.canvas.Height = height;
        }

        public override void BuildVertex(Point position, double width, double height, object originalVertex, DotVertex<int> dotVertex)
        {
            var element = this.verticesElements[originalVertex];
            UpdatePosition(element, position.X, position.Y, width, height, canvas);
#if !SILVERLIGHT
            Panel.SetZIndex(element, 1);    // TODO: implement this functionality for Silverlight
#endif
        }

        public override void BuildSubGraph(
            double leftX,
            double upperY,
            double rightX,
            double lowerY,
            ISubGraph originalSubGraph,
            DotSubGraph<int> subGraph)
        {
            var element = this.elementsFactory.CreateSubGraphBorder(new BorderViewModel(subGraph.Label, originalSubGraph));
            canvas.Children.Add(element);
            element.Width = rightX - leftX;
            element.Height = upperY - lowerY;
            Point orig = new Point(leftX, upperY);
            Point p = TransformCoordinates(orig, canvas);
            Canvas.SetLeft(element, p.X);
            Canvas.SetTop(element, p.Y);

#if !SILVERLIGHT
            Panel.SetZIndex(element, -1);   // TODO: implement this functionality for silverlight
#endif
            this.verticesElements.Add(originalSubGraph, element);
        }

        public override void BuildEdge(Point[] path, IEdge originalEdge, DotEdge<int> edge)
        {            
#if SILVERLIGHT
            var data = new StringToPathGeometryConverter().Convert(TransformCoordinates(path, this.canvas));
#else
            var data = Geometry.Parse(TransformCoordinates(path, this.canvas));
#endif
            var edgeElement = this.elementsFactory.CreateEdge(new EdgeViewModel(data, originalEdge));
            if(edge.Label != "blind")
                this.canvas.Children.Add(edgeElement);

            if (edge.DestinationArrowEnd.HasValue)
            {
                CreateArrow(
                    TransformCoordinates(edge.DestinationArrowEnd.Value, canvas),
                    TransformCoordinates(path.Last(), canvas),
                    edge.Destination,
                    this.elementsFactory.CreateEdgeArrow(new EdgeArrowViewModel(originalEdge, originalEdge.DestinationArrow)),
                    this.canvas);
            }

            if (edge.SourceArrowEnd.HasValue)
            {
                CreateArrow(
                    TransformCoordinates(edge.SourceArrowEnd.Value, canvas),
                    TransformCoordinates(path.First(), canvas),
                    edge.Source,
                    this.elementsFactory.CreateEdgeArrow(new EdgeArrowViewModel(originalEdge, originalEdge.SourceArrow)),
                    this.canvas);
            }

            if (edge.LabelPos.HasValue && edge.Label != "blind")
            {
                var labelElement = this.elementsFactory.CreateEdgeLabel(new EdgeLabelViewModel(edge.Label, edge.labelToolTip, originalEdge));
                this.CreateLabel(edge.LabelPos.Value, labelElement);
            }

            if (edge.SourceArrowLabelPosition.HasValue)
            {
                var viewModel = new EdgeArrowLabelViewModel(edge.SourceArrowLabel, originalEdge, edge.SourceArrow);
                var labelElement = this.elementsFactory.CreateEdgeArrowLabel(viewModel);
                this.CreateLabel(edge.SourceArrowLabelPosition.Value, labelElement);
            }

            if (edge.DestinationArrowLabelPosition.HasValue)
            {
                var viewModel = new EdgeArrowLabelViewModel(edge.DestinationArrowLabel, originalEdge, originalEdge.DestinationArrow);
                var labelElement = this.elementsFactory.CreateEdgeArrowLabel(viewModel);
                this.CreateLabel(edge.DestinationArrowLabelPosition.Value, labelElement);
            }
        }

        private void CreateLabel(Point pos, FrameworkElement label)
        {
            this.canvas.Children.Add(label);
            label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Point p = TransformCoordinates(pos, canvas);
            Canvas.SetLeft(label, p.X - label.DesiredSize.Width / 2);
            Canvas.SetTop(label, p.Y - label.DesiredSize.Height / 2);
        }

        private static void CreateArrow(
            Point arrowCorner1,
            Point arrowCorner2,
            DotVertex<int> edgeDestination,
            FrameworkElement edgeArrow,
            Canvas canvas)
        {
            Point arrowStart, border;
            if (IsOnBoder(edgeDestination.Position.Value, edgeDestination.Width.Value, edgeDestination.Height.Value, arrowCorner2))
            {
                arrowStart = arrowCorner1;
                border = arrowCorner2;
            }
            else
            {
                arrowStart = arrowCorner2;
                border = arrowCorner1;
            }

            double width = arrowStart.X - border.X;
            double height = border.Y - arrowStart.Y;
            double angle = (180 / Math.PI) * Math.Atan2(width, height);

            var arrowControl = edgeArrow;
            canvas.Children.Add(arrowControl);
            arrowControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double halfWidth = Math.Floor(arrowControl.DesiredSize.Width / 2);

#if SILVERLIGHT
            arrowControl.RenderTransform = new RotateTransform { Angle = angle, CenterX = halfWidth, CenterY = 0 };
#else
            arrowControl.RenderTransform = new RotateTransform(angle, halfWidth, 0);
#endif

            Canvas.SetTop(arrowControl, arrowStart.Y);
            Canvas.SetLeft(arrowControl, arrowStart.X - halfWidth);
        }

        private static bool IsOnBoder(Point leftBottom, double width, double height, Point point)
        {
            return
                (Utils.AreEqual(leftBottom.Y, point.Y) && leftBottom.X <= point.X && point.X <= leftBottom.X + width) ||
                (Utils.AreEqual(leftBottom.X, point.X) && leftBottom.Y <= point.Y && point.Y <= leftBottom.Y + height) ||
                (Utils.AreEqual(leftBottom.Y + height, point.Y) && leftBottom.X <= point.X && point.X <= leftBottom.X + width) ||
                (Utils.AreEqual(leftBottom.X + width, point.X) && leftBottom.Y <= point.Y && point.Y <= leftBottom.Y + height);
        }

        private static double GetSizeInPoints(double value)
        {
            return value * 72;
        }

        private static void UpdatePosition(UIElement element, double x, double y, double width, double height, Canvas canvas)
        {
            double widthInPoints = GetSizeInPoints(width);
            double heightInPoints = GetSizeInPoints(height);
            Canvas.SetLeft(element, x - widthInPoints / 2);
            Canvas.SetTop(element, canvas.Height - (y + heightInPoints / 2));
        }

        private static Point TransformCoordinates(Point p, Canvas canvas)
        {
            return new Point(p.X, canvas.Height - p.Y);
        }

        private static string TransformCoordinates(Point[] data, Canvas canvas)
        {
            var transformed = data.Select(x => TransformCoordinates(x, canvas))
                .Select(p => p.X.ToInvariantString() + "," + p.Y.ToInvariantString());
            return "M" + transformed.First() + "C" +
                   string.Join(" ", transformed.Skip(1).ToArray());
        }
    }

#if SILVERLIGHT
    /// <summary>
    /// This class was taken from http://stringtopathgeometry.codeplex.com/
    /// </summary>
    internal class StringToPathGeometryConverter : IValueConverter
    {
        #region Const & Private Variables
        const bool AllowSign = true;
        const bool AllowComma = true;
        const bool IsFilled = true;
        const bool IsClosed = true;

        IFormatProvider _formatProvider;

        PathFigure _figure = null;     // Figure object, which will accept parsed segments
        string _pathString;        // Input string to be parsed
        int _pathLength;
        int _curIndex;          // Location to read next character from
        bool _figureStarted;     // StartFigure is effective 

        Point _lastStart;         // Last figure starting point
        Point _lastPoint;         // Last point 
        Point _secondLastPoint;   // The point before last point

        char _token;             // Non whitespace character returned by ReadToken
        #endregion

        #region Public Functionality
        /// <summary>
        /// Main conversion routine - converts string path data definition to PathGeometry object
        /// </summary>
        /// <param name="path">String with path data definition</param>
        /// <returns>PathGeometry object created from string definition</returns>
        public PathGeometry Convert(string path)
        {
            if (null == path)
                throw new ArgumentException("Path string cannot be null!");

            if (path.Length == 0)
                throw new ArgumentException("Path string cannot be empty!");

            return parse(path);
        }

        /// <summary>
        /// Main back conversion routine - converts PathGeometry object to its string equivalent
        /// </summary>
        /// <param name="geometry">Path Geometry object</param>
        /// <returns>String equivalent to PathGeometry contents</returns>
        public string ConvertBack(PathGeometry geometry)
        {
            if (null == geometry)
                throw new ArgumentException("Path Geometry cannot be null!");

            return parseBack(geometry);
        }
        #endregion

        #region Private Functionality
        /// <summary>
        /// Main parser routine, which loops over each char in received string, and performs actions according to command/parameter being passed
        /// </summary>
        /// <param name="path">String with path data definition</param>
        /// <returns>PathGeometry object created from string definition</returns>
        private PathGeometry parse(string path)
        {
            PathGeometry _pathGeometry = null;


            _formatProvider = CultureInfo.InvariantCulture;
            _pathString = path;
            _pathLength = path.Length;
            _curIndex = 0;

            _secondLastPoint = new Point(0, 0);
            _lastPoint = new Point(0, 0);
            _lastStart = new Point(0, 0);

            _figureStarted = false;

            bool first = true;

            char last_cmd = ' ';

            while (ReadToken()) // Empty path is allowed in XAML
            {
                char cmd = _token;

                if (first)
                {
                    if ((cmd != 'M') && (cmd != 'm') && (cmd != 'f') && (cmd != 'F'))  // Path starts with M|m 
                    {
                        ThrowBadToken();
                    }

                    first = false;
                }

                switch (cmd)
                {
                    case 'f':
                    case 'F':
                        _pathGeometry = new PathGeometry();
                        double _num = ReadNumber(!AllowComma);
                        _pathGeometry.FillRule = _num == 0 ? FillRule.EvenOdd : FillRule.Nonzero;
                        break;

                    case 'm':
                    case 'M':
                        // XAML allows multiple points after M/m
                        _lastPoint = ReadPoint(cmd, !AllowComma);

                        _figure = new PathFigure();
                        _figure.StartPoint = _lastPoint;
                        _figure.IsFilled = IsFilled;
                        _figure.IsClosed = !IsClosed;
                        //context.BeginFigure(_lastPoint, IsFilled, !IsClosed);
                        _figureStarted = true;
                        _lastStart = _lastPoint;
                        last_cmd = 'M';

                        while (IsNumber(AllowComma))
                        {
                            _lastPoint = ReadPoint(cmd, !AllowComma);

                            LineSegment _lineSegment = new LineSegment();
                            _lineSegment.Point = _lastPoint;
                            _figure.Segments.Add(_lineSegment);
                            //context.LineTo(_lastPoint, IsStroked, !IsSmoothJoin);
                            last_cmd = 'L';
                        }
                        break;

                    case 'l':
                    case 'L':
                    case 'h':
                    case 'H':
                    case 'v':
                    case 'V':
                        EnsureFigure();

                        do
                        {
                            switch (cmd)
                            {
                                case 'l': _lastPoint = ReadPoint(cmd, !AllowComma); break;
                                case 'L': _lastPoint = ReadPoint(cmd, !AllowComma); break;
                                case 'h': _lastPoint.X += ReadNumber(!AllowComma); break;
                                case 'H': _lastPoint.X = ReadNumber(!AllowComma); break;
                                case 'v': _lastPoint.Y += ReadNumber(!AllowComma); break;
                                case 'V': _lastPoint.Y = ReadNumber(!AllowComma); break;
                            }

                            LineSegment _lineSegment = new LineSegment();
                            _lineSegment.Point = _lastPoint;
                            _figure.Segments.Add(_lineSegment);
                            //context.LineTo(_lastPoint, IsStroked, !IsSmoothJoin);
                        }
                        while (IsNumber(AllowComma));

                        last_cmd = 'L';
                        break;

                    case 'c':
                    case 'C': // cubic Bezier 
                    case 's':
                    case 'S': // smooth cublic Bezier
                        EnsureFigure();

                        do
                        {
                            Point p;

                            if ((cmd == 's') || (cmd == 'S'))
                            {
                                if (last_cmd == 'C')
                                {
                                    p = Reflect();
                                }
                                else
                                {
                                    p = _lastPoint;
                                }

                                _secondLastPoint = ReadPoint(cmd, !AllowComma);
                            }
                            else
                            {
                                p = ReadPoint(cmd, !AllowComma);

                                _secondLastPoint = ReadPoint(cmd, AllowComma);
                            }

                            _lastPoint = ReadPoint(cmd, AllowComma);

                            BezierSegment _bizierSegment = new BezierSegment();
                            _bizierSegment.Point1 = p;
                            _bizierSegment.Point2 = _secondLastPoint;
                            _bizierSegment.Point3 = _lastPoint;
                            _figure.Segments.Add(_bizierSegment);
                            //context.BezierTo(p, _secondLastPoint, _lastPoint, IsStroked, !IsSmoothJoin);

                            last_cmd = 'C';
                        }
                        while (IsNumber(AllowComma));

                        break;

                    case 'q':
                    case 'Q': // quadratic Bezier 
                    case 't':
                    case 'T': // smooth quadratic Bezier
                        EnsureFigure();

                        do
                        {
                            if ((cmd == 't') || (cmd == 'T'))
                            {
                                if (last_cmd == 'Q')
                                {
                                    _secondLastPoint = Reflect();
                                }
                                else
                                {
                                    _secondLastPoint = _lastPoint;
                                }

                                _lastPoint = ReadPoint(cmd, !AllowComma);
                            }
                            else
                            {
                                _secondLastPoint = ReadPoint(cmd, !AllowComma);
                                _lastPoint = ReadPoint(cmd, AllowComma);
                            }

                            QuadraticBezierSegment _quadraticBezierSegment = new QuadraticBezierSegment();
                            _quadraticBezierSegment.Point1 = _secondLastPoint;
                            _quadraticBezierSegment.Point2 = _lastPoint;
                            _figure.Segments.Add(_quadraticBezierSegment);
                            //context.QuadraticBezierTo(_secondLastPoint, _lastPoint, IsStroked, !IsSmoothJoin);

                            last_cmd = 'Q';
                        }
                        while (IsNumber(AllowComma));

                        break;

                    case 'a':
                    case 'A':
                        EnsureFigure();

                        do
                        {
                            // A 3,4 5, 0, 0, 6,7
                            double w = ReadNumber(!AllowComma);
                            double h = ReadNumber(AllowComma);
                            double rotation = ReadNumber(AllowComma);
                            bool large = ReadBool();
                            bool sweep = ReadBool();

                            _lastPoint = ReadPoint(cmd, AllowComma);

                            ArcSegment _arcSegment = new ArcSegment();
                            _arcSegment.Point = _lastPoint;
                            _arcSegment.Size = new Size(w, h);
                            _arcSegment.RotationAngle = rotation;
                            _arcSegment.IsLargeArc = large;
                            _arcSegment.SweepDirection = sweep ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                            _figure.Segments.Add(_arcSegment);
                            //context.ArcTo(
                            //    _lastPoint,
                            //    new Size(w, h),
                            //    rotation,
                            //    large,
                            //    sweep ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                            //    IsStroked,
                            //    !IsSmoothJoin
                            //    );
                        }
                        while (IsNumber(AllowComma));

                        last_cmd = 'A';
                        break;

                    case 'z':
                    case 'Z':
                        EnsureFigure();
                        _figure.IsClosed = IsClosed;
                        //context.SetClosedState(IsClosed);

                        _figureStarted = false;
                        last_cmd = 'Z';

                        _lastPoint = _lastStart; // Set reference point to be first point of current figure
                        break;

                    default:
                        ThrowBadToken();
                        break;
                }

                if (null != _figure)
                {
                    if (_figure.IsClosed)
                    {
                        if (null == _pathGeometry)
                            _pathGeometry = new PathGeometry();

                        _pathGeometry.Figures.Add(_figure);

                        _figure = null;
                        first = true;
                    }
                }


            }

            if (null != _figure)
            {
                if (null == _pathGeometry)
                    _pathGeometry = new PathGeometry();

                if (!_pathGeometry.Figures.Contains(_figure))
                    _pathGeometry.Figures.Add(_figure);

            }
            return _pathGeometry;
        }

        void SkipDigits(bool signAllowed)
        {
            // Allow for a sign 
            if (signAllowed && More() && ((_pathString[_curIndex] == '-') || _pathString[_curIndex] == '+'))
            {
                _curIndex++;
            }

            while (More() && (_pathString[_curIndex] >= '0') && (_pathString[_curIndex] <= '9'))
            {
                _curIndex++;
            }
        }

        bool ReadBool()
        {
            SkipWhiteSpace(AllowComma);

            if (More())
            {
                _token = _pathString[_curIndex++];

                if (_token == '0')
                {
                    return false;
                }
                else if (_token == '1')
                {
                    return true;
                }
            }

            ThrowBadToken();

            return false;
        }

        private Point Reflect()
        {
            return new Point(2 * _lastPoint.X - _secondLastPoint.X,
                             2 * _lastPoint.Y - _secondLastPoint.Y);
        }

        private void EnsureFigure()
        {
            if (!_figureStarted)
            {
                _figure = new PathFigure();
                _figure.StartPoint = _lastStart;

                //_context.BeginFigure(_lastStart, IsFilled, !IsClosed);
                _figureStarted = true;
            }
        }

        double ReadNumber(bool allowComma)
        {
            if (!IsNumber(allowComma))
            {
                ThrowBadToken();
            }

            bool simple = true;
            int start = _curIndex;

            //
            // Allow for a sign
            //
            // There are numbers that cannot be preceded with a sign, for instance, -NaN, but it's 
            // fine to ignore that at this point, since the CLR parser will catch this later.
            // 
            if (More() && ((_pathString[_curIndex] == '-') || _pathString[_curIndex] == '+'))
            {
                _curIndex++;
            }

            // Check for Infinity (or -Infinity).
            if (More() && (_pathString[_curIndex] == 'I'))
            {
                // 
                // Don't bother reading the characters, as the CLR parser will 
                // do this for us later.
                // 
                _curIndex = Math.Min(_curIndex + 8, _pathLength); // "Infinity" has 8 characters
                simple = false;
            }
            // Check for NaN 
            else if (More() && (_pathString[_curIndex] == 'N'))
            {
                // 
                // Don't bother reading the characters, as the CLR parser will
                // do this for us later. 
                //
                _curIndex = Math.Min(_curIndex + 3, _pathLength); // "NaN" has 3 characters
                simple = false;
            }
            else
            {
                SkipDigits(!AllowSign);

                // Optional period, followed by more digits 
                if (More() && (_pathString[_curIndex] == '.'))
                {
                    simple = false;
                    _curIndex++;
                    SkipDigits(!AllowSign);
                }

                // Exponent
                if (More() && ((_pathString[_curIndex] == 'E') || (_pathString[_curIndex] == 'e')))
                {
                    simple = false;
                    _curIndex++;
                    SkipDigits(AllowSign);
                }
            }

            if (simple && (_curIndex <= (start + 8))) // 32-bit integer
            {
                int sign = 1;

                if (_pathString[start] == '+')
                {
                    start++;
                }
                else if (_pathString[start] == '-')
                {
                    start++;
                    sign = -1;
                }

                int value = 0;

                while (start < _curIndex)
                {
                    value = value * 10 + (_pathString[start] - '0');
                    start++;
                }

                return value * sign;
            }
            else
            {
                string subString = _pathString.Substring(start, _curIndex - start);

                try
                {
                    return System.Convert.ToDouble(subString, _formatProvider);
                }
                catch (FormatException except)
                {
                    throw new FormatException(string.Format("Unexpected character in path '{0}' at position {1}", _pathString, _curIndex - 1), except);
                }
            }
        }

        private bool IsNumber(bool allowComma)
        {
            bool commaMet = SkipWhiteSpace(allowComma);

            if (More())
            {
                _token = _pathString[_curIndex];

                // Valid start of a number
                if ((_token == '.') || (_token == '-') || (_token == '+') || ((_token >= '0') && (_token <= '9'))
                    || (_token == 'I')  // Infinity
                    || (_token == 'N')) // NaN 
                {
                    return true;
                }
            }

            if (commaMet) // Only allowed between numbers
            {
                ThrowBadToken();
            }

            return false;
        }

        private Point ReadPoint(char cmd, bool allowcomma)
        {
            double x = ReadNumber(allowcomma);
            double y = ReadNumber(AllowComma);

            if (cmd >= 'a') // 'A' < 'a'. lower case for relative
            {
                x += _lastPoint.X;
                y += _lastPoint.Y;
            }

            return new Point(x, y);
        }

        private bool ReadToken()
        {
            SkipWhiteSpace(!AllowComma);

            // Check for end of string 
            if (More())
            {
                _token = _pathString[_curIndex++];

                return true;
            }
            else
            {
                return false;
            }
        }

        bool More()
        {
            return _curIndex < _pathLength;
        }

        // Skip white space, one comma if allowed
        private bool SkipWhiteSpace(bool allowComma)
        {
            bool commaMet = false;

            while (More())
            {
                char ch = _pathString[_curIndex];

                switch (ch)
                {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t': // SVG whitespace 
                        break;

                    case ',':
                        if (allowComma)
                        {
                            commaMet = true;
                            allowComma = false; // one comma only
                        }
                        else
                        {
                            ThrowBadToken();
                        }
                        break;

                    default:
                        // Avoid calling IsWhiteSpace for ch in (' ' .. 'z']
                        if (((ch > ' ') && (ch <= 'z')) || !Char.IsWhiteSpace(ch))
                        {
                            return commaMet;
                        }
                        break;
                }

                _curIndex++;
            }

            return commaMet;
        }

        private void ThrowBadToken()
        {
            throw new FormatException(string.Format("Unexpected character in path '{0}' at position {1}", _pathString, _curIndex - 1));
        }

        static internal char GetNumericListSeparator(IFormatProvider provider)
        {
            char numericSeparator = ',';

            // Get the NumberFormatInfo out of the provider, if possible
            // If the IFormatProvider doesn't not contain a NumberFormatInfo, then 
            // this method returns the current culture's NumberFormatInfo. 
            NumberFormatInfo numberFormat = NumberFormatInfo.GetInstance(provider);

            // Is the decimal separator is the same as the list separator?
            // If so, we use the ";". 
            if ((numberFormat.NumberDecimalSeparator.Length > 0) && (numericSeparator == numberFormat.NumberDecimalSeparator[0]))
            {
                numericSeparator = ';';
            }

            return numericSeparator;
        }

        private string parseBack(PathGeometry geometry)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            IFormatProvider provider = new System.Globalization.CultureInfo("en-us");
            string format = null;

            sb.Append("F" + (geometry.FillRule == FillRule.EvenOdd ? "0" : "1") + " ");

            foreach (PathFigure figure in geometry.Figures)
            {
                sb.Append("M " + ((IFormattable)figure.StartPoint).ToString(format, provider) + " ");

                foreach (PathSegment segment in figure.Segments)
                {
                    char separator = GetNumericListSeparator(provider);

                    if (segment.GetType() == typeof(LineSegment))
                    {
                        LineSegment _lineSegment = segment as LineSegment;

                        sb.Append("L " + ((IFormattable)_lineSegment.Point).ToString(format, provider) + " ");
                    }
                    else if (segment.GetType() == typeof(BezierSegment))
                    {
                        BezierSegment _bezierSegment = segment as BezierSegment;

                        sb.Append(String.Format(provider,
                             "C{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "} ",
                             separator,
                             _bezierSegment.Point1,
                             _bezierSegment.Point2,
                             _bezierSegment.Point3
                             ));
                    }
                    else if (segment.GetType() == typeof(QuadraticBezierSegment))
                    {
                        QuadraticBezierSegment _quadraticBezierSegment = segment as QuadraticBezierSegment;

                        sb.Append(String.Format(provider,
                             "Q{1:" + format + "}{0}{2:" + format + "} ",
                             separator,
                             _quadraticBezierSegment.Point1,
                             _quadraticBezierSegment.Point2));
                    }
                    else if (segment.GetType() == typeof(ArcSegment))
                    {
                        ArcSegment _arcSegment = segment as ArcSegment;

                        sb.Append(String.Format(provider,
                             "A{1:" + format + "}{0}{2:" + format + "}{0}{3}{0}{4}{0}{5:" + format + "} ",
                             separator,
                             _arcSegment.Size,
                             _arcSegment.RotationAngle,
                             _arcSegment.IsLargeArc ? "1" : "0",
                             _arcSegment.SweepDirection == SweepDirection.Clockwise ? "1" : "0",
                             _arcSegment.Point));
                    }
                }

                if (figure.IsClosed)
                    sb.Append("Z");
            }

            return sb.ToString();
        }
        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (null != path)
                return Convert(path);
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PathGeometry geometry = value as PathGeometry;

            if (null != geometry)
                return ConvertBack(geometry);
            else
                return default(string);
        }

        #endregion
    }
#endif
}