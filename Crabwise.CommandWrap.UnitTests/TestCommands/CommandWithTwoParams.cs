namespace Crabwise.CommandWrap.UnitTests.TestCommands
{
    using Crabwise.CommandWrap.Library;

    /// <summary>
    /// Valid command with 2 parameters. Parameter1 has a priority of 1.
    /// </summary>
    [CommandSyntax("command")]
    internal class CommandWithTwoParams : Command
    {
        [ParameterSyntax("--param1 {arg}", Priority = 1)]
        public object Parameter1 { get; set; }

        [ParameterSyntax("--param2 {arg}")]
        public object Parameter2 { get; set; }
    }
}
