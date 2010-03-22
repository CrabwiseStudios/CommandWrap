namespace Crabwise.CommandWrap.Library
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Provides an abstract representation of a command prompt command.
    /// </summary>
    public abstract class Command
    {
        private Process process;

        public string ErrorOutput { get; private set; }

        public bool HasExecuted { get; private set; }

        public string StandardOutput { get; private set; }

        public int Execute()
        {
            if (this.HasExecuted)
            {
                // Need command exception.
                throw new Exception();
            }

            var commandSyntaxAttribute = this.GetCommandSyntaxAttribute();
            SyntaxBuilder syntaxBuilder = new SyntaxBuilder(this);
            var arguments = syntaxBuilder.Arguments;
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

            process = new Process { StartInfo = processStartInfo };
            var errorOutputBuilder = new StringBuilder();
            var standardOutputBuilder = new StringBuilder();
            process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorOutputBuilder.AppendLine(e.Data);
                    }
                };
            process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        standardOutputBuilder.AppendLine(e.Data);
                    }
                };

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();

            this.ErrorOutput = errorOutputBuilder.ToString();
            this.StandardOutput = standardOutputBuilder.ToString();
            this.HasExecuted = true;
            return process.ExitCode;
        }

        public void WriteToStandardIn(object input)
        {
            process.StandardInput.Write(input);
        }

        public void WriteToStandardIn(ICollection input)
        {
            foreach (var item in input)
            {
                process.StandardInput.WriteLine(item);
            }
        }

        public void WriteLineToStandardIn(object input)
        {
            process.StandardInput.WriteLine(input);
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
            SyntaxBuilder syntaxBuilder = new SyntaxBuilder(this);

            return syntaxBuilder.ToString();
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