
using System;
using System.Collections.Generic;
using System.IO;
using Graphviz4Net.Dot;

namespace Graphviz4Net.Tests.Dot
{
	using NUnit.Framework;
	using Graphviz4Net.Graphs;

	[TestFixture]
	public class GraphToDotConverterTest
	{
		[Test]
		public void ConvertSimpleGraphWith2VerticesAndEdge()
		{
			var graph = new Graph<int>();
			graph.AddVertex(0);
			graph.AddVertex(1);
			graph.AddEdge(new Edge<int>(0, 1));

			var writer = new StringWriter();
			new GraphToDotConverter().Convert(writer, graph, new AttributesProvider());
			var result = writer.GetStringBuilder().ToString().Trim();

			StringAssert.Contains("0 -> 1", result, "output appears to not contain the edge");
		}

		private class AttributesProvider : IAttributesProvider 
		{
			public IDictionary<string, string> GetVertexAttributes(object vertex)
			{
				return new Dictionary<string, string>();
			}
		}
	}
}
