namespace Crabwise.CommandWrap.Library
{
    using System;
    using System.Collections;
    using System.Text;

    public class Parameter : IComparable<Parameter>
    {
        private readonly object argument;
        private readonly int priority;
        private readonly bool required;
        private readonly string syntax;

        public Parameter(ParameterSyntaxAttribute syntaxAttribute, object argument)
        {
            this.priority = syntaxAttribute.Priority;
            this.required = syntaxAttribute.Required;
            this.syntax = syntaxAttribute.Syntax;
            this.argument = argument;
        }

        public object Argument
        {
            get
            {
                return this.argument;
            }
        }

        public int Priority
        {
            get
            {
                return this.priority;
            }
        }

        public bool Required
        {
            get
            {
                return this.required;
            }
        }

        public string Syntax
        {
            get
            {
                return this.syntax;
            }
        }

        public int CompareTo(Parameter other)
        {
            return other.priority.CompareTo(this.priority);
        }

        public override string ToString()
        {
            if (this.argument == null)
            {
                return string.Empty;
            }

            // Default bool behavior.
            bool? argumentAsBool = this.argument as bool?;
            if (argumentAsBool != null && !this.syntax.Contains("{arg}"))
            {
                return (bool)argumentAsBool ? this.syntax : string.Empty;
            }

            // Default IEnumberable behavior.
            IEnumerable argumentAsIEnumberable = this.argument as IEnumerable;
            StringBuilder argumentBuilder = new StringBuilder();
            if (argumentAsIEnumberable != null && !(argument is string))
            {
                foreach (var item in argumentAsIEnumberable)
                {
                    var itemString = item.ToString();
                    if (string.IsNullOrEmpty(itemString))
                    {
                        continue;
                    }

                    itemString = this.EscapeSpaces(itemString);
                    argumentBuilder.AppendFormat(itemString);
                }

                var argumentString = argumentBuilder.ToString().Trim();
                return this.syntax.Replace("{arg}", argumentString).Trim();
            }

            // Default Enum behavior
            Enum argumentAsEnum = this.argument as Enum;
            if (argumentAsEnum != null)
            {
                var enumName = Enum.GetName(this.argument.GetType(), this.argument);
                return this.syntax.Replace("{arg}", enumName).Trim();
            }

            string defaultArgumentString = this.EscapeSpaces(this.argument.ToString());
            return this.syntax.Replace("{arg}", defaultArgumentString).Trim();
        }

        private string EscapeSpaces(string argument)
        {
            if (argument.Contains(" "))
            {
                argument = string.Format("\"{0}\"", argument);
            }

            return argument;
        }
    }
}