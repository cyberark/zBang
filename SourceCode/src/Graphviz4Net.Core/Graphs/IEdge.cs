
namespace Graphviz4Net.Graphs
{
    public interface IEdge
    {
        object Source { get; }

        object Destination { get; }

        object DestinationPort { get; }

        /// <summary>
        /// If the arrow is null, then the edge does not have a destination arrow.
        /// </summary>
        object DestinationArrow { get; }

        /// <summary>
        /// If the arrow is null, then the edge does not have a source arrow.
        /// </summary>
        object SourceArrow { get; }

        object SourcePort { get; }

        string edgeColor { get; set; }
    }

    public interface IEdge<out TVertex> : IEdge
    {
        new TVertex Source { get; }

        new TVertex Destination { get; }
    }
}
