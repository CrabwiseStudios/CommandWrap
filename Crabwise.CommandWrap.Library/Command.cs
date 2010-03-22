namespace Crabwise.CommandWrap.Library
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Provides an abstract representation of a command prompt command.
    /// </summary>
    public abstract class Command
    {
        public void Execute()
        {
            var commandSyntaxAttribute = this.GetCommandSyntaxAttribute();
            SyntaxBuilder syntaxBuilder = new SyntaxBuilder();
            syntaxBuilder.AppendCommand(this);
            var arguments = syntaxBuilder.ToString();
            var fileName = Environment.ExpandEnvironmentVariables(syntaxBuilder.FileName);
            var workingDirectory = commandSyntaxAttribute.DefaultWorkingDirectory;
            var processStartInfo = new ProcessStartInfo
                {
                    Arguments = arguments,
                    FileName = fileName,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = workingDirectory
                };

            var process = Process.Start(processStartInfo);
        }

        /// <summary>
        /// Determines whether two <see cref="Command"/> objects have the same <see cref="String"/>.
        /// </summary>
        /// <param name="obj"><see cref="Object"/> to compare.</param>
        /// <returns><c>true</c> if obj is a <see cref="Command"/> and its <see cref="String"/> representation is the 
        /// same as this instance; otherwise, <c>false</c>.</returns>
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
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Gets the command prompt <see cref="String"/> representation for this <see cref="Command"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that contains the command prompt representation of this 
        /// <see cref="Command"/>.</returns>
        public override string ToString()
        {
            SyntaxBuilder syntaxBuilder = new SyntaxBuilder();
            syntaxBuilder.AppendCommand(this);

            return syntaxBuilder.FileName + ' ' + syntaxBuilder.ToString();
        }

        private CommandSyntaxAttribute GetCommandSyntaxAttribute()
        {
            var attributes = this.GetType().GetCustomAttributes(typeof(CommandSyntaxAttribute), true);
            if (attributes.Length == 0)
            {
                throw new SyntaxException("This command does not have a CommandSyntaxAttribute.");
            }

            return (CommandSyntaxAttribute)attributes[0];
        }
    }
}