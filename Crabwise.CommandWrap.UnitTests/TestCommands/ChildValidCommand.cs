namespace Crabwise.CommandWrap.UnitTests.TestCommands
{
    using Crabwise.CommandWrap;

    [CommandSyntax("child")]
    internal class ChildValidCommand : CommandWithOneParam
    {
        [ParameterSyntax("--child-para1 {arg}")]
        [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.CollectionConverter))]
        public System.Collections.ObjectModel.Collection<string> ChildParameter1 { get; set; }

        [ParameterSyntax("--child-para2 {arg}")]
        public string ChildParameter2 { get; set; }
    }
}
