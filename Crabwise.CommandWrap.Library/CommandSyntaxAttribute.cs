namespace Crabwise.CommandWrap.Library
{
    using System;

    /// <summary>
    /// Describes the syntax of a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CommandSyntaxAttribute : SyntaxAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSyntaxAttribute"/> class.
        /// </summary>
        /// <param name="syntax">The syntax of the command.</param>
        public CommandSyntaxAttribute(string syntax)
            : base(syntax)
        {
        }

        /// <summary>
        /// Gets or sets the default path for the command.
        /// </summary>
        public string DefaultPath { get; set; }

        /// <summary>
        /// Gets or sets the default working directory for the command.
        /// </summary>
        public string DefaultWorkingDirectory { get; set; }
    }
}