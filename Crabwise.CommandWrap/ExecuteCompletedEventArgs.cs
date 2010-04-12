namespace Crabwise.CommandWrap
{
    using System;

    /// <summary>
    /// EventArg that handles the exit code of a process
    /// </summary>
    public class ExecuteCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ExecuteCompletedEventArgs class.
        /// </summary>
        /// <param name="exitCode">The exit code of the process</param>
        public ExecuteCompletedEventArgs(int exitCode)
        {
            this.ExitCode = exitCode;
        }

        /// <summary>
        /// Gets the exit code property of this object.
        /// </summary>
        public int ExitCode { get; private set; }
    }
}