
namespace Graphviz4Net
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class DotExeRunner : IDotRunner
    {
        public DotExeRunner()
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
            var process = new Process();
            try
            {
                process.StartInfo = new ProcessStartInfo
                                        {
                                            CreateNoWindow = true,
                                            UseShellExecute = false,
                                            Arguments = "-Tdot",
                                            FileName = Path.Combine(this.DotExecutablePath, this.DotExecutable),
                                            RedirectStandardOutput = true,
                                            RedirectStandardInput = true
                                        };
                process.Start();
                process.StandardInput.AutoFlush = true;
                writeGraph(process.StandardInput);
                process.StandardInput.Close();
                return process.StandardOutput;
            }
            catch (Exception ex)
            {
                var msg = string.Format(
                    "An exception occurred in DotExeRunner.RunDot(Action<StreamWriter>)." + 
                    "Check if you have GrahpViz installed on your machine. " + 
                    "Check that the folder where file 'dot.exe' is (part of GraphViz installation) is " + 
                    "either in the system PATH variable or you must set it as value of the DotExecutablePath property." + 
                    "Original exception type {0} and message: {1}", 
                    ex.GetType().Name,
                    ex.Message);
                throw new Exception(msg, ex);
            }
            finally
            {
                process.Dispose();
            }
        }

        public class Exception : System.Exception
        {
            public Exception(string message, System.Exception innerException) 
                : base(message, innerException)
            {
            }
        }
    }
}