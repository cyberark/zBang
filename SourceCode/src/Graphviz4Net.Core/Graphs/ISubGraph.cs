
namespace Graphviz4Net.Graphs
{
    using System;
    using System.Collections.Generic;

    public interface ISubGraph
    {
        IEnumerable<object> Vertices { get; }

        string Label { get; }
        string borderColor { get; }
        string borderThickness { get; }

        /// <summary>
        /// This event should be fired when <see cref="Vertices"/> collection is changed.
        /// </summary>
        event EventHandler<GraphChangedArgs> Changed;
    }

#if SILVERLIGHT
    public interface ISubGraph<TVertex> : ISubGraph
#else
    public interface ISubGraph<out TVertex> : ISubGraph
#endif
    {
        new IEnumerable<TVertex> Vertices { get; }
        
    }
}
