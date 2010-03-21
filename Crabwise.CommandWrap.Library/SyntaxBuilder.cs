namespace Crabwise.CommandWrap.Library
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    internal sealed class SyntaxBuilder
    {
        private readonly StringBuilder syntax = new StringBuilder(32, 32767);

        public void BuildCommand(Command command)
        {
            var commandType = command.GetType();
            this.BuildCommandFromType(commandType, command);
        }

        public override string ToString()
        {
            return this.syntax.ToString().Trim();
        }

        private void BuildCommandFromType(Type commandType, Command command)
        {
            var baseType = commandType.BaseType;
            if (baseType != null)
            {
                this.BuildCommandFromType(baseType, command);
            }

            var syntaxAttribute = this.GetSyntaxAttribute(commandType);
            if (syntaxAttribute == null)
            {
                return;
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
                    // Type isn't nullable.
                    throw new System.Exception();
                }

                var argument = property.GetValue(command, null);
                if (argument == null && parameterSyntaxAttribute.Required)
                {
                    // Argument required and wasn't provided.
                    throw new System.Exception();
                }

                parameters.Add(new Parameter(parameterSyntaxAttribute, argument));
            }

            parameters.Sort();
            foreach (var parameter in parameters)
            {
                var parameterString = parameter.ToString();
                if (string.IsNullOrEmpty(parameterString))
                {
                    continue;
                }

                this.syntax.Append(parameter.ToString() + ' ');
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