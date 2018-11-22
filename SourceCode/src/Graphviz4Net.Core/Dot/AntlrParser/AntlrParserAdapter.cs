
namespace Graphviz4Net.Dot.AntlrParser
{
    using System;
    using System.IO;
    using Antlr.Runtime;

    /// <summary>
    /// This adapter provides simple interface to the ANTLR parsing pipeline. 
    /// </summary>
    /// <typeparam name="TVertexId"></typeparam>
    public class AntlrParserAdapter<TVertexId> : IDotParser<TVertexId>
    {
        private readonly DotGraphBuilder<TVertexId> builder;

        /// <summary>
        /// Returns an instance of <see cref="AntlrParserAdapter{TVertexId}"/> composed with 
        /// graph builder for given type of node's ids. At the moment only <see cref="int"/> and 
        /// <see cref="string"/> ids are supported.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use <see cref="string"/> as <see cref="TVertexId"/> if you want arbitrary ids or support for 
        /// 'node:record' syntax when defining edges. Use <see cref="int"/> as <see cref="TVertexId"/> if 
        /// you want better performance, but in such case ids of nodes in your graph must be integral and 
        /// 'node:record' syntax is not supported.
        /// </para>
        /// <para>
        /// To implement custom builder, one must implement <see cref="IDotGraphBuilder"/> (or 
        /// abstract class <see cref="DotGraphBuilder{TVertexId}"/>) and use the other overload of this method.
        /// </para>
        /// </remarks>
        public static AntlrParserAdapter<TVertexId> GetParser()
        {
            if (typeof(TVertexId) == typeof(int))
            {
                return new AntlrParserAdapter<int>(new IntDotGraphBuilder()) as AntlrParserAdapter<TVertexId>;
            }

            if (typeof(TVertexId) == typeof(string))
            {
                return new AntlrParserAdapter<string>(new StringDotGraphBuilder()) as AntlrParserAdapter<TVertexId>;
            }

            var msg = string.Format(
                "AntlrParserAdapter does not provide an implementation for type {0}." + 
                "Only int and string types are supported out of the box. " + 
                "If you want to implement parser for a different type of vertices id, " + 
                "you must implement abstract class DotGraphBuilder and use the " +
                "other overload of this factory method, which accepts an instance of DotGraphBuilder " + 
                "as an argument.",
                typeof (TVertexId).Name);
            throw new NotImplementedException(msg);
        }

        /// <summary>
        /// Returns an instance of <see cref="AntlrParserAdapter{TVertexId}"/> composed with 
        /// given graph builder.
        /// </summary>
        public static AntlrParserAdapter<TVertexId> GetParser(DotGraphBuilder<TVertexId> builder)
        {
            return new AntlrParserAdapter<TVertexId>(builder);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AntlrParserAdapter{TVertexId}"/> class.
        /// This constructor is private, use <see cref="GetParser()"/> or 
        /// <see cref="GetParser(DotGraphBuilder{TVertexId})"/> factory methods instead.
        /// </summary>
        private AntlrParserAdapter(DotGraphBuilder<TVertexId> builder)
        {
            this.builder = builder;
        }

        public DotGraph<TVertexId> Parse(TextReader reader)
        {
            var antlrStream = new ANTLRReaderStream(reader);
            var lexer = new DotGrammarLexer(antlrStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new DotGrammarParser(tokenStream);
            parser.Builder = this.builder;
            parser.dot();
            return this.builder.DotGraph;
        }

        /// <summary>
        /// Convenient method for parsing strings. 
        /// For betters performance, when you read graph from a file use, 
        /// consider using <see cref="Parse(TextReader)"/> instead.
        /// </summary>
        public DotGraph<TVertexId> Parse(string graph)
        {
            var reader = new StringReader(graph);
            return this.Parse(reader);
        }
    }
}
