namespace Crabwise.CommandWrap
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class that defines the behavior of a SyntaxException.
    /// </summary>
    [Serializable]
    public class SyntaxException : Exception
    {
        /// <summary>
        /// The <see cref="SyntaxAttribute"/> associated with the exception.
        /// </summary>
        private readonly SyntaxAttribute syntaxAttribute = null;

        /// <summary>
        /// Initializes a new instance of the SyntaxException class.
        /// </summary>
        /// <para>The default constructor.</para>
        public SyntaxException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SyntaxException class.
        /// </summary>
        /// <para>This constructor takes in a custom message.</para>
        /// <param name="message">The message of the exception.</param>
        public SyntaxException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SyntaxException class. 
        /// </summary>
        /// <para>This constructor takes in a custom message and inner exception.</para>
        /// <param name="message">The message of the exception.</param>
        /// <param name="inner">The inner exception.</param>
        public SyntaxException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SyntaxException class.
        /// </summary>
        /// <para>Takes in a message associated with the exception, an inner exception, and a
        ///  related <see cref="SyntaxAttribute"/> object.</para>
        /// <param name="message">The message of the exception.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="syntaxAttribute">The <see cref="SyntaxAttribute"/> object associated with this exception.</param>
        public SyntaxException(string message, Exception inner, SyntaxAttribute syntaxAttribute)
            : base(message, inner)
        {
            this.syntaxAttribute = syntaxAttribute;
        }

        /// <summary>
        /// Initializes a new instance of the SyntaxException class.
        /// </summary>
        /// <para>Takes in serialized info over a stream.</para>
        /// <param name="info">The given serialized info.</param>
        /// <param name="context">Information about the stream and sender.</param>
        protected SyntaxException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the <see cref="SyntaxAttribute"/> object associated with the exception.
        /// </summary>
        public SyntaxAttribute SyntaxAttribute
        {
            get
            {
                return this.syntaxAttribute;
            }
        }
    }
}