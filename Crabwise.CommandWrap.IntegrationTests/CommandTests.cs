namespace Crabwise.CommandWrap.IntegrationTests
{
    using System.Collections;
    using Crabwise.CommandWrap.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for Command and is intended to contain all Command integration tests.
    /// </summary>
    [TestClass()]
    public class CommandTests
    {
        [TestMethod()]
        public void ToString_StringArgument_UsesDefaultStringBehavior()
        {
            var dirCommand = new DirCommand();
            dirCommand.CommandExecution = CommandPromptCommand.CommandOptions.K;
            dirCommand.Paths.Add(".");
            dirCommand.Execute();

            //Assert.AreEqual(expected, actual);
        }
    }
}
