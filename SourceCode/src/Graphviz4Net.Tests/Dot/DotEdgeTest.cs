
namespace Graphiz4Net.Tests.Dot
{
    using System.Windows;
    using Graphviz4Net.Dot;
    using NUnit.Framework;

    [TestFixture]
    public class DotEdgeTest
    {
        [Test]
        public void TestPathAndEndEdgeEndProperties()
        {
            var edge = new DotEdge<int>(new DotVertex<int>(0), new DotVertex<int>(1));
            edge.Attributes.Add("pos", "e,2,3 1.14,3.14 9,8.7");

            Assert.AreEqual(new Point(1.14, 3.14), edge.Path[0]);
            Assert.AreEqual(new Point(9, 8.7), edge.Path[1]);
            Assert.AreEqual(new Point(2, 3), edge.DestinationArrowEnd);
        }
    }
}
