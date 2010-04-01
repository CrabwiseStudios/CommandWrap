namespace Crabwise.CommandWrap.UnitTests.TestCommands
{
    using Crabwise.CommandWrap;

    /// <summary>
    /// Valid command with 1 parameter.
    /// </summary>
    [CommandSyntax("command")]
    internal class CommandWithOneParam : Command
    {
        [ParameterSyntax("--param1 {arg}")]
        public object Parameter1 { get; set; }
    }
}
