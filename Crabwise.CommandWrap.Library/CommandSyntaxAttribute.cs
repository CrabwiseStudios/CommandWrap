namespace Crabwise.CommandWrap.Library
{
    using System;
    using System.IO;

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

        /// <summary>
        /// Gets the full path pointing to the file name indicated by this attribute.
        /// </summary>
        /// <returns>The full path file name.</returns>
        /// <remarks>
        /// The <see cref="System.String"/> returned by this method is calculated by combining the 
        /// <see cref="CommandSyntaxAttribute.DefaultPath"/> property and the <see cref="SyntaxAttribute.Syntax"/>
        /// property.
        /// </remarks>
        public string GetFullPath()
        {
            string fileName;
            if (this.DefaultPath == null)
            {
                fileName = this.Syntax;
            }
            else
            {
                fileName = Path.Combine(this.DefaultPath, this.Syntax);
            }

            return fileName;
        }
    }
}