

namespace Graphviz4Net
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows;
    using Dot;
    using Dot.AntlrParser;
    using Graphs;

    /// <summary>
    /// Directs the process of building a layout. 
    /// </summary>
    /// <remarks>
    /// <para>
    ///     LayoutDirector delegates all the work to individual components, 
    ///     which are 
    ///     <list type="dot">
    ///         <item><see cref="IGraphToDotConverter"/></item>
    ///         <item><see cref="IDotRunner"/></item>
    ///         <item><see cref="IDotParser{TVertexId}"/></item>
    ///         <item><see cref="ILayoutBuilder{TVertexId}"/></item>
    ///     </list>
    /// </para>
    /// <para>
    ///     The process of building a graph is divided up into three stages: <see cref="StartBuilder"/>, 
    ///     <see cref="RunDot"/>, <see cref="BuildGraph"/>. 
    ///     Only the <see cref="BuildGraph"/> and <see cref="StartBuilder"/> stages invokes the <see cref="ILayoutBuilder{TVertexId}"/>; 
    ///     therefore, if the graph is built from GUI elements (e.g. WPF controls) these has to be 
    ///     run in the dispatcher thread. On contrary, the <see cref="RunDot"/> phase 
    ///     invokes only the <see cref="ILayoutBuilder{TVertexId}.GetSize"/> method so, if this method 
    ///     can run outsize the dispatcher thread (e.g., one can calculate the size in advance), 
    ///     then the invocation of <see cref="RunDot"/> (the actual layouting, which might be slow) can 
    ///     be run in parallel.
    /// </para>
    /// </remarks>
    public class LayoutDirector
    {
        private readonly ILayoutBuilder<int> builder;

        private readonly IDotParser<int> parser;

        private readonly IGraphToDotConverter converter;

        private readonly IDotRunner dotRunner;

        private IGraph originalGraph = null;

        private DotGraph<int> dotGraph = null;

        //private object[] originalGraphElementsMap = null;
        public object[] originalGraphElementsMap = null;

        /// <summary>
        /// Factory method: provides convenient way to construct <see cref="LayoutDirector"/>. 
        /// </summary>
        /// <remarks>
        /// <para>If an argument value is not provided (i.e., it's null), then
        /// default implementation of required interface is provided. See the 
        /// documentation of the constructor of this class for the list of 
        /// default implementations of these interfaces.</para>
        /// </remarks>
        public static LayoutDirector GetLayoutDirector(
            ILayoutBuilder<int> builder,
            IDotParser<int> parser = null, 
            IGraphToDotConverter converter = null,
            IDotRunner dotRunner = null)
        {
            Contract.Requires(builder != null);
            Contract.Ensures(Contract.Result<LayoutDirector>() != null);
            if (parser == null)
                parser = AntlrParserAdapter<int>.GetParser();
            if (converter == null)
                converter = new GraphToDotConverter();
            if (dotRunner == null)
            {
#if SILVERLIGHT
                dotRunner = new DotAutomationFactoryRunner();
#else
                dotRunner = new DotExeRunner();
#endif
            }

            return new LayoutDirector(builder, parser, converter, dotRunner);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutDirector"/> class.
        /// </summary>
        /// <param name="builder">The builder is responsible for building the actual graphical output.</param>
        /// <param name="parser">The parser pases DOT output, default 
        /// implementation is <see cref="AntlrParserAdapter{T}"/>. 
        /// Use factory method <see cref="AntlrParserAdapter{T}.GetParser()"/></param>
        /// <param name="converter">The converter converts the .NET graph into a 
        /// GraphViz representation. Default implementation is <see cref="GraphToDotConverter"/>.</param>
        /// <param name="dotRunner">The dot runner runs the DOT utility. 
        /// Default implementation is <see cref="DotExeRunner"/></param>
        private LayoutDirector(
            ILayoutBuilder<int> builder,
            IDotParser<int> parser, 
            IGraphToDotConverter converter, 
            IDotRunner dotRunner)
        {
            Contract.Requires(parser != null);
            Contract.Requires(converter != null);
            Contract.Requires(builder != null);
            Contract.Requires(dotRunner != null);
            this.builder = builder;
            this.parser = parser;
            this.converter = converter;
            this.dotRunner = dotRunner;
        }

        /// <summary>
        /// Starts the builder. If the builder requires to be run 
        /// in the dispatcher thread, this method must be run in it.
        /// </summary>
        /// <param name="graph">The graph to be layouted.</param>
        public void StartBuilder(IGraph graph)
        {
            this.dotGraph = null;
            this.originalGraph = graph;
            builder.Start(graph);            
        }

        /// <summary>
        /// Runs the dot program and parses it's output.         
        /// An invocation of <see cref="StartBuilder"/> must precede by a call to this method.
        /// </summary>
        /// <remarks>
        /// This method uses <see cref="IDotRunner"/> and <see cref="IDotParser{TVertexId}"/>, 
        /// it does not invoke any method on <see cref="ILayoutBuilder{TVertexId}"/>. 
        /// So in case the <see cref="ILayoutBuilder{TVertexId}"/> requires to run in dispatcher thread, 
        /// this method does not have to do so, unless the <see cref="IDotRunner"/> or 
        /// <see cref="IDotParser{TVertexId}"/> require it.
        /// </remarks>
        public DotGraph<int> RunDot()
        {
            if (this.originalGraph == null)
            {
                throw new InvalidOperationException(
                    "LayoutDirector: the RunDot method must be invoked before call to BuildGraph");
            }

            var reader = this.dotRunner.RunDot(
                writer => 
                    this.originalGraphElementsMap = 
                        this.converter.Convert(
                            writer, 
                            this.originalGraph, 
                            new AttributesProvider(builder)));
            this.dotGraph = this.parser.Parse(reader);
            return this.dotGraph;
        }

        // NS 05-12 trying to solve the blinking issues...
        public void SetDot(DotGraph<int> dot, object[] oGraphElementsMap)
        {
            this.dotGraph = dot;
            this.originalGraphElementsMap = oGraphElementsMap;
        }


        /// <summary>
        /// Processes the graph parsed by <see cref="RunDot"/> and invokes methods on <see cref="ILayoutBuilder"/> 
        /// to build the actual layout. An invocation of <see cref="RunDot"/> must precede by a call to this method.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="InvalidFormatException"/>
        public void BuildGraph()
        {
            if (this.dotGraph == null)
            {
                throw new InvalidOperationException(
                    "LayoutDirector: the RunDot method must be invoked before call to BuildGraph");
            }

            if (this.dotGraph.Width.HasValue == false ||
                this.dotGraph.Height.HasValue == false)
            {
                throw new InvalidFormatException("Graph in dot output does not have width or height value set up.");
            }
            
            this.builder.BuildGraph(dotGraph.Width.Value, dotGraph.Height.Value, this.originalGraph, dotGraph);

            this.BuildVertices();
            this.BuildSubGraphs();
            this.BuildEdges();

            this.builder.Finish();
        }

        private void BuildEdges()
        {
            foreach (var edge in dotGraph.Edges)
            {
                if (edge is DotEdge<int>)
                {
                    var dotEdge = (DotEdge<int>) edge;
                    Contract.Assert(
                        0 <= dotEdge.Id && dotEdge.Id < this.originalGraphElementsMap.Length,
                        "The id of an edge is not in the range of originalGraphElementsMap.");
                    Contract.Assert(
                        this.originalGraphElementsMap[dotEdge.Id] is IEdge,
                        "The id of an edge does not point to an IEdge in originalGraphElementsMap.");
                    builder.BuildEdge(dotEdge.Path, (IEdge)this.originalGraphElementsMap[dotEdge.Id], dotEdge);
                }
            }
        }

        private void BuildSubGraphs()
        {
            foreach (var subGraph in dotGraph.SubGraphs)
            {
                if (subGraph.BoundingBox.HasAllValues)
                {
                    Contract.Assert(
                        0 <= subGraph.Id && subGraph.Id < this.originalGraphElementsMap.Length,
                        "The id of a subGraph is not in the range of originalGraphElementsMap.");
                    Contract.Assert(
                        this.originalGraphElementsMap[subGraph.Id] is ISubGraph,
                        "The id of an edge does not point to an IEdge in originalGraphElementsMap.");
                    builder.BuildSubGraph(
                        subGraph.BoundingBox.LeftX.Value,
                        subGraph.BoundingBox.UpperY.Value,
                        subGraph.BoundingBox.RightX.Value,
                        subGraph.BoundingBox.LowerY.Value,
                        (ISubGraph)this.originalGraphElementsMap[subGraph.Id],
                        subGraph);
                }
                else
                {
                    throw new InvalidFormatException(
                        string.Format(
                            "SubGraph '{0}' in dot layout output does not have bounding box (bb) set up.", 
                            subGraph.Label));
                }
            }
        }

        private void BuildVertices()
        {
            foreach (var vertex in dotGraph.AllVertices)
            {
                if (vertex.Position.HasValue &&
                    vertex.Width.HasValue &&
                    vertex.Height.HasValue)
                {
                    Contract.Assert(
                        0 <= vertex.Id && vertex.Id < this.originalGraphElementsMap.Length,
                        "The id of a vertex is not in the range of originalGraphElementsMap.");
                    builder.BuildVertex(
                        vertex.Position.Value,
                        vertex.Width.Value,
                        vertex.Height.Value,
                        this.originalGraphElementsMap[vertex.Id],
                        vertex);
                }
                else
                {
                    throw new InvalidFormatException(
                        string.Format(
                            "Vertex '{0}' in dot layout output does not have position, width or height set.", 
                            vertex));
                }
            }
        }

        public class InvalidFormatException : Graphviz4NetException
        {
            public InvalidFormatException(string message, Exception inner = null)
                : base(message, inner)
            {
            }
        }

        private class AttributesProvider : IAttributesProvider
        {
            private readonly ILayoutBuilder<int> builder;

            public AttributesProvider(ILayoutBuilder<int> builder)
            {
                this.builder = builder;
            }

            public IDictionary<string, string> GetVertexAttributes(object vertex)
            {
                var result = new Dictionary<string, string>();
                Size size = this.builder.GetSize(vertex);
                result.Add("width", size.Width.ToInvariantString());
                result.Add("height", size.Height.ToInvariantString());
                result.Add("shape", "rect");
                result.Add("fixedsize", "true");

                foreach (var attribute in this.builder.GetAdditionalAttributes(vertex))
                {
                    result.Add(attribute.Key, attribute.Value);
                }

                return result;
            }
        }
    }
}
