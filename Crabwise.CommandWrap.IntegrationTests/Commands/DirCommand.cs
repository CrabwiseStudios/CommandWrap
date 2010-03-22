namespace Crabwise.CommandWrap.IntegrationTests
{
    using Crabwise.CommandWrap.Library;
    using System.Collections.ObjectModel;

    [CommandSyntax("dir")]
    internal sealed class DirCommand : CommandPromptCommand
    {
        private readonly Collection<string> paths = new Collection<string>();

        [ParameterSyntax("{arg}")]
        public Collection<string> Paths
        {
            get
            {
                return paths;
            }
        }
    }
}