
namespace Graphiz4Net.Tests.Dot.AntlrParser
{
    using System.Linq;
    using Antlr.Runtime;
    using Graphviz4Net.Dot;
    using Graphviz4Net.Dot.AntlrParser;
    using NUnit.Framework;

    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void ParseNodesTest()
        {
            var graph = Parse("graph { 0; 1 [width=30] }");
            Assert.AreEqual(2, graph.Vertices.Count());
        }

        [Test]
        public void ParseNodeAttributes()
        {
            var graph = Parse("graph { 0 [width=30, height=\"45\", pos=\"e,1,2.45,3\"] }");
            Assert.IsNotNull(graph.Vertices.FirstOrDefault());
            CollectionAssert.AreEquivalent(
                new[] {"30", "45", "e,1,2.45,3"},
                graph.Vertices.First().Attributes.Select(x => x.Value).ToArray());
        }

        [Test]
        public void ParseEdge()
        {
            var graph = Parse("graph { 0 [width=30]; 1; 2; 0 -> 1; 1 -- 2; }");
            Assert.AreEqual(2, graph.Edges.Count());
            Assert.That(graph.Edges.Any(e => ((DotEdge<int>) e).Source.Id == 0 && ((DotEdge<int>) e).Destination.Id == 1));
            Assert.That(graph.Edges.Any(e => ((DotEdge<int>) e).Source.Id == 1 && ((DotEdge<int>) e).Destination.Id == 2));
        }

        [Test]
        public void ParseGraphAttributes()
        {
            var graph = Parse("graph { graph [width=30, height=\"39\"] }");
            Assert.AreEqual(2, graph.Attributes.Count);
            CollectionAssert.AreEquivalent(
                new[] { "30", "39" },
                graph.Attributes.Select(x => x.Value).ToArray());
        }

        [Test]
        public void ParseCompleteGraph1()
        {
            var graph = Parse(@"
                digraph g { 
                    graph [ratio=0.5 ];
                    0 [ width=""0.486111111111111"" ,height=""0.707777777777778"" ,shape=""rect"" ,fixesize=""true""  ];
                    1 [ width=""0.486111111111111"" ,height=""0.707777777777778"" ,shape=""rect"" ,fixesize=""true""  ];
                    2 [ width=""0.486111111111111"" ,height=""0.707777777777778"" ,shape=""rect"" ,fixesize=""true""  ];
                    3 [ width=""0.486111111111111"" ,height=""0.707777777777778"" ,shape=""rect"" ,fixesize=""true""  ];
                    4 [ width=""0.486111111111111"" ,height=""0.707777777777778"" ,shape=""rect"" ,fixesize=""true""  ];
                    5 [ width=""0.486111111111111"" ,height=""0.707777777777778"" ,shape=""rect"" ,fixesize=""true""  ];
                    0 -> 1 [  ];
                    0 -> 2 [  ];
                    0 -> 3 [  ];
                    1 -> 2 [  ];
                    1 -> 3 [  ];
                    2 -> 3 [  ];
                    4 -> 5 [  ];
                    4 -> 2 [  ];
                    2 -> 5 [  ];
                    1 -> 0 [  ];
                }");
            Assert.AreEqual(6, graph.Vertices.Count());
            Assert.AreEqual(10, graph.Edges.Count());
        }

        [Test]
        public void ParseCompleteGraph2()
        {
            var graph = Parse(@"
                    digraph g {
	                    graph [ratio=NaN];
	                    node [label=""\N""];
	                    graph [bb=""0,0,137.36,316""];
	                    0 [width=""0.47222"", height=""0.70833"", shape=rect, fixesize=true, pos=""30.358,290""];
	                    1 [width=""0.47222"", height=""0.70833"", shape=rect, fixesize=true, pos=""30.358,202""];
	                    2 [width=""0.47222"", height=""0.70833"", shape=rect, fixesize=true, pos=""75.358,114""];
	                    3 [width=""0.47222"", height=""0.70833"", shape=rect, fixesize=true, pos=""30.358,26""];
	                    4 [width=""0.47222"", height=""0.70833"", shape=rect, fixesize=true, pos=""120.36,202""];
	                    5 [width=""0.47222"", height=""0.70833"", shape=rect, fixesize=true, pos=""101.36,26""];
	                    0 -> 1 [pos=""e,24.154,227.74 24.142,264.07 23.627,255.84 23.485,246.58 23.714,237.78""];
	                    0 -> 2 [pos=""e,72.795,139.73 42.74,264.24 47.591,253.28 52.828,240.22 56.358,228 63.758,202.37 68.597,172.62 71.538,149.93""];
	                    0 -> 3 [pos=""e,21.72,51.699 16.967,264.49 11.953,253.56 6.8674,240.47 4.358,228 -7.2789,170.16 7.2997,101.9 18.863,61.38""];
	                    1 -> 2 [pos=""e,62.195,139.74 43.616,176.07 47.965,167.57 52.871,157.98 57.502,148.92""];
	                    1 -> 3 [pos=""e,30.358,51.623 30.358,176.41 30.358,146.3 30.358,96.106 30.358,62.062""];
	                    2 -> 3 [pos=""e,43.521,51.742 62.1,88.073 57.751,79.569 52.845,69.975 48.214,60.918""];
	                    4 -> 5 [pos=""e,104.12,51.623 117.6,176.41 114.33,146.17 108.88,95.671 105.2,61.621""];
	                    4 -> 2 [pos=""e,88.521,139.74 107.1,176.07 102.75,167.57 97.845,157.98 93.214,148.92""];
	                    2 -> 5 [pos=""e,93.753,51.742 83.018,88.073 85.478,79.748 88.246,70.378 90.872,61.49""];
	                    1 -> 0 [pos=""e,36.574,264.07 36.562,227.74 37.082,235.96 37.231,245.22 37.007,254.02""];
                    }");
            Assert.AreEqual(137.36, graph.Width);
            Assert.AreEqual(6, graph.Vertices.Count());
            Assert.AreEqual(10, graph.Edges.Count());
        }

        private static DotGraph<int> Parse(string content)
        {
            var antlrStream = new ANTLRStringStream(content);
            var lexer = new DotGrammarLexer(antlrStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new DotGrammarParser(tokenStream);
            var builder = new IntDotGraphBuilder();
            parser.Builder = builder;
            parser.dot();
            return builder.DotGraph;
        }
    }
}
