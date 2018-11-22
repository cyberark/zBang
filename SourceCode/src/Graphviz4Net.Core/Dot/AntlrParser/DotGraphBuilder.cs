
namespace Graphviz4Net.Dot.AntlrParser
{
    using System.Collections.Generic;

    public interface IDotGraphBuilder
    {
        void EnterSubGraph(string name);

        void LeaveSubGraph();

        void AddEdge(string sourceStr, string targetStr, IDictionary<string, string> attributes);

        void AddVertex(string idStr, IDictionary<string, string> attributes);

        void AddGraphAttributes(IDictionary<string, string> attributes);
    }

    /// <summary>
    /// The implementor must initialize <see cref="DotGraph"/> in its constructor!
    /// </summary>
    public abstract class DotGraphBuilder<TVertexId> : IDotGraphBuilder
    {
        private DotSubGraph<TVertexId> subGraph;

        public DotGraph<TVertexId> DotGraph { get; protected set; }

        public void AddGraphAttributes(IDictionary<string, string> attributes)
        {
            if (attributes == null)
            {
                return;
            }

            foreach (var attribute in attributes)
            {
                if (this.subGraph == null)
                {
                    this.DotGraph.Attributes.Add(attribute);
                }
                else
                {
                    this.subGraph.Attributes.Add(attribute);
                }
            }
        }

        public void EnterSubGraph(string name)
        {
            this.subGraph = new DotSubGraph<TVertexId> {Name = name};
            this.DotGraph.AddSubGraph(this.subGraph);
        }

        public void LeaveSubGraph()
        {
            this.subGraph = null;
        }

        public void AddEdge(string sourceStr, string targetStr, IDictionary<string, string> attributes)
        {
            var source = this.GetVertex(sourceStr);
            var target = this.GetVertex(targetStr);
            this.DotGraph.AddEdge(new DotEdge<TVertexId>(source, target, attributes));
        }

        public void AddVertex(string idStr, IDictionary<string, string> attributes)
        {
            var vertex = this.CreateVertex(idStr, attributes);

            if (this.subGraph == null)
            {
                this.DotGraph.AddVertex(vertex);
            }
            else
            {
                this.subGraph.AddVertex(vertex);
            }
        }

        protected abstract DotVertex<TVertexId> CreateVertex(string idStr, IDictionary<string, string> attributes);

        protected abstract DotVertex<TVertexId> GetVertex(string idStr);
    }
}
