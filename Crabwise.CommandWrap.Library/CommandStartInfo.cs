namespace Crabwise.CommandWrap.Library
{
    using System.Diagnostics;
    using System.Security;

    /// <summary>
    /// Specifies a set of values that are used when you execute a command.
    /// </summary>
    /// <remarks>
    /// Properties of this class are passed to an instance of <see cref="System.Diagnostics.ProcessStartInfo"/> when 
    /// spawning the process for this command.
    /// </remarks>
    public sealed class CommandStartInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandStartInfo"/> class with empty properties.
        /// </summary>
        public CommandStartInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandStartInfo"/> class using a path.
        /// </summary>
        /// <param name="path">The path to use for executing a command.</param>
        public CommandStartInfo(string path)
        {
            this.Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandStartInfo"/> class using a 
        /// <see cref="ProcessStartInfo"/> object to initialize its properties.
        /// </summary>
        /// <param name="processStartInfo">The <see cref="ProcessStartInfo"/> object to use.</param>
        public CommandStartInfo(ProcessStartInfo processStartInfo)
        {
            this.CreateNoWindow = processStartInfo.CreateNoWindow;
            this.Domain = processStartInfo.Domain;
            this.LoadUserProfile = processStartInfo.LoadUserProfile;
            this.Password = processStartInfo.Password;
            this.RedirectStandardInput = processStartInfo.RedirectStandardInput;
            this.UserName = processStartInfo.UserName;
            this.WindowStyle = processStartInfo.WindowStyle;
            this.WorkingDirectory = processStartInfo.WorkingDirectory;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the CreateNoWindow property when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.CreateNoWindow"/>
        public bool CreateNoWindow { get; set; }

        /// <summary>
        /// Gets or sets the Domain property when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.Domain"/>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the LoadUserProfile property when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.LoadUserProfile"/>
        public bool LoadUserProfile { get; set; }

        /// <summary>
        /// Gets or sets the Password property when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.Password"/>
        public SecureString Password { get; set; }

        /// <summary>
        /// Gets or sets the path to use for executing a command.
        /// </summary>
        /// <remarks>
        /// This property overrides the DefaultPath attribute on any commands. It can be used to set the path of the 
        /// command if it isn't known at compile time.
        /// </remarks>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the RedirectStandardInput property when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.RedirectStandardInput"/>
        public bool RedirectStandardInput { get; set; }

        /// <summary>
        /// Gets or sets the UserName property when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.UserName"/>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the WindowStyle property when executing the command.
        /// </summary>
        /// <seealso cref="System.Diagnostics.ProcessStartInfo.WindowStyle"/>
        public ProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Gets or sets the working directory in which the command will execute.
        /// </summary>
        /// <remarks>
        /// This property overrides the DefaultWorkingDirectory attribute on any commands. It can be used to set the
        /// working directory of the command if it isn't known at compile time.
        /// </remarks>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Returns a <see cref="System.Diagnostics.ProcessStartInfo"/> object which has matching properties of this
        /// <see cref="CommandStartInfo"/> object.
        /// </summary>
        /// <returns><see cref="System.Diagnostics.ProcessStartInfo"/> object with matching properties.</returns>
        /// <remarks>
        /// Specifically, the returned <see cref="System.Diagnostics.ProcessStartInfo"/> object that is returned has 
        /// the same properties except for <see cref="CommandStartInfo.Path"/> and 
        /// <see cref="CommandStartInfo.WorkingDirectory"/>.
        /// </remarks>
        internal ProcessStartInfo GetProcessStartInfo()
        {
            return new ProcessStartInfo
            {
                CreateNoWindow = this.CreateNoWindow,
                Domain = this.Domain,
                LoadUserProfile = this.LoadUserProfile,
                Password = this.Password,
                RedirectStandardInput = this.RedirectStandardInput,
                UserName = this.UserName,
                WindowStyle = this.WindowStyle,
                WorkingDirectory = this.WorkingDirectory
            };
        }
    }
}