
namespace Graphviz4Net.Dot
{
    using System.IO;

    public interface IDotParser<TVertexId>
    {
        DotGraph<TVertexId> Parse(TextReader reader);
    }
}
