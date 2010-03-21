namespace Crabwise.CommandWrap.UnitTests
{
    using Crabwise.CommandWrap.Library;
    using Crabwise.CommandWrap.UnitTests.TestCommands;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for SyntaxBuilder and is intended to contain all SyntaxBuilder unit tests.
    /// </summary>
    [TestClass()]
    public class SyntaxBuilderTests
    {
        [TestMethod()]
        public void CommandWithOneParam_ArgumentProvided_BuildsCorrectly()
        {
            SyntaxBuilder target = new SyntaxBuilder();
            const int argument = 0;
            CommandWithOneParam command = new CommandWithOneParam
                {
                    Parameter1 = argument
                };

            string expected = "command --param1 \"" + argument.ToString() + '"';
            target.AppendCommand(command);
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CommandWithTwoParams_TwoArgumentProvided_BuildsCorrectly()
        {
            SyntaxBuilder target = new SyntaxBuilder();
            const int argument1 = 0;
            const int argument2 = 0;
            CommandWithTwoParams command = new CommandWithTwoParams
            {
                Parameter1 = argument1,
                Parameter2 = argument2
            };

            string expected = "command --param1 \"" + argument1.ToString() + "\" --param2 \"" +
                argument2.ToString() + '"';
            target.AppendCommand(command);
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CommandWithTwoParams_OneArgumentProvided_BuildsCorrectly()
        {
            SyntaxBuilder target = new SyntaxBuilder();
            const int argument1 = 0;
            CommandWithTwoParams command = new CommandWithTwoParams
            {
                Parameter1 = argument1
            };

            string expected = "command --param1 \"" + argument1.ToString() + '"';
            target.AppendCommand(command);
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CommandWithTwoParams_NoArgumentProvided_BuildsCorrectly()
        {
            SyntaxBuilder target = new SyntaxBuilder();
            CommandWithTwoParams command = new CommandWithTwoParams();

            string expected = "command";
            target.AppendCommand(command);
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void CommandWithRequiredParam_ArgumentProvided_BuildsCorrectly()
        {
            SyntaxBuilder target = new SyntaxBuilder();
            const int argument1 = 0;
            CommandWithRequiredParam command = new CommandWithRequiredParam
            {
                Parameter1 = argument1,
            };

            string expected = "command --param1 \"" + argument1.ToString() + '"';
            target.AppendCommand(command);
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [ExpectedException(typeof(SyntaxException))]
        public void CommandWithRequiredParam_ArgumentNotProvided_ExceptionThrown()
        {
            SyntaxBuilder target = new SyntaxBuilder();
            const int argument1 = 0;
            CommandWithRequiredParam command = new CommandWithRequiredParam();

            string expected = "command --param1 " + argument1.ToString();
            target.AppendCommand(command);
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}
