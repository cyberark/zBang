
namespace Graphviz4Net.Graphs
{
    using System.Linq;
    using System.Collections.Generic;

    public static class GraphUtils
    {
        public static IEnumerable<object> GetAllVertices(this IGraph graph)
        {
            return graph.Vertices.Union(graph.SubGraphs.SelectMany(s => s.Vertices));
        }

        public static string GetAttributes(this IAttributed item)
        {
            if (item == null)
            {
                return string.Empty;
            }

            return string.Join(",", item.Attributes.Select(GetAttributeAssignment));
        }

        public static string GetAttributes(this IDictionary<string, string> attributes)
        {
            if (attributes == null)
            {
                return string.Empty;
            }

            return string.Join(",", attributes.Select(GetAttributeAssignment));
        }

        public static string GetAttributeAssignment(KeyValuePair<string, string> attribute)
        {
            // CW: This is to support HTML labels. The syntax is label=<<HTML... without quotes.
            if (attribute.Key.ToLower() == "label" &&
				attribute.Value.StartsWith("<<"))
            {
                return string.Format("{0}={1} ", attribute.Key, attribute.Value);
            }

            return string.Format("{0}=\"{1}\" ", attribute.Key, attribute.Value);
        }
    }
}
