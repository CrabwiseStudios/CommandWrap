namespace Crabwise.CommandWrap.Library
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    internal sealed class SyntaxBuilder
    {
        private readonly StringBuilder syntax = new StringBuilder(32, 32767);

        public void AppendCommand(Command command)
        {
            var commandType = command.GetType();
            var syntaxAttribute = this.GetSyntaxAttribute(commandType);
            if (syntaxAttribute == null || !(syntaxAttribute is CommandSyntaxAttribute))
            {
                const string MESSAGE = "The provided command doesn't have a CommandSyntaxAttribute.";
                throw new SyntaxException(MESSAGE, null);
            }

            this.BuildCommandFromType(commandType, command);
        }

        public override string ToString()
        {
            return this.syntax.ToString().Trim();
        }

        private void BuildCommandFromType(Type commandType, Command command)
        {
            var syntaxAttribute = this.GetSyntaxAttribute(commandType);
            if (syntaxAttribute == null || !(syntaxAttribute is CommandSyntaxAttribute))
            {
                return;
            }

            var baseType = commandType.BaseType;
            if (baseType != null && baseType.IsSubclassOf(typeof(Command)))
            {
                this.BuildCommandFromType(baseType, command);
            }

            this.syntax.Append(syntaxAttribute.Syntax + ' ');

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

                this.syntax.Append(parameterString + ' ');
            }
        }

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