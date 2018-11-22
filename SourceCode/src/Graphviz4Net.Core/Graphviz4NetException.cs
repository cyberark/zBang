
namespace Graphviz4Net
{
    using System;

    public class Graphviz4NetException : Exception
    {
        public Graphviz4NetException(
            string message = "This Graphviz4Net exception does not specify any message.", 
            Exception innerException = null) 
            : base(message, innerException)
        {
        }
    }
}
