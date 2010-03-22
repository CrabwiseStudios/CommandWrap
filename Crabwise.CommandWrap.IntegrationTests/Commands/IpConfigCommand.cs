namespace Crabwise.CommandWrap.IntegrationTests
{
    using Crabwise.CommandWrap.Library;
    using System.Collections.ObjectModel;

    [CommandSyntax("ipconfig")]
    internal sealed class IpConfigCommand : CommandPromptCommand
    {
    }
}