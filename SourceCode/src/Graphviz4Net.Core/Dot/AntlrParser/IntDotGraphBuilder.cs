
namespace Graphviz4Net.Dot.AntlrParser
{
    using System.Collections.Generic;

    public class IntDotGraphBuilder : DotGraphBuilder<int>
    {
        private readonly IList<DotVertex<int>> vertices = new List<DotVertex<int>>(32);

        public IntDotGraphBuilder()
        {
            this.DotGraph = new DotGraph<int>();
        }

        protected override DotVertex<int> CreateVertex(string idStr, IDictionary<string, string> attributes)
        {
            int id;
            if (int.TryParse(idStr, out id) == false)
            {
                throw new ParserException(GetInvalidIdMessage(idStr));
            }

            var vertex = new DotVertex<int>(id, attributes);
            this.vertices.InsertAt(id, vertex);
            return vertex;
        }

        private static string GetInvalidIdMessage(string idStr)
        {
            return string.Format(
                "The id '{0}', which was found in the input file, is not an integer. " +
                "Provide input file with only integral ids, or use other implementation of IDotGraphBuilder, " +
                "for example StringDotGraphBuilder. If you are using the whole process of layout building via " +
                "LayoutDirector, then this error is probably a bug a you can report it on the project web site.",
                idStr);
        }

        protected override DotVertex<int> GetVertex(string idStr)
        {
            int id;
            if (int.TryParse(idStr, out id) == false)
            {
                throw new ParserException(GetInvalidIdMessage(idStr));
            }

            if (id >= this.vertices.Count || id < 0 || this.vertices[id] == null)
            {
                var msg = string.Format(
                    "The id '{0}', which was found in the input file, has either wrong format, or " +
                    "appeared in edge definition before the vertex itself was defined.",
                    idStr);
                throw new ParserException();                
            }

            return this.vertices[id];
        }
    }
}
