
namespace Graphviz4Net.Tests.Graphs
{
    using NUnit.Framework;
    using Graphviz4Net.Graphs;

    [TestFixture]
    public class GraphChangedTests
    {
        private Graph<Model> graph;

        private int graphChangedCalled;

        [SetUp]
        public void SetUp()
        {
            this.graph = new Graph<Model>();
            this.graph.Changed += GraphChanged;
            this.graphChangedCalled = 0;
        }

        [Test]
        public void ChangedFiredAfterAddVertex()
        {
            this.graph.AddVertex(new Model());
            Assert.AreEqual(1, this.graphChangedCalled);
        }

        [Test]
        public void ChangedFiredAfterAddSubGraph()
        {
            this.graph.AddSubGraph(new SubGraph<Model>());
            Assert.AreEqual(1, this.graphChangedCalled);
        }

        [Test]
        public void ChangedFiredAfterAddVertexToSubGraph()
        {
            var subgraph = new SubGraph<Model>();
            this.graph.AddSubGraph(subgraph);
            this.graphChangedCalled = 0;
            subgraph.AddVertex(new Model());
            Assert.AreEqual(1, this.graphChangedCalled);
        }

        [Test]
        public void ChangedFiredAfterRemovalOfVertexWithEdges()
        {
            var model = new Model();
            this.graph.AddVertex(model);
            this.graphChangedCalled = 0;
            this.graph.RemoveVertexWithEdges(model);
            Assert.AreEqual(1, this.graphChangedCalled);            
        }

        private void GraphChanged(object sender, GraphChangedArgs e)
        {
            this.graphChangedCalled++;
        }        

        public class Model
        {
        }
    }
}
