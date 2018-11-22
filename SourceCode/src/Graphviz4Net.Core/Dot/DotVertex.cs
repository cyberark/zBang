
namespace Graphviz4Net.Dot
{
    using System.Windows;
    using System.Collections.Generic;
    using Graphs;

    /// <summary>
    /// Represents a vertex in a <see cref="DotGraph{TVertexId}"/> obtained from an output of the DOT program.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A vertex can have any general Id type. If you want to use the whole process of layout 
    /// building implemented in <see cref="LayoutDirector"/> then under the hood, the vertices 
    /// will have <see cref="System.Int32"/> ids.
    /// </para>
    /// <para>
    /// If you want to use only parsing abilities of Graphviz4Net, 
    /// you can use the vertices with e.g. <see cref="System.String"/> ids.
    /// </para>
    /// </remarks>
    /// <typeparam name="TId"></typeparam>
    public class DotVertex<TId> : IAttributed
    {
        private readonly IDictionary<string, string> attributes;

        public DotVertex(TId id)
            : this(id, new Dictionary<string, string>())
        {
        }

        public DotVertex(TId id, IDictionary<string, string> attributes)
        {
            this.Id = id;
            this.attributes = attributes;
        }

        public TId Id { get; private set; }

        public double? Width
        {
            get { return Utils.ParseInvariantNullableDouble(this.Attributes.GetValue("width")); }
        }

        public double? Height
        {
            get { return Utils.ParseInvariantNullableDouble(this.Attributes.GetValue("height")); }
        }

        public Point? Position
        {
            get
            {
                var posStr = this.Attributes.GetValue("pos");
                if (string.IsNullOrEmpty(posStr))
                {
                    return null;
                }

                var posParts = posStr.Split(',');
                if (posParts.Length < 2)
                {
                    return null;
                }

                double x, y;
                if (Utils.TryParseInvariantDouble(posParts[0], out x) == false ||
                    Utils.TryParseInvariantDouble(posParts[1], out y) == false)
                {
                    return null;
                }

                return new Point(x, y);
            }
        }

        public IDictionary<string, string> Attributes
        {
            get { return this.attributes; }
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}
