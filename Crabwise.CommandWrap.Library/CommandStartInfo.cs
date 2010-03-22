namespace Crabwise.CommandWrap.Library
{
    using System.Diagnostics;
    using System.Security;

    public sealed class CommandStartInfo
    {
        public bool CreateNoWindow { get; set; }

        public string Domain { get; set; }

        public bool LoadUserProfile { get; set; }

        public SecureString Password { get; set; }

        public string Path { get; set; }

        public string UserName { get; set; }

        public ProcessWindowStyle WindowStyle { get; set; }

        public string WorkingDirectory { get; set; }

        public CommandStartInfo(ProcessStartInfo processStartInfo)
        {
            this.CreateNoWindow = processStartInfo.CreateNoWindow;
            this.Domain = processStartInfo.Domain;
            this.LoadUserProfile = processStartInfo.LoadUserProfile;
            this.Password = processStartInfo.Password;
            this.UserName = processStartInfo.UserName;
            this.WindowStyle = processStartInfo.WindowStyle;
        }

        internal ProcessStartInfo GetProcessStartInfo()
        {
            return new ProcessStartInfo
            {
                CreateNoWindow = this.CreateNoWindow,
                Domain = this.Domain,
                LoadUserProfile = this.LoadUserProfile,
                Password = this.Password,
                UserName = this.UserName,
                WindowStyle = this.WindowStyle
            };
        }
    }
}