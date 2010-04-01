namespace Crabwise.CommandWrap
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class SyntaxException : Exception
    {
        private readonly SyntaxAttribute syntaxAttribute = null;

        public SyntaxException()
        {
        }

        public SyntaxException(string message)
            : base(message)
        {
        }

        public SyntaxException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public SyntaxException(string message, Exception inner, SyntaxAttribute syntaxAttribute)
            : base(message, inner)
        {
            this.syntaxAttribute = syntaxAttribute;
        }

        protected SyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public SyntaxAttribute SyntaxAttribute
        {
            get
            {
                return this.syntaxAttribute;
            }
        }
    }
}