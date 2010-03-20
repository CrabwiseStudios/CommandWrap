namespace Crabwise.CommandWrap.Library
{
    using System;

    /// <summary>
    /// Provides an abstract representation of a command prompt command.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Determines whether two <see cref="Command"/> objects have the same <see cref="String"/>.
        /// </summary>
        /// <param name="obj"><see cref="Object"/> to compare.</param>
        /// <returns><b>true</b> if obj is a <see cref="Command"/> and its <see cref="String"/> representation is the 
        /// same as this instance; otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            var command = obj as Command;
            if (command == null)
            {
                return false;
            }

            return command.ToString() == this.ToString();
        }

        /// <summary>
        /// Gets the hash code for the Command.
        /// </summary>
        /// <returns>An <see cref="Int32"/> containing the hash value generated for this command.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets the command prompt <see cref="String"/> representation for this <see cref="Command"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that contains the command prompt representation of this 
        /// <see cref="Command"/>.</returns>
        public override string ToString()
        {
            return base.ToString();
        }

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
            public CommandSyntaxAttribute(string syntax) : base(syntax)
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

        /// <summary>
        /// Describes the syntax of a command parameter.
        /// </summary>
        [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
        public sealed class ParameterSyntaxAttribute : SyntaxAttribute
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterSyntaxAttribute"/> class.
            /// </summary>
            /// <param name="syntax">The syntax of the parameter.</param>
            public ParameterSyntaxAttribute(string syntax)
                : base(syntax)
            {
            }

            /// <summary>
            /// Gets or sets the ordering priority of this parameter. Parameters with higher priorities are placed 
            /// first in the command prompt string.
            /// </summary>
            public int Priority { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this parameter is required.
            /// </summary>
            public bool Required { get; set; }
        }

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
}