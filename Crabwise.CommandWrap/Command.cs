namespace Crabwise.CommandWrap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Remoting.Messaging;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Provides an abstract representation of a command prompt command.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Builds a string from the error output messages sent by the command.
        /// </summary>
        private readonly StringBuilder errorOutputBuilder = new StringBuilder();

        /// <summary>
        /// Builds a string from the standard output messages sent by the command.
        /// </summary>
        private readonly StringBuilder standardOutputBuilder = new StringBuilder();

        /// <summary>
        /// The process in which this command runs.
        /// </summary>
        private Process process = new Process();

        /// <summary>
        /// Event fired after the command is executed.
        /// </summary>
        public event EventHandler<ExecuteCompletedEventArgs> ExecuteCompleted;

        public event EventHandler<CommandStartingEventArgs> CommandStarting;

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
        /// Gets a value indicating whether the command is executing or not.
        /// </summary>
        public bool IsExecuting { get; private set; }

        /// <summary>
        /// Gets the output that was printed to standard output after the command was executed.
        /// </summary>
        public string StandardOutput { get; private set; }

        /// <summary>
        /// Cancels the running command. Throws an exception if called on a command that is not executing.
        /// </summary>
        public void CancelAsync()
        {
            if (this.IsExecuting)
            {
                this.process.CloseMainWindow();
                this.process.Close();
            }
            else
            {
                throw new CommandException("Cannot cancel this command because it isn't executing.");
            }
        }

        /// <summary>
        /// Executes the command with default starting parameters.
        /// </summary>
        public void ExecuteAsync()
        {
            this.ExecuteAsync(new CommandStartInfo());
        }

        /// <summary>
        /// Executes the command with custom starting parameters.
        /// </summary>
        /// <param name="startInfo">The CommandStartInfo object that defines how the command should start</param>
        public void ExecuteAsync(CommandStartInfo startInfo)
        {
            this.process.Exited += this.ProcessExited;
            this.StartProcess(startInfo);
        }

        /// <summary>
        /// Closes the standard input to this command.
        /// </summary>
        public void CloseStandardInput()
        {
            try
            {
                this.process.StandardInput.Close();
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Executes this command with the default <see cref="CommandStartInfo"/> for this <see cref="Command"/>.
        /// </summary>
        /// <returns>The exit code of the process.</returns>
        /// <remarks>
        /// The default <see cref="CommandStartInfo"/> options can be set by deriving from this class and then 
        /// overriding the <see cref="Command.DefaultCommandStartInfo"/> property.
        /// </remarks>
        public int Execute()
        {
            return this.Execute(new CommandStartInfo());
        }

        /// <summary>
        /// Uses a <see cref="CommandStartInfo"/> object to execute this command.
        /// </summary>
        /// <param name="startInfo">Specifies how to execute this command.</param>
        /// <returns>The exit code of the process.</returns>
        public int Execute(CommandStartInfo startInfo)
        {
            this.StartProcess(startInfo);

            this.process.WaitForExit();

            return this.CleanupProcess();
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

        private void ProcessExited(object sender, EventArgs e)
        {
            this.OnExecuteCompleted();
        }

        /// <summary>
        /// Called when the command has finished running.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event argument</param>
        protected void OnExecuteCompleted()
        {
            var exitCode = this.CleanupProcess();
            if (this.ExecuteCompleted != null)
            {
                this.ExecuteCompleted(this, new ExecuteCompletedEventArgs(exitCode));
            }
        }

        protected void OnCommandStarting(CommandStartingEventArgs e)
        {
            if (this.CommandStarting != null)
            {
                this.CommandStarting(this, e);
            }
        }

        /// <summary>
        /// Handles all the finishing tasks within the command.
        /// </summary>
        /// <returns>The exit code that the process returned</returns>
        private int CleanupProcess()
        {
            this.CloseStandardInput();
            this.ErrorOutput = this.errorOutputBuilder.ToString();
            this.StandardOutput = this.standardOutputBuilder.ToString();
            var exitCode = this.process.ExitCode;
            this.process.Close();
            this.HasExecuted = true;
            return exitCode;
        }

        /// <summary>
        /// Starts the process with customized starting information
        /// </summary>
        /// <param name="startInfo">CommandStartInfo object that defines how this command should start</param>
        private void StartProcess(CommandStartInfo startInfo)
        {
            if (this.IsExecuting)
            {
                throw new CommandException("Cannot execute this command because it is already running.");
            }

            if (this.CommandStarting != null)
            {
                var e = new CommandStartingEventArgs();
                this.OnCommandStarting(e);
                if (e.Cancel)
                {
                    return;
                }
            }

            this.IsExecuting = true;

            if (this.HasExecuted)
            {
                throw new CommandException("This command has already been executed.", null, this);
            }

            if (startInfo == null)
            {
                throw new ArgumentNullException("startInfo");
            }

            var commandSyntaxAttribute = this.GetCommandSyntaxAttribute();
            SyntaxBuilder syntaxBuilder = new SyntaxBuilder(this);
            var arguments = syntaxBuilder.Arguments;

            ProcessStartInfo processStartInfo = startInfo.GetProcessStartInfo(syntaxBuilder.FileName);
            processStartInfo.Arguments = arguments;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;

            this.process.StartInfo = processStartInfo;
            this.process.ErrorDataReceived += (s, e) => this.errorOutputBuilder.AppendLine(e.Data);
            this.process.OutputDataReceived += (s, e) => this.standardOutputBuilder.AppendLine(e.Data);

            this.process.Start();
            this.process.BeginErrorReadLine();
            this.process.BeginOutputReadLine();
            this.process.EnableRaisingEvents = true;
        }

        public void WaitForExit()
        {
            this.process.WaitForExit();
        }
    }
}