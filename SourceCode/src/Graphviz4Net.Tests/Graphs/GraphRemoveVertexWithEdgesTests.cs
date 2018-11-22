
namespace Graphviz4Net.Tests.Graphs
{
    using System.Linq;
    using NUnit.Framework;
    using Graphviz4Net.Graphs;

    [TestFixture]
    public class GraphRemoveVertexWithEdgesTests
    {
        private Graph<string> graph;

        [SetUp]
        public void SetUp()
        {
            this.graph = new Graph<string>();
            this.graph.AddVertex("a");
            this.graph.AddVertex("b");
            var subGraph = new SubGraph<string>();
            this.graph.AddSubGraph(subGraph);
            subGraph.AddVertex("c");

            this.graph.AddEdge(new Edge<string>("a", "b"));
            this.graph.AddEdge(new Edge<string>("b", "c"));
        }

        [Test]
        public void WhenRemoveNodeAEdgeFromAToBIsRemoved()
        {
            this.graph.RemoveVertexWithEdges("a");
            Assert.AreEqual(1, this.graph.Vertices.Count());
            Assert.AreEqual("b", this.graph.Vertices.First());
            Assert.AreEqual(1, this.graph.Edges.Count());
            Assert.AreEqual("b", this.graph.Edges.First().Source);
        }

        [Test]
        public void WhenRemoveNodeCEdgeFromBToCIsRemoved()
        {
            this.graph.RemoveVertexWithEdges("c");
            Assert.AreEqual(2, this.graph.Vertices.Count());
            Assert.AreEqual(0, this.graph.SubGraphs.First().Vertices.Count());
            Assert.AreEqual(1, this.graph.Edges.Count());
            Assert.AreEqual("a", this.graph.Edges.First().Source);
        }
    }
}
