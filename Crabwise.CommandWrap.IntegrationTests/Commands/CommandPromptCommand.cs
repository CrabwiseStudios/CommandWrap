﻿namespace Crabwise.CommandWrap.IntegrationTests
{
    using Crabwise.CommandWrap.Library;
    using System.Collections.ObjectModel;

    [CommandSyntax("cmd.exe", DefaultPath = @"%windir%\system32\")]
    internal class CommandPromptCommand : Command
    {
        public enum CommandOptions
        {
            C,
            K
        }

        [ParameterSyntax("/{arg}")]
        public CommandOptions? CommandExecution { get; set; }
    }
}