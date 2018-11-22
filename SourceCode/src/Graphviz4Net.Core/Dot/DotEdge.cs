
namespace Graphviz4Net.Dot
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using Graphs;

    public class DotEdge<TVertexId> : Edge<DotVertex<TVertexId>>
    {
        private Point[] data;

        private Point? destionationArrowPos;

        private Point? sourceArrowPos;

        /// <summary>
        /// For checking whether cached value in <see cref="data"/>, <see cref="destionationArrowPos"/> 
        /// and <see cref="sourceArrowPos"/> are still valid.
        /// This member holds original string from which <see cref="data"/> were parsed.
        /// </summary>
        private string parsedPos;

        public DotEdge(DotVertex<TVertexId> source, DotVertex<TVertexId> destination) 
            : base(source, destination)
        {
        }

        public DotEdge(DotVertex<TVertexId> source, DotVertex<TVertexId> destination, IDictionary<string, string> attributes)
            : base(source, destination, attributes: attributes)
        {
        }      

        public int Id
        {
            get
            {
                string idStr;
                int id;
                if (this.Attributes.TryGetValue("comment", out idStr) &&
                    int.TryParse(idStr, out id))
                {
                    return id;
                }

                throw new InvalidFormatException(
                    string.Format(
                        "Edge {0} does not have its id in the dot 'comment' attribute, " + 
                        "instead the attribute contains this string: '{1}'.", 
                        this,
                        idStr));
            }
        }
  
        public Point[] Path
        {
            get
            {
                this.ParsePathData();
                return this.data;
            }
        }

        public Point? LabelPos
        {
            get
            {
                var lb = this.Attributes.GetValue("lp");
                if (lb == null)
                { 
                    return null;
                }

                var parts = lb.Split(',');
                if (parts.Length < 2)
                {
                    return null;
                }

                double x, y;
                if (Utils.TryParseInvariantDouble(parts[0], out x) &&
                    Utils.TryParseInvariantDouble(parts[1], out y))
                {
                    return new Point(x, y);
                }

                return null;
            }
        }

        public Point? DestinationArrowEnd
        {
            get
            {
                this.ParsePathData();
                return this.destionationArrowPos;
            }
        }

        public Point? SourceArrowEnd
        {
            get
            {
                this.ParsePathData();
                return this.sourceArrowPos;
            }
        }

        public Point? DestinationArrowLabelPosition
        {
            get
            {
                var value = this.Attributes.GetValue("head_lp");
                if (value == null)
                {
                    return null;
                }

                return this.GetPosition(value);
            }
        }

        public Point? SourceArrowLabelPosition
        {
            get
            {
                var value = this.Attributes.GetValue("tail_lp");
                if (value == null)
                {
                    return null;
                }

                return this.GetPosition(value);
            }
        }

        public bool HasDestinationArrowEnd
        {
            get
            {
                var result = this.Attributes.GetValue("pos");
                return result != null && result.StartsWith("e");
            }
        }

        private void ParsePathData()
        {
            var pos = this.Attributes.GetValue("pos");
            if (string.IsNullOrEmpty(pos))
            {
                throw new InvalidFormatException(string.Format("The pos attribute of edge {0} is empty.", this));
            }

            if (pos == this.parsedPos && this.data != null)
            {
                return;
            }

            this.parsedPos = pos;
            var positions = pos.Trim().Split(' ').Select(x => x.Trim());

            var destinationArrowPosStr = positions.FirstOrDefault(x => x.StartsWith("e"));
            if (destinationArrowPosStr != null)
            {
                this.destionationArrowPos = 
                    this.GetPosition(destinationArrowPosStr.Substring(2, destinationArrowPosStr.Length - 2));
            }

            var sourceArrowPosStr = positions.FirstOrDefault(x => x.StartsWith("s"));
            if (sourceArrowPosStr != null)
            {
                this.sourceArrowPos =
                    this.GetPosition(sourceArrowPosStr.Substring(2, sourceArrowPosStr.Length - 2));
            }

            try
            {
                this.data = positions.Where(x => x != sourceArrowPosStr && x != destinationArrowPosStr)
                    .Select(GetPosition).ToArray();
            }
            catch (InvalidFormatException e)
            {
                throw new InvalidFormatException(
                    string.Format("The pos attribute of edge {0} has invalid value. {1}", this, e.Message), 
                    e);
            }
        }

        public Point GetPosition(string value)
        {
            Contract.Requires(value != null);
            double x, y;
            var pointParts = value.Split(',');
            if (pointParts.Length == 2 &&
                Utils.TryParseInvariantDouble(pointParts[0], out x) &&
                Utils.TryParseInvariantDouble(pointParts[1], out y))
            {
                return new Point(x, y);
            }            
            else
            {
                throw new InvalidFormatException(
                    string.Format(
                        "Position {0} in attributes of edge {1} has invalid format.",
                        string.Join(", ", pointParts),
                        this));                
            }
        }

        public class InvalidFormatException : Graphviz4NetException
        {
            public InvalidFormatException(string message, Exception inner = null)
                : base(message, inner)
            {                
            }
        }
    }
}
