namespace Crabwise.CommandWrap.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Provides an abstract representation of a command prompt command.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// The process in which this command runs.
        /// </summary>
        private Process process;

        /// <summary>
        /// Gets the output that was printed to standard error after the command was executed.
        /// </summary>
        public string ErrorOutput { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not this command has ben executed.
        /// </summary>
        /// <remarks>
        /// After a command has been executed it can not be executed again.
        /// </remarks>
        public bool HasExecuted { get; private set; }

        /// <summary>
        /// Gets the output that was printed to standard output after the command was executed.
        /// </summary>
        public string StandardOutput { get; private set; }

        /// <summary>
        /// Closes the standard input to this command.
        /// </summary>
        public void CloseStandardInput()
        {
            this.process.StandardInput.Close();
        }

        /// <summary>
        /// Executes this command with the default <see cref="CommandStartInfo"/> for this <see cref="Command"/>.
        /// </summary>
        /// <returns>The exit code of the process.</returns>
        /// <remarks>
        /// The default <see cref="CommandStartInfo"/> options can be set by deriving from this class and then 
        /// overriding the <see cref="Command.GetDefaultCommandStartInfo"/> method.
        /// </remarks>
        public int Execute()
        {
            return this.Execute(this.GetDefaultCommandStartInfo());
        }

        /// <summary>
        /// Uses a <see cref="CommandStartInfo"/> object to execute this command.
        /// </summary>
        /// <param name="startInfo">Specifies how to execute this command.</param>
        /// <returns>The exit code of the process.</returns>
        public int Execute(CommandStartInfo startInfo)
        {
            if (this.HasExecuted)
            {
                throw new CommandException("This command has already been executed.", null, this);
            }

            var commandSyntaxAttribute = this.GetCommandSyntaxAttribute();
            SyntaxBuilder syntaxBuilder = new SyntaxBuilder(this);
            var arguments = syntaxBuilder.Arguments;
            string fileName = Environment.ExpandEnvironmentVariables(syntaxBuilder.FileName);
            string workingDirectory = commandSyntaxAttribute.DefaultWorkingDirectory;
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Environment.ExpandEnvironmentVariables(workingDirectory);
            }

            if (startInfo != null)
            {
                if (!string.IsNullOrEmpty(startInfo.Path))
                {
                    var path = Path.Combine(startInfo.Path, commandSyntaxAttribute.Syntax);
                    fileName = Environment.ExpandEnvironmentVariables(path);
                }

                if (!string.IsNullOrEmpty(startInfo.WorkingDirectory))
                {
                    workingDirectory = Environment.ExpandEnvironmentVariables(startInfo.WorkingDirectory);
                }
            }

            var processStartInfo = new ProcessStartInfo();
            if (startInfo != null)
            {
                processStartInfo = startInfo.GetProcessStartInfo();
            }

            processStartInfo.Arguments = arguments;
            processStartInfo.FileName = fileName;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = workingDirectory;

            this.process = new Process { StartInfo = processStartInfo };
            var errorOutputBuilder = new StringBuilder();
            var standardOutputBuilder = new StringBuilder();
            this.process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        errorOutputBuilder.AppendLine(e.Data);
                    }
                };
            this.process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        standardOutputBuilder.AppendLine(e.Data);
                    }
                };

            this.process.Start();
            this.process.BeginErrorReadLine();
            this.process.BeginOutputReadLine();
            this.process.WaitForExit();

            this.ErrorOutput = errorOutputBuilder.ToString();
            this.StandardOutput = standardOutputBuilder.ToString();
            this.HasExecuted = true;
            return this.process.ExitCode;
        }

        /// <summary>
        /// Gets the command prompt <see cref="String"/> representation for this <see cref="Command"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that contains the command prompt representation of this 
        /// <see cref="Command"/>.</returns>
        public string GetSyntax()
        {
            SyntaxBuilder syntaxBuilder = new SyntaxBuilder(this);

            return syntaxBuilder.ToString();
        }

        /// <summary>
        /// Writes a <see cref="System.String"/> to the standard input of this command.
        /// </summary>
        /// <param name="input"><see cref="System.String"/> to write.</param>
        /// <remarks>
        /// This method can only be used after <see cref="Command.Execute()"/> has been called.
        /// </remarks>
        public void WriteToStandardIn(string input)
        {
            this.process.StandardInput.Write(input);
        }

        /// <summary>
        /// Writes a collection of strings to the standard input of this command.
        /// </summary>
        /// <param name="input"><see cref="System.Collections.Generic.ICollection{T}"/> of strings to write.</param>
        /// <remarks>
        /// This method can only be used after <see cref="Command.Execute()"/> has been called.
        /// </remarks>
        public void WriteToStandardIn(ICollection<string> input)
        {
            foreach (var item in input)
            {
                this.process.StandardInput.WriteLine(item);
            }
        }

        /// <summary>
        /// Writes a <see cref="System.String"/> followed by a newline to the standard input of this command.
        /// </summary>
        /// <param name="input"><see cref="System.String"/> to write.</param>
        /// <remarks>
        /// This method can only be used after <see cref="Command.Execute()"/> has been called.
        /// </remarks>
        public void WriteLineToStandardIn(string input)
        {
            this.process.StandardInput.WriteLine(input);
        }

        /// <summary>
        /// Can be overriden to provide default <see cref="CommandStartInfo"/> options for this <see cref="Command"/>.
        /// When not overridden, the <see cref="CommandStartInfo"/> defaults to null (no options).
        /// </summary>
        /// <returns>The default <see cref="CommandStartInfo"/> object to use when executing this command.</returns>
        protected virtual CommandStartInfo GetDefaultCommandStartInfo()
        {
            return null;
        }

        /// <summary>
        /// Gets the <see cref="CommandSyntaxAttribute"/> that adorns this command.
        /// </summary>
        /// <returns><see cref="CommandSyntaxAttribute"/> attached to this command.</returns>
        /// <remarks>
        /// This method throws a SyntaxException if it cannot find a <see cref="CommandSyntaxAttribute"/>. This is 
        /// generally wanted behavior since all classes implementing <see cref="Command"/> are required to have this 
        /// attribute.
        /// </remarks>
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