
namespace Graphviz4Net.Dot
{
    using System.IO;
    using Graphs;

    public interface IGraphToDotConverter
    {
        /// <summary>
        /// Converts given graph into the dot language.
        /// </summary>
        /// <returns>During the conversion each graph element is assigned an id. Map of these ids is returned as an array.
        /// Index is vertex id and value is the vertex instance with given id.</returns>
        object[] Convert(TextWriter writer, IGraph graph, IAttributesProvider additionalAttributesProvider);
    }
}
