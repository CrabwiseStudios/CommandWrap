namespace Crabwise.CommandWrap.UnitTests
{
    using System.Collections;
    using Crabwise.CommandWrap.Library;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// This is a test class for Parameter and is intended to contain all Parameter unit tests.
    /// </summary>
    [TestClass()]
    public class ParameterTests
    {
        [TestMethod()]
        public void ToString_StringArgument_UsesDefaultStringBehavior()
        {
            ParameterSyntaxAttribute attribute = new ParameterSyntaxAttribute("--param {arg}");
            Parameter target = new Parameter(attribute, "argument");

            string expected = "--param \"argument\"";
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToString_TrueBoolArgumentWithoutValue_UsesDefaultBoolBehavior()
        {
            ParameterSyntaxAttribute attribute = new ParameterSyntaxAttribute("--param");
            Parameter target = new Parameter(attribute, true);

            string expected = "--param";
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToString_FalseBoolArgumentWithoutValue_UsesDefaultBoolBehavior()
        {
            ParameterSyntaxAttribute attribute = new ParameterSyntaxAttribute("--param");
            Parameter target = new Parameter(attribute, false);

            string expected = "";
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToString_TrueBoolArgumentWithValue_UsesDefaultBoolBehavior()
        {
            ParameterSyntaxAttribute attribute = new ParameterSyntaxAttribute("--param {arg}");
            Parameter target = new Parameter(attribute, true);

            string expected = "--param \"True\"";
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToString_FalseBoolArgumentWithValue_UsesDefaultBoolBehavior()
        {
            ParameterSyntaxAttribute attribute = new ParameterSyntaxAttribute("--param {arg}");
            Parameter target = new Parameter(attribute, false);

            string expected = "--param \"False\"";
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToString_IEnumerableArgumentWithOneItem_UsesDefaultIEnumerableBehavior()
        {
            ParameterSyntaxAttribute attribute = new ParameterSyntaxAttribute("--param {arg}");
            IEnumerable argument = new string[] { "argument" };
            Parameter target = new Parameter(attribute, argument);

            string expected = "--param \"argument\"";
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ToString_IEnumerableArgumentWithTwoItems_UsesDefaultIEnumerableBehavior()
        {
            ParameterSyntaxAttribute attribute = new ParameterSyntaxAttribute("--param {arg}");
            IEnumerable argument = new string[] { "argument1", "argument2" };
            Parameter target = new Parameter(attribute, argument);

            string expected = "--param \"argument1\" \"argument2\"";
            string actual = target.ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}
