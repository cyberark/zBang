
namespace Graphviz4Net.Graphs
{
    using System;
    using System.Collections.Generic;

    public interface IGraph
    {
        IEnumerable<IEdge> Edges { get; }

        IEnumerable<object> Vertices { get; }

        IEnumerable<ISubGraph> SubGraphs { get; }

        /// <summary>
        /// This event should be fired when <see cref="Edges"/>, <see cref="Vertices"/> 
        /// or <see cref="SubGraphs"/> is changed.
        /// </summary>
        event EventHandler<GraphChangedArgs> Changed;
    }

    // NS 06-12-2017
    public class GraphChangedArgs : EventArgs
    {
        public bool needDotReread = true;

        public GraphChangedArgs()
        {
            needDotReread = true;
        }

        public GraphChangedArgs(bool needsreread)
        {
            needDotReread = needsreread;
        }
    }
}
