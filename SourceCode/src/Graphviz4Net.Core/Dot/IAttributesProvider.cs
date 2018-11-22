
namespace Graphviz4Net.Dot
{
    using System.Collections.Generic;

    public interface IAttributesProvider
    {
        IDictionary<string, string> GetVertexAttributes(object vertex);
    }
}
