
namespace Graphviz4Net
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Dot;
    using Graphs;

    /// <summary>
    /// Abstract class providers basic implementation of some of the <see cref="ILayoutBuilder{TVertexId}"/> 
    /// methods. Methods that are necessary to override in order to implement correct builder are abstract.
    /// </summary>
    public abstract class LayoutBuilder<TVertexId> : ILayoutBuilder<TVertexId>
    {
        public virtual void Start(IGraph originalGraph)
        {
        }

        public virtual void BuildGraph(double width, double height, IGraph originalGraph, DotGraph<TVertexId> dotGraph)
        {
        }

        public abstract void BuildVertex(Point position, double width, double height, object originalVertex, DotVertex<TVertexId> dotVertex);

        public abstract void BuildSubGraph(
            double leftX, 
            double upperY, 
            double rightX, 
            double lowerY, 
            ISubGraph originalSubGraph,
            DotSubGraph<int> subGraph);

        public abstract void BuildEdge(Point[] path, IEdge originalEdge, DotEdge<TVertexId> edge);

        public virtual void Finish()
        {
        }

        /// <summary>
        /// This method is given a vertex from the original graph and is 
        /// expected to return its size (or size of a graphical element that will represent it).
        /// </summary>
        /// <param name="vertex">The vertex from the original graph. 
        /// It must be <see cref="object"/>, because the original graph might be any general 
        /// <see cref="Graph{TVertex,TSubGraph,TVeticesEdge,TSubGraphsEdge}"/>.</param>
        public abstract Size GetSize(object vertex);

        /// <summary>
        /// This method is given a vertex from the original graph and is expected to return 
        /// additional GraphViz attributes that will be added to it.
        /// </summary>
        /// <param name="vertex">The vertex from the original graph. 
        /// It must be <see cref="object"/>, because the original graph might be any general 
        /// <see cref="Graph{TVertex,TSubGraph,TVeticesEdge,TSubGraphsEdge}"/>.</param>
        public IEnumerable<KeyValuePair<string, string>> GetAdditionalAttributes(object vertex)
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }
    }
}