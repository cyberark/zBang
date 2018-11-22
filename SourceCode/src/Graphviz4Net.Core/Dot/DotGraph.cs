
namespace Graphviz4Net.Dot
{
    using Graphs;

    public class DotGraph<TVertexId> : 
        Graph<DotVertex<TVertexId>, DotSubGraph<TVertexId>, DotEdge<TVertexId>, Edge<DotSubGraph<TVertexId>>> 
    {
        private BoundingBox bondingBox = null;

        public double? Width
        {
            get { return this.BoundingBox.RightX - this.BoundingBox.LeftX; }
        }

        public double? Height
        {
            get { return this.BoundingBox.UpperY - this.bondingBox.LowerY; }
        }

        public BoundingBox BoundingBox
        {
            get
            {
                string newBb;
                this.Attributes.TryGetValue("bb", out newBb);
                if (this.bondingBox == null || this.bondingBox.Equals(newBb) == false)
                {
                    this.bondingBox = new BoundingBox(newBb);
                }

                return this.bondingBox;
            }
        }       
    }
}
