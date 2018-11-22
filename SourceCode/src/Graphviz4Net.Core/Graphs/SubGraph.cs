
namespace Graphviz4Net.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;

    public class SubGraph<TVertex> : ISubGraph<TVertex>, IAttributed, IVerticesCollection<TVertex>
    {
        private readonly IList<TVertex> vertices = new List<TVertex>();

        private readonly IDictionary<string, string> attributes = new Dictionary<string, string>();

        IEnumerable<object> ISubGraph.Vertices
        {
            get { return this.Vertices.Cast<object>(); }
        }

        public IEnumerable<TVertex> Vertices
        {
            get { return this.vertices; }
        }

        public void AddVertex(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            this.vertices.Add(vertex);
            this.FireChanged();
        }

        /// <summary>
        /// Removes a vertex from this graph. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is internal, because it does not remove edges and therefore it 
        /// can left the graph in undefined state when used not properly.
        /// </para>
        /// </remarks>
        /// <param name="vertex">Vertex to be removed.</param>
        internal void RemoveVertex(TVertex vertex)
        {
            Contract.Requires(vertex != null);
            Contract.Requires(
                this.Vertices.Contains(vertex),
                "RemoveVertex: given vertex is not part of the graph. See the API documentation for more details.");
            this.vertices.Remove(vertex);
        }

        public string Label
        {
            get { return this.Attributes.GetValue("label", string.Empty); }
            set
            {
                this.Attributes["label"] = value;
                this.FireChanged();
            }
        }
        public string borderColor
        {
            get { return this.Attributes.GetValue("borderColor", "Red"); }
            set
            {
                this.Attributes["borderColor"] = value;
                this.FireChanged();
            }
        }
        public string borderThickness
        {
            get { return this.Attributes.GetValue("borderThickness", "2"); }
            set
            {
                this.Attributes["borderThickness"] = value;
                this.FireChanged();
            }
        }

        public event EventHandler<GraphChangedArgs> Changed;

        public IDictionary<string, string> Attributes
        {
            get { return this.attributes; }
        }

        public override string ToString()
        {
            var result = new StringBuilder("subgraph: ");
            foreach (var vertex in this.Vertices)
            {
                result.AppendLine("\t" + vertex);
            }

            return result.ToString();
        }

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
                        typeof(TVertex)));

            }

            this.AddVertex((TVertex)vertex);
        }

        #endregion

        private void FireChanged()
        {
            if (this.Changed != null)
            {
                this.Changed(this, new GraphChangedArgs());
            }
        }
    }
}
