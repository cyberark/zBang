
namespace Graphviz4Net.Dot.AntlrParser
{
    using System.Collections.Generic;

    /// <summary>
    /// Code behind file for generated parser class.
    /// </summary>
    public partial class DotGrammarParser
    {
        public IDotGraphBuilder Builder { get; set; }

        public void EnterSubGraph(string name)
        {
            this.Builder.EnterSubGraph(name);
        }

        public void LeaveSubGraph()
        {
            this.Builder.LeaveSubGraph();
        }

        public void AddGraphAttributes(IDictionary<string, string> attributes)
        {
            this.Builder.AddGraphAttributes(attributes);
        }

        /// <summary>
        /// This method is used inside the DotGrammar.g file.
        /// </summary>
        public string Unquote(string str)
        {
            return str.Substring(1, str.Length - 1).Substring(0, str.Length - 2).Replace(@"\", string.Empty);
        }

        public void AddEdge(string sourceStr, string targetStr, IDictionary<string, string> attributes)
        {
            this.Builder.AddEdge(sourceStr, targetStr, attributes);
        }

        public void AddVertex(string idStr, IDictionary<string, string> attributes)
        {
            this.Builder.AddVertex(idStr, attributes);
        }
    }
}
