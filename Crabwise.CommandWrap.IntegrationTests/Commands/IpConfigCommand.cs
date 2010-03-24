namespace Crabwise.CommandWrap.IntegrationTests
{
    using Crabwise.CommandWrap.Library;
    using System.Collections.ObjectModel;

    [CommandSyntax("ipconfig", DefaultPath = @"C:\Windows\System32")]
    internal sealed class IpConfigCommand : Command
    {
    }
}