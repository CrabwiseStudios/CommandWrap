namespace Crabwise.CommandWrap.UnitTests.TestCommands
{
    using Crabwise.CommandWrap;

    /// <summary>
    /// Valid command with 2 parameters. Parameter2 has a positioning of 1.
    /// </summary>
    [CommandSyntax("command")]
    internal class CommandWithTwoParams : Command
    {
        [ParameterSyntax("--param1 {arg}")]
        public object Parameter1 { get; set; }

        [ParameterSyntax("--param2 {arg}", Position = 1)]
        public object Parameter2 { get; set; }
    }
}
