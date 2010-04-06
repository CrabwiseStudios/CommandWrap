namespace Crabwise.CommandWrap
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;

    /// <summary>
    /// Provides an abstract representation of a command prompt command.
    /// </summary>
    public abstract class Command
    {
        public event EventHandler<ExecuteCompletedEventArgs> ExecuteCompleted;

        /// <summary>
        /// Default <see cref="CommandStartInfo"/> to use for default options when executing this command.
        /// </summary>
        private readonly CommandStartInfo defaultCommandStartInfo = new CommandStartInfo();

        private readonly StringBuilder errorOutputBuilder = new StringBuilder();

        private readonly StringBuilder standardOutputBuilder = new StringBuilder();

        /// <summary>
        /// The process in which this command runs.
        /// </summary>
        private Process process;

        /// <summary>
        /// Initializes a new instance of the Command class.
        /// </summary>
        public Command()
        {
            var commandSyntaxAttribute = this.GetCommandSyntaxAttribute();
            var path = commandSyntaxAttribute.DefaultPath;
            if (!string.IsNullOrEmpty(path))
            {
                path = Environment.ExpandEnvironmentVariables(path);
            }

            var workingDirectory = commandSyntaxAttribute.DefaultWorkingDirectory;
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Environment.ExpandEnvironmentVariables(workingDirectory);
            }

            this.defaultCommandStartInfo.Path = path;
            this.defaultCommandStartInfo.WorkingDirectory = workingDirectory;
            this.process = new Process();
        }

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

        public bool IsExecuting { get; private set; }

        /// <summary>
        /// Gets the output that was printed to standard output after the command was executed.
        /// </summary>
        public string StandardOutput { get; private set; }

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
        /// Gets a <see cref="CommandStartInfo"/> object which provides default options to use when executing 
        /// this command. Defaults to null (no options).
        /// </summary>
        public CommandStartInfo DefaultCommandStartInfo
        {
            get
            {
                return this.defaultCommandStartInfo;
            }
        }

        public void ExecuteAsync()
        {
            this.ExecuteAsync(this.defaultCommandStartInfo);
        }

        public void ExecuteAsync(CommandStartInfo startInfo)
        {
            this.process.Exited += this.OnExecuteCompleted;
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
            return this.Execute(this.defaultCommandStartInfo);
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

        private void OnExecuteCompleted(object sender, EventArgs e)
        {
            var exitCode = this.CleanupProcess();
            if (this.ExecuteCompleted != null)
            {
                this.ExecuteCompleted(this, new ExecuteCompletedEventArgs(exitCode));
            }
        }

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

        private void StartProcess(CommandStartInfo startInfo)
        {
            if (this.IsExecuting)
            {
                throw new CommandException("Cannot execute this command because it is already running.");
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
    }
}