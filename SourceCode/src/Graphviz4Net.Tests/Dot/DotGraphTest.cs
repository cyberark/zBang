
namespace Graphiz4Net.Tests.Dot
{
    using Graphviz4Net.Dot;
    using NUnit.Framework;

    [TestFixture]
    public class DotGraphTest
    {
        [Test]
        public void TestPosAndSizeProperties()
        {
            var graph = new DotGraph<int>();
            graph.Attributes.Add("bb", "1.2,3.4,5,6.7");

            Assert.AreEqual(1.2, graph.BoundingBox.LeftX, "graph.X was not set correctly.");
            Assert.AreEqual(3.4, graph.BoundingBox.LowerY, "graph.Y was not set correctly.");
            Assert.AreEqual(5, graph.BoundingBox.RightX, "graph.Width was not set correctly.");
            Assert.AreEqual(6.7, graph.BoundingBox.UpperY, "graph.Height was not set correctly.");
        }
    }
}
