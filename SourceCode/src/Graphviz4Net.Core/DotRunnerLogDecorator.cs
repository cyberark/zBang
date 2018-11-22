
namespace Graphviz4Net
{
    using System;
    using System.IO;
    using System.Text;

    public class DotRunnerLogDecorator : IDotRunner
    {
        private readonly IDotRunner runner;

        private readonly string filename;

        public DotRunnerLogDecorator(IDotRunner runner, string filename = "tmp")
        {
            this.runner = runner;
            this.filename = filename;
        }

        public TextReader RunDot(Action<TextWriter> writeGraph)
        {
            using (var writer = new StringWriter()) 
            {
                writeGraph(writer);
                string graph = writer.GetStringBuilder().ToString();
                var graphFile = Path.Combine(Path.GetTempPath(), this.filename + ".dot");
                File.WriteAllText(graphFile, graph);

                // now we read the file and write it to the real process input.
                using (var reader = this.runner.RunDot(w => w.Write(graph)))
                {
                    // we read all output, save it into another file, and return it as a memory stream
                    var text = reader.ReadToEnd();

                    var layoutFile = Path.Combine(Path.GetTempPath(), this.filename + ".layout.dot");
                    File.WriteAllBytes(layoutFile, Encoding.UTF8.GetBytes(text));
                    return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text)));
                }
            }
        }
    }
}