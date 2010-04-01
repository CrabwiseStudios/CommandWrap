namespace Crabwise.CommandWrap
{
    using System;

    /// <summary>
    /// Describes the syntax of a command prompt element.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SyntaxAttribute : Attribute
    {
        /// <summary>
        /// Syntax of the element.
        /// </summary>
        private readonly string syntax;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxAttribute"/> class.
        /// </summary>
        /// <param name="syntax">The syntax of the element.</param>
        public SyntaxAttribute(string syntax)
        {
            this.syntax = syntax;
        }

        /// <summary>
        /// Gets the syntax of the element.
        /// </summary>
        public string Syntax
        {
            get { return this.syntax; }
        }
    }
}