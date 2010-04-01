namespace Crabwise.CommandWrap
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Builds a syntactical representation of a <see cref="Command"/> object.
    /// </summary>
    internal sealed class SyntaxBuilder
    {
        /// <summary>
        /// The command that was provided in the constructor.
        /// </summary>
        private readonly Command command;

        /// <summary>
        /// The <see cref="StringBuilder"/> which is used to create the syntax string.
        /// </summary>
        private StringBuilder arguments;

        /// <summary>
        /// Initializes a new instance of the SyntaxBuilder class.
        /// </summary>
        /// <param name="command">Command from which to build the syntax.</param>
        public SyntaxBuilder(Command command)
        {
            this.command = command;
            this.BuildSyntax();
        }

        /// <summary>
        /// Gets the arguments portion of the command's syntax.
        /// </summary>
        public string Arguments { get; private set; }

        /// <summary>
        /// Gets the file name portion of the command's syntax. Specifically, this file name is used when launching 
        /// the process for a command.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the complete syntax for the command.
        /// </summary>
        /// <returns>The complete syntax for the command.</returns>
        /// <remarks>
        /// The string returned by this method is equivalent to what would be entered in a command prompt.
        /// </remarks>
        public override string ToString()
        {
            return (this.FileName + " " + this.Arguments).Trim();
        }

        /// <summary>
        /// Recursively builds the syntax for the command.
        /// </summary>
        /// <param name="commandType">The current type of command.</param>
        /// <param name="command">The command being built.</param>
        /// <remarks>
        /// This method recursively works its way up a <see cref="Command"/> object's inheritance hierarchy. It stops 
        /// when the next base class is not a subclass of <see cref="Command"/>. This allows for nested commands to be 
        /// built.
        /// </remarks>
        private void BuildCommandFromType(Type commandType, Command command)
        {
            var syntaxAttribute = this.GetSyntaxAttribute(commandType);
            if (syntaxAttribute == null)
            {
                return;
            }

            var commandSyntaxAttribute = syntaxAttribute as CommandSyntaxAttribute;
            if (commandSyntaxAttribute == null)
            {
                return;
            }

            var baseType = commandType.BaseType;
            if (baseType != null && baseType.IsSubclassOf(typeof(Command)))
            {
                this.BuildCommandFromType(baseType, command);
            }

            var fileName = commandSyntaxAttribute.GetFullPath();
            if (fileName.Contains(" "))
            {
                fileName = string.Format("\"{0}\"", fileName);
            }

            if (commandType.BaseType == typeof(Command))
            {
                this.FileName = fileName;
            }
            else
            {
                this.arguments.AppendFormat(fileName + ' ');
            }

            var properties = commandType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                BindingFlags.Public);
            var parameters = new List<Parameter>();
            foreach (var property in properties)
            {
                var parameterSyntaxAttribute = (ParameterSyntaxAttribute)this.GetSyntaxAttribute(property);
                if (parameterSyntaxAttribute == null)
                {
                    continue;
                }

                var returnType = property.GetGetMethod().ReturnType;
                if (returnType.IsSubclassOf(typeof(ValueType)) && Nullable.GetUnderlyingType(returnType) == null)
                {
                    const string MESSAGE = "The return type for a parameter isn't nullable. All properties adorned " +
                        "with a ParameterSyntaxAttribute must be nullable. For primitives, consider suffixing the " +
                        "type declaration with a question mark (?).";
                    throw new SyntaxException(MESSAGE, null, parameterSyntaxAttribute);
                }

                var argument = property.GetValue(command, null);
                if (argument == null && parameterSyntaxAttribute.Required)
                {
                    const string MESSAGE = "A parameter was marked as required but an argument was never given.";
                    throw new SyntaxException(MESSAGE, null, parameterSyntaxAttribute);
                }

                var parameterType = parameterSyntaxAttribute.ParameterType;
                try
                {
                    var parameterInstance = (Parameter)Activator.CreateInstance(
                        parameterType,
                        parameterSyntaxAttribute,
                        argument);
                    parameters.Add(parameterInstance);
                }
                catch (Exception e)
                {
                    const string MESSAGE = "Could not create an instance of the parameter type. If you're using a " +
                        "custom parameter type, make sure it doesn't implement any new constructors.";
                    throw new SyntaxException(MESSAGE, e, parameterSyntaxAttribute);
                }
            }

            parameters.Sort();
            foreach (var parameter in parameters)
            {
                var parameterString = parameter.ToString();
                if (string.IsNullOrEmpty(parameterString))
                {
                    continue;
                }

                this.arguments.Append(parameterString + ' ');
            }
        }

        /// <summary>
        /// Checks to make sure the provided <see cref="Command"/> has a <see cref="CommandSyntaxAttribute"/> and then 
        /// calls <see cref="SyntaxBuilder.BuildCommandFromType"/>
        /// </summary>
        private void BuildSyntax()
        {
            this.arguments = new StringBuilder(32, 32767);
            var commandType = this.command.GetType();
            var syntaxAttribute = this.GetSyntaxAttribute(commandType);
            if (syntaxAttribute == null || !(syntaxAttribute is CommandSyntaxAttribute))
            {
                const string MESSAGE = "The provided command doesn't have a CommandSyntaxAttribute.";
                throw new SyntaxException(MESSAGE, null);
            }

            try
            {
                this.BuildCommandFromType(commandType, this.command);
            }
            catch (ArgumentOutOfRangeException e)
            {
                const string MESSAGE = "The length of this command is too long. Process start strings are limited to " +
                    "32767 characters.";
                throw new SyntaxException(MESSAGE, e);
            }

            this.Arguments = this.arguments.ToString().Trim();
        }

        /// <summary>
        /// Gets a <see cref="SyntaxAttribute"/> from a member.
        /// </summary>
        /// <param name="memberInfo">The member from which to get the <see cref="SyntaxAttribute"/>.</param>
        /// <returns>If a <see cref="SyntaxAttribute"/> was found, then it's returned; otherwise, null.</returns>
        private SyntaxAttribute GetSyntaxAttribute(MemberInfo memberInfo)
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(SyntaxAttribute), true);
            if (attributes.Length == 0)
            {
                return null;
            }

            return (SyntaxAttribute)attributes[0];
        }
    }
}