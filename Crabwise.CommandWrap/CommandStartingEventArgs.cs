namespace Crabwise.CommandWrap
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CommandStartingEventArgs : EventArgs
    {
        private readonly CommandStartInfo startInfo;

        public CommandStartingEventArgs(CommandStartInfo startInfo)
        {
            this.startInfo = startInfo;
        }

        public bool Cancel { get; set; }

        public CommandStartInfo StartInfo
        {
            get
            {
                return this.startInfo;
            }
        }
    }
}
