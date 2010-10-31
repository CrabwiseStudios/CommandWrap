namespace Crabwise.CommandWrap
{
    using System;

    public sealed class OutputWrittenEventArgs : EventArgs
    {
        private readonly string outputLine;

        public OutputWrittenEventArgs(string outputLine)
        {
            this.outputLine = outputLine;
        }

        public string OutputLine
        {
            get
            {
                return this.outputLine;
            }
        }
    }
}
