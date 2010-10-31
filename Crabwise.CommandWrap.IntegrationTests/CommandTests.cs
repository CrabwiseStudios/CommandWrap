namespace Crabwise.CommandWrap.IntegrationTests
{
    using System.Collections;
    using Crabwise.CommandWrap;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    /// <summary>
    /// This is a test class for Command and is intended to contain all Command integration tests.
    /// </summary>
    [TestClass()]
    public class CommandTests
    {
        [TestMethod()]
        public void Execute_DirCommand_ExecutesSuccessfully()
        {
            var dirCommand = new DirCommand();
            dirCommand.CommandExecution = CommandPromptCommand.CommandOptions.C;
            dirCommand.Paths.Add(".");
            dirCommand.Execute();
        }

        [TestMethod()]
        public void Execute_IpConfigCommand_ExecutesSuccessfully()
        {
            var ipConfigCommand = new IpConfigCommand();
            ipConfigCommand.Execute();
            var output = ipConfigCommand.StandardOutput;
        }

        [TestMethod]
        public void StandardOutputWrittenGetsCorrectOutput()
        {
            var dirCommand = new DirCommand();
            dirCommand.CommandExecution = CommandPromptCommand.CommandOptions.C;
            dirCommand.Paths.Add(".");

            string actualOutput = string.Empty;
            dirCommand.StandardOutputWritten += (s, e) => actualOutput += (e.OutputLine + Environment.NewLine);

            dirCommand.Execute();
            Assert.AreEqual(dirCommand.StandardOutput, actualOutput);
        }
    }
}
