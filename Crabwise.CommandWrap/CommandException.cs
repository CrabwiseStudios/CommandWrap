namespace Crabwise.CommandWrap
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class CommandException : Exception
    {
        private readonly Command command = null;

        public CommandException()
        {
        }

        public CommandException(string message)
            : base(message)
        {
        }

        public CommandException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public CommandException(string message, Exception inner, Command command)
            : base(message, inner)
        {
            this.command = command;
        }

        protected CommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public Command Command
        {
            get
            {
                return this.command;
            }
        }
    }
}