
namespace Graphviz4Net.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;

    public class Graph<TVertex, TSubGraph, TVeticesEdge, TSubGraphsEdge> : IGraph, IAttributed, IVerticesCollection<TVertex> 
        where TVeticesEdge : IEdge<TVertex> 
        where TSubGraphsEdge: IEdge<TSubGraph>
        where TSubGraph : ISubGraph<TVertex>
    {
        private readonly IList<IEdge> edges = new List<IEdge>();

        private readonly IList<TVertex> vertices = new List<TVertex>();

        private readonly IList<TSubGraph> subGraphs = new List<TSubGraph>();

        private readonly IDictionary<string, string> attributes = new Dictionary<string, string>();

        private int startChangesCalled = 0;

        public bool resetDotGraph = true;

        #region Properties

        /// <summary>
        /// This collection is not specialized, because edges might connect either nodes or sub-graphs, 
        /// therefore we cannot choose between <c>TVerticesEdge</c> or <c>TSubgraphsEdge</c>. 
        /// If you want 'type safe' access use <see cref="VerticesEdges"/> or <see cref="SubGraphsEdges"/>.
        /// </summary>
        public IEnumerable<IEdge> Edges
        {
            get { return this.edges; }
        }

        public IEnumerable<TVertex> Vertices
        {
            get { return this.vertices; }
        }

        public IEnumerable<TSubGraph> SubGraphs
        {
            get { return this.subGraphs; }
        }

        public IEnumerable<TVeticesEdge> VerticesEdges
        {
            get { return this.Edges.OfType<TVeticesEdge>(); }
        }

        public IEnumerable<TSubGraphsEdge> SubGraphsEdges
        {
            get { return this.Edges.OfType<TSubGraphsEdge>(); }
        }

        public event EventHandler<GraphChangedArgs> Changed;

        /// <summary>
        /// Gets the height/width ratio that should be fulfilled when generating the layout.
        /// </summary>
        public double? Ratio
        {
            get { return Utils.ParseInvariantNullableDouble(this.Attributes.GetValue("ratio")); }
            set { this.Attributes["ratio"] = value.ToInvariantString(); }
        }

        /// <summary>
        /// Gets the vertices on the top level and all the vertices from subgraphs.
        /// </summary>
        public IEnumerable<TVertex> AllVertices
        {
            get
            {
                return this.vertices.Concat(this.subGraphs.SelectMany(x => x.Vertices));
            }
        }

        public IDictionary<string, string> Attributes
        {
            get { return this.attributes; }
        }

        public RankDirection Rankdir
        {
            get { return RankDirection.FromString(this.attributes.GetValue("rankdir", "LR")); }
            set { this.attributes["rankdir"] = value.ToString(); }
        }
        public string Rank
        {
            get { return this.attributes.GetValue("rank", "LR"); }
            set { this.attributes["rank"] = value; }
        }

        #endregion

        #region Add operations

        public void AddEdge(TVeticesEdge edge)
        {
            Contract.Requires(edge != null);
            Contract.Requires(this.AllVertices.Contains(edge.Source), "Edge's source does not belong to the graph.");
            Contract.Requires(this.AllVertices.Contains(edge.Destination), "Edge's source does not belong to the graph.");
            this.edges.Add(edge);
            this.RaiseChanged();
        }

        public void AddEdge(TSubGraphsEdge edge)
        {
            Contract.Requires(edge != null);
            Contract.Requires(this.SubGraphs.Contains(edge.Source), "Edge's source does not belong to the graph.");
            Contract.Requires(this.SubGraphs.Contains(edge.Destination), "Edge's source does not belong to the graph.");
            this.edges.Add(edge);
            this.RaiseChanged();
        }

        public void AddVertex(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            Contract.Requires(this.AllVertices.Contains(vertex) == false, "Vertex is already in the graph.");
            this.vertices.Add(vertex);
            this.RaiseChanged();
        }

        public void AddSubGraph(TSubGraph subGraph)
        {
            Contract.Requires(subGraph != null);
            this.subGraphs.Add(subGraph);
            this.RaiseChanged();
            subGraph.Changed += SubGraphChanged;
        }

        #endregion

        /// <summary>
        /// Removes a vertex from this graph. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// It cannot remove vertex from sub-graphs, for this 
        /// one has to invoke specific method on sub-graph, if the sub-graph supports it.
        /// </para>
        /// <para>
        /// To remove a vertex from sub-graph one has invoke RemoveVertex on that particular sub-graph. 
        /// Class <see cref="Graph{TVertex,TEdge}"/> provides method <see cref="Graph{TVertex,TEdge}.RemoveVertexWithEdges"/>, 
        /// which is capable of removing a vertex from a sub-graph and which also removes all edges that contain the vertex.
        /// </para>
        /// </remarks>
        /// <param name="vertex">Vertex to be removed.</param>
        public void RemoveVertex(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            Contract.Requires(
                this.Vertices.Contains(vertex),
                "RemoveVertex: given vertex is not part of the graph. See the API documentation for more details.");
            this.vertices.Remove(vertex);
            this.RaiseChanged();
        }

        public void RemoveEdge(IEdge edge)
        {
            Contract.Requires(edge != null);
            Contract.Requires(this.edges.Contains(edge), "RemoveEdge: given edge is not part of the graph");
            this.edges.Remove(edge);
            this.RaiseChanged();
        }

        public void RemoveSubGraph(TSubGraph subGraph)
        {
            Contract.Requires(subGraph != null);
            Contract.Requires(this.subGraphs.Contains(subGraph), "RemoveSubGraph: given subgraph is not part of the graph.");
            this.subGraphs.Remove(subGraph);
            this.RaiseChanged();
        }

        #region Explicit IGraph implementation

        IEnumerable<object> IGraph.Vertices
        {
            get
            {
                // Note: we use Cast<object> instead of casting to IEnumerable<object>, because 
                // silverlight seems not to support the former
                return this.vertices.Cast<object>();
            }
        }

        IEnumerable<ISubGraph> IGraph.SubGraphs
        {
            get { return this.subGraphs.Cast<ISubGraph>(); }
        }

        #endregion

        #region Explicit IVerticesCollection implementation

        IEnumerable<object> IVerticesCollection.Vertices
        {
            get { return this.vertices.Cast<object>(); }
        }

        void IVerticesCollection.AddVertex(object vertex)
        {
            if (vertex is TVertex == false)
            {
                throw new NotSupportedException(
                    string.Format(
                        "AddVertex(object) called with {0}, but this graph is generic and supports only vertices of type {1}",
                        vertex != null ? vertex.GetType().Name : "null",
                        typeof (TVertex)));

            }

            this.AddVertex((TVertex) vertex);
        }

        #endregion

        public override string ToString()
        {
            var result = new StringBuilder("graph: ");
            foreach (var vertex in this.Vertices)
            {
                result.AppendLine(vertex.ToString());
            }

            foreach (var subGraph in this.SubGraphs)
            {
                result.Append(subGraph);
                result.AppendLine();
            }

            foreach (var edge in Edges)
            {
                result.AppendLine(edge.ToString());
            }

            return result.ToString();
        }

        protected void StartChanges()
        {
            this.startChangesCalled++;
        }

        protected void EndChanges()
        {
            if (this.startChangesCalled == 0)
            {
                throw new InvalidOperationException("Cannot call EndChanges before StartChanges.");
            }

            this.startChangesCalled--;
            if (this.startChangesCalled == 0)
            {
                this.RaiseChanged();
            }
        }


        public void EndChangesDontRereadDot()
        {
            if (this.startChangesCalled == 0)
            {
                throw new InvalidOperationException("Cannot call EndChanges before StartChanges.");
            }

            this.startChangesCalled--;
            if (this.startChangesCalled == 0)
            {
                this.RaiseChanged( false);
            }
        }


        protected void RaiseChanged()
        {
            if (this.Changed != null && this.startChangesCalled == 0)
            {
                this.Changed(this, new GraphChangedArgs());
            }
        }

        private void SubGraphChanged(object sender, GraphChangedArgs e)
        {
            this.RaiseChanged();
        }

        /**
         * @brief       sends a graph changed refresh requests
         * 
         **/
        protected void RaiseChanged(bool needsDotReread)
        {
            if (this.Changed != null && this.startChangesCalled == 0)
            {
                this.Changed(this, new GraphChangedArgs(needsDotReread));
            }
        }
    }

    public class Graph<TVertex, TEdge> : Graph<TVertex, SubGraph<TVertex>, TEdge, Edge<SubGraph<TVertex>>>
        where TEdge : IEdge<TVertex>
    {
        /// <summary>
        /// Removes given vertex and all related edges.
        /// </summary>
        /// <remarks>Edges with <paramref name="vertex"/> as <see cref="IEdge.Source"/> or <see cref="IEdge.Destination"/>
        /// will be removed as well. 
        /// This operation performs several searches in list of vertices, therefore, it may be quiet slow.</remarks>
        /// <param name="vertex">The vertex to be removed.</param>
        public void RemoveVertexWithEdges(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            this.StartChanges();
            bool found = false;

            if (this.Vertices.Contains(vertex))
            {
                this.RemoveVertex(vertex);
                found = true;
            }
            else
            {
                foreach (var subGraph in this.SubGraphs)
                {
                    if (subGraph.Vertices.Contains(vertex))
                    {
                        subGraph.RemoveVertex(vertex);
                        found = true;
                    }
                }
            }

            if (found)
            {
                this.RemoveEdgesWith(vertex);
                this.EndChanges();
                return;
            }

            throw new ArgumentException(
                "RemoveVertexWithEdges: given vertex is not part of the graph nor part of any of it's subgraphs.");
        }

        private void RemoveEdgesWith(TVertex vertex)
        {
            var edgesToRemove = this.Edges.Where(e => e.Source.Equals(vertex) || e.Destination.Equals(vertex)).ToArray();
            foreach (var e in edgesToRemove)
            {
                this.RemoveEdge(e);
            }
        }

        public void changeEdgeColorStart()
        {
            this.StartChanges();
        }
        public void changeEdgeColorEnd()
        {
            this.EndChanges();
        }

        /**
         * @brief       sends a graph changed refresh requests
         * 
         **/
        public void UpdateGraphWithoutRereadingDot()
        {
            RaiseChanged(false);
        }

        public void UpdateGraphRereadingDot()
        {
            RaiseChanged();
        }



    }

    public class Graph<TVertex> : Graph<TVertex, Edge<TVertex>>
    {
    }
}
