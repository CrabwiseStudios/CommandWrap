namespace Crabwise.CommandWrap
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception that is thrown when a <see cref="Command"/> tries to do something that conflicts with its current state.
    /// </summary>
    [Serializable]
    public class CommandException : Exception
    { 
        /// <summary>
        /// The <see cref="Command"/> object associated with the Exception.
        /// </summary>
        private readonly Command command = null;

        /// <summary>
        /// Initializes a new instance of the CommandException class.
        /// </summary>
        public CommandException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandException class. 
        /// </summary>
        /// <para>Takes in an error message associated with the exception.</para>
        /// <param name="message">The error message associated with the exception.</param>
        public CommandException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandException class.
        /// </summary>
        /// <para>Takes in an error message and an inner exception.</para>
        /// <param name="message">The error message associated with the exception.</param>
        /// <param name="inner">The inner exception.</param>
        public CommandException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CommandException class.
        /// </summary>
        /// <para>Takes in an error message, an inner exception, and a <see cref="Command"/> object.</para>
        /// <param name="message">The error message associated with the exception.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="command">The <see cref="Command"/> object associated with the exception.</param>
        public CommandException(string message, Exception inner, Command command)
            : base(message, inner)
        {
            this.command = command;
        }

        /// <summary>
        /// Initializes a new instance of the CommandException class.
        /// </summary>
        /// <para>Takes in serialized info over a stream.</para>
        /// <param name="info">The given serialized info.</param>
        /// <param name="context">Information about the stream and sender.</param>
        protected CommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the exception's command object.
        /// </summary>
        public Command Command
        {
            get
            {
                return this.command;
            }
        }
    }
}