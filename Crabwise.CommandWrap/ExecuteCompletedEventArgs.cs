namespace Crabwise.CommandWrap
{
    using System;

    public class ExecuteCompletedEventArgs : EventArgs
    {
        public ExecuteCompletedEventArgs(int exitCode)
        {
            this.ExitCode = exitCode;
        }

        public int ExitCode { get; private set; }
    }
}