namespace Crabwise.CommandWrap.UnitTests.TestCommands
{
    using Crabwise.CommandWrap;

    /// <summary>
    /// Valid command with 1 required parameter.
    /// </summary>
    [CommandSyntax("command")]
    internal class CommandWithRequiredParam : Command
    {
        [ParameterSyntax("--param1 {arg}", Required = true)]
        public object Parameter1 { get; set; }
    }
}
