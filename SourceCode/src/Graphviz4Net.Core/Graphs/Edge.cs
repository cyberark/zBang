
namespace Graphviz4Net.Graphs
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    public class Edge<TVertex> : IEdge<TVertex>, IAttributed
    {
        private readonly IDictionary<string, string> attributes;

        public Edge(
			TVertex source, 
			TVertex destination, 
			object destinationArrow = null,
			object sourceArrow = null,
			object destinationPort = null, 			 
			object sourcePort = null, 
			IDictionary<string, string> attributes = null)
        {
            this.Source = source;
            this.SourcePort = sourcePort;
            this.Destination = destination;
            this.DestinationPort = destinationPort;
            this.DestinationArrow = destinationArrow;
            this.SourceArrow = sourceArrow;
            this.attributes = attributes ?? new Dictionary<string, string>();
        }

		/// <summary>
		/// Determines the position within the destination node where the edge will be pointing to.
		/// </summary>
		/// <remarks>Ports are not supporter in the WPF control yet.</remarks>
        public virtual object DestinationPort { get; private set; }

        public TVertex Source { get; private set; }

        public TVertex Destination { get; private set; }

        public virtual object DestinationArrow { get; private set; }

        public virtual object SourceArrow { get; private set; }

		/// <summary>
		/// Determines the position within the source node where the edge will be pointing to.
		/// </summary>
		/// <remarks>Ports are not supporter in the WPF control yet.</remarks>
        public virtual object SourcePort { get; private set; }

        /// <summary>
        /// Gets or sets the label which will be rendered near the source arrow.
        /// </summary>
        public string SourceArrowLabel
        {
            get { return this.Attributes.GetValue("taillabel", string.Empty); }
            set { this.Attributes["taillabel"] = value; }
        }

        /// <summary>
        /// Gets or sets the label which will be rendered near the destination arrow.
        /// </summary>
        public string DestinationArrowLabel
        {
            get { return this.Attributes.GetValue("headlabel", string.Empty); }
            set { this.Attributes["headlabel"] = value; }
        }

        /// <summary>
        /// Gets or sets Label for the edge. 
        /// Label will not be rendered if set to null or empty string.
        /// </summary>
        public string Label
        {
            get { return this.Attributes.GetValue("label", string.Empty); }
            set { this.Attributes["label"] = value; }
        }

        public string labelToolTip
        {
            get { return this.Attributes.GetValue("labelToolTip", string.Empty); }
            set { this.Attributes["labelToolTip"] = value; }
        }

        public string edgeColor
        {
            get { return  this.Attributes.GetValue( "edgeColor", "black"/*string.Empty*/); }
            set { this.Attributes["edgeColor"] = value; }
        }

        public string edgeStrokeThickness
        {
            get { return this.Attributes.GetValue("edgeStrokeThickness", "1"/*string.Empty*/); }
            set { this.Attributes["edgeStrokeThickness"] = value; }
        }


        /// <summary>
        /// Gets or sets the edge weight.
        /// Use the weight to tweak the generated graphs.
        /// More weight the edge has, more shorter it will be. 
        /// </summary>
        public double? Weight
        {
            get { return Utils.ParseInvariantNullableDouble(this.Attributes.GetValue("weight")); }
            set { this.Attributes["weight"] = value.ToInvariantString(); }
        }

        /// <summary>
        /// Gets or sets the distance between arrow labels and nodes.
        /// </summary>
        public double? ArrowLabelDistance
        {
            get { return Utils.ParseInvariantNullableDouble(this.Attributes.GetValue("labeldistance")); }
            set { this.Attributes["labeldistance"] = value.ToInvariantString(); }            
        }

        /// <summary>
        /// Gets or sets the minimal space between the arrows on the boundary of the same node.
        /// </summary>
        public double? Minlen
        {
            get { return Utils.ParseInvariantNullableDouble(this.Attributes.GetValue("minlen")); }
            set { this.Attributes["minlen"] = value.ToInvariantString(); }
        }

        public IDictionary<string, string> Attributes
        {
            get { return this.attributes; }
        }

        #region IEdge explicit implementation

        object IEdge.Destination
        {
            get { return this.Destination; }
        }

        object IEdge.Source
        {
            get { return this.Source; }
        }

        #endregion

        public static bool AreEqual(IEdge edge, object source, object destination)
        {
            return edge.Source.Equals(source) && edge.Destination.Equals(destination);            
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var edge = (IEdge)obj;
            return AreEqual(this, edge.Source, edge.Destination);
        }

        public override int GetHashCode()
        {
            return this.Source.GetHashCode() ^ this.Destination.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} -- {1} [{2}]", this.Source, this.Destination, this.GetAttributes());
        }
    }
}
