namespace Crabwise.CommandWrap.UnitTests
{
    using System.Collections;
    using Crabwise.CommandWrap.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Crabwise.CommandWrap.UnitTests.TestCommands;

    /// <summary>
    /// This is a test class for Command and is intended to contain all Command unit tests.
    /// </summary>
    [TestClass()]
    public class CommandTests
    {
        [TestMethod()]
        public void ToString_StringArgument_UsesDefaultStringBehavior()
        {
            Command command = new CommandWithOneParam
                {
                    Parameter1 = "parameter 1"
                };
            command.Execute();

            //Assert.AreEqual(expected, actual);
        }
    }
}
