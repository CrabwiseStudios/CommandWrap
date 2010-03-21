namespace Crabwise.CommandWrap.Library
{
    using System;

    internal sealed class Parameter : IComparable<Parameter>
    {
        private readonly ParameterSyntaxAttribute syntaxAttribute;
        private readonly object argument;

        public Parameter(ParameterSyntaxAttribute syntaxAttribute, object argument)
        {
            this.syntaxAttribute = syntaxAttribute;
            this.argument = argument;
        }

        public object Argument
        {
            get
            {
                return this.argument;
            }
        }

        public ParameterSyntaxAttribute Attribute
        {
            get
            {
                return this.syntaxAttribute;
            }
        }

        public int CompareTo(Parameter other)
        {
            return this.syntaxAttribute.CompareTo(other.Attribute);
        }

        public override string ToString()
        {
            if (this.argument == null)
            {
                return string.Empty;
            }

            return this.syntaxAttribute.Syntax.Replace("{arg}", this.argument.ToString());
        }
    }
}