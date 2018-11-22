

namespace Graphviz4Net.Dot
{
    using System.Text;
    using System;
    using System.IO;    
    using System.Collections.Generic;
    using System.Linq;
    using Graphs;
#if !SILVERLIGHT
    using System.CodeDom.Compiler;
#endif

    public class GraphToDotConverter : IGraphToDotConverter
    {
        /// <summary>
        /// Converts given graph into the dot language.
        /// </summary>
        /// <returns>During the conversion each node is assigned an id. Map of these ids is returned.</returns>
        public object[] Convert(
            TextWriter originalWriter, 
            IGraph graph, 
            IAttributesProvider additionalAttributesProvider)
        {
            var nodesMap = new List<object>();
            var idsMap = new Dictionary<object, int>();
            var writer = new IndentedTextWriter(originalWriter);
            writer.WriteLine("digraph g { ");

            var graphAttributes = new Dictionary<string, string>();
            if (graph is IAttributed)
            {
                graphAttributes = new Dictionary<string,string>(((IAttributed) graph).Attributes);
            }

            // we don't need compound attribute if there are no sub-graphs
            if (graph.SubGraphs.Any())
            {
	            graphAttributes.Add("compound", "true");
            }

        	writer.WriteLine("graph [{0}];", graphAttributes.GetAttributes());

            writer.Indent++;
            foreach (var vertex in graph.Vertices)
            {
                writer.WriteLine("{0} [ {1} ];", nodesMap.Count, GetVertexAttributes(vertex, additionalAttributesProvider));
                idsMap.Add(vertex, nodesMap.Count);
                nodesMap.Add(vertex);
            }

            var subGraphs = graph.SubGraphs.ToArray();
            for (int i = 0; i < subGraphs.Length; i++)
            {
                var subGraph = subGraphs[i];
                writer.WriteLine("subgraph cluster{0} {{ ", nodesMap.Count);
                idsMap.Add(subGraph, nodesMap.Count);
                nodesMap.Add(subGraph);
                writer.Indent++;

                // In order to have clusters labels always on the top:
                // when the direction is from bottom to top, change label location to bottom, 
                // which in this direction means top of the image
                string rankdir;
                if (graph is IAttributed &&
                    ((IAttributed)graph).Attributes.TryGetValue("rankdir", out rankdir) &&
                    rankdir == RankDirection.BottomToTop)
                {
                    writer.WriteLine("graph [labelloc=b];");
                }

                if (subGraph is IAttributed)
                {
                    writer.WriteLine("graph [{0}];", ((IAttributed)subGraph).GetAttributes());
                }

                foreach (var vertex in subGraph.Vertices)
                {
                    writer.WriteLine("{0} [ {1} ];", nodesMap.Count, GetVertexAttributes(vertex, additionalAttributesProvider));
                    idsMap.Add(vertex, nodesMap.Count);
                    nodesMap.Add(vertex);
                }
                writer.Indent--;
                writer.WriteLine("}; ");
            }

            foreach (var edge in graph.Edges)
            {
                IDictionary<string, string> attributes = new Dictionary<string, string>();
                if (edge is IAttributed)
                {
                    attributes = ((IAttributed)edge).Attributes;
                }

                if (edge.SourceArrow != null)
                {
                    attributes["dir"] = "both";
                }

                object edgeSource = edge.Source;
                object edgeDestination = edge.Destination;

                if (edgeSource is ISubGraph)
                {
                    attributes["ltail"] = "cluster" + idsMap[edgeSource];
                    edgeSource = ((ISubGraph) edgeSource).Vertices.First();
                }

                if (edgeDestination is ISubGraph)
                {
                    attributes["lhead"] = "cluster" + idsMap[edgeDestination];
                    var subGraph = ((ISubGraph) edgeDestination);
                    edgeDestination = subGraph.Vertices.FirstOrDefault();
                    if (edgeDestination == null)
                    {
                        throw new InvalidOperationException(
                            "SubGraph must have at least one vertex. " +
                            string.Format("This does not hold for {0} subgraph. ", subGraph.Label) +
                            "Graphviz4Net cannot render empty subgraphs. ");
                    }
                }

                attributes["comment"] = nodesMap.Count.ToString();
                nodesMap.Add(edge);

                writer.WriteLine(
                    "{0}{1} -> {2}{3} [ {4} ];", 
                    idsMap[edgeSource],
                    edge.SourcePort == null ? string.Empty : ":\"" + edge.SourcePort + "\"",
                    idsMap[edgeDestination],
                    edge.DestinationPort == null ? string.Empty : ":\"" + edge.DestinationPort + "\"",
                    attributes.GetAttributes());
            }

            writer.Indent--;
            writer.WriteLine("}");   // end of the diagraph

            return nodesMap.ToArray();
        }

        public static string GetVertexAttributes(object vertex, IAttributesProvider provider)
        {
            var attributes = provider.GetVertexAttributes(vertex);
            if (vertex is IAttributed)
            {
                foreach (var attribute in ((IAttributed)vertex).Attributes)
                {
                    attributes.Add(attribute);
                }
            }

            return attributes.GetAttributes();
        }

#if SILVERLIGHT
        public class IndentedTextWriter : TextWriter
        {
            private readonly TextWriter writer;

            public IndentedTextWriter(TextWriter writer)
            {
                this.writer = writer;
            }

            public int Indent { get; set; }

            public override void Write(char value)
            {
                this.writer.Write(value);
            }

            public override Encoding Encoding
            {
                get { return this.writer.Encoding; }
            }
        }
#endif
    }
}
