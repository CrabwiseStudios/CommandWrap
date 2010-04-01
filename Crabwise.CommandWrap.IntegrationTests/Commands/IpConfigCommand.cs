namespace Crabwise.CommandWrap.IntegrationTests
{
    using Crabwise.CommandWrap;
    using System.Collections.ObjectModel;

    [CommandSyntax("ipconfig", DefaultPath = @"C:\Windows\System32")]
    internal sealed class IpConfigCommand : Command
    {
    }
}