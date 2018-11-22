
namespace Graphviz4Net.Graphs
{
    using System.Collections.Generic;

    public interface IAttributed
    {
        IDictionary<string, string> Attributes { get; }
    }
}
