
namespace Graphviz4Net.Graphs
{
    using System.Collections.Generic;

    public interface IVerticesCollection
    {
        IEnumerable<object> Vertices { get; }

        void AddVertex(object vertex);
    }

    public interface IVerticesCollection<T> : IVerticesCollection
    {
        new IEnumerable<T> Vertices { get; }

        void AddVertex(T vertex);
    }
}
