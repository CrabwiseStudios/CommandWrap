namespace Crabwise.CommandWrap
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CommandStartingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}
