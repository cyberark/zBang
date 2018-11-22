

namespace Graphviz4Net
{
    using System;
    using System.IO;
    using System.Text;
    using System.Runtime.InteropServices.Automation;

    public class DotAutomationFactoryRunner : IDotRunner
    {
        public DotAutomationFactoryRunner()
        {
            this.DotExecutablePath = string.Empty;
            this.DotExecutable = "dot.exe";
        }

        /// <summary>
        /// Gets or sets the path to the GraphViz executable file (dot.exe).
        /// </summary>
        public string DotExecutablePath { get; set; }

        /// <summary>
        /// Gets or sets the name of the executable file that should be used.
        /// </summary>
        public string DotExecutable { get; set; }

        public TextReader RunDot(Action<TextWriter> writeGraph)
        {
            var command = string.Format(
                "\"{0}\"", 
                Path.Combine(this.DotExecutablePath, this.DotExecutable));

            dynamic cmd = AutomationFactory.CreateObject("WScript.Shell");
            dynamic exec = cmd.Exec(command);

            // Note: implementation with custom TextWriter that called Write(char) 
            // on exec.StdIn did not work (couldn't find a cause for that).
            var writer = new StringWriter();
            writeGraph(writer);
            exec.StdIn.Write(writer.GetStringBuilder().ToString());
            exec.StdIn.Close();

            return new StdOutReader(exec.StdOut);
        }

        private class StdOutReader : TextReader
        {
            private readonly dynamic stdout;

            private string line = string.Empty;

            private int i = 0;

            public StdOutReader(dynamic stdout)
            {
                this.stdout = stdout;
            }

            public override int Read()
            {
                int result = this.Peek();
                i++;
                return result;
            }

            public override int Peek()
            {
                // Note: method Read(1) of WScript.Shell.StdOut does not seem to work property,
                // therefore we use ReadLine instead

                if (this.stdout.AtEndOfStream)
                {
                    return -1;
                }

                if (this.i == this.line.Length)
                {
                    this.i = 0;
                    this.line = this.stdout.ReadLine() + "\r\n";
                }

                return this.line[this.i];                
            }
        }
    }
}
