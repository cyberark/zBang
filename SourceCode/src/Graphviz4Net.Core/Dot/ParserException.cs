
namespace Graphviz4Net.Dot.AntlrParser
{
    public class ParserException : Graphviz4NetException
    {
        public ParserException()
            : base("The Graphviz4Net parser cannot parse the file.")
        {            
        }

        public ParserException(string message) : base(message)
        {
        }
    }
}
