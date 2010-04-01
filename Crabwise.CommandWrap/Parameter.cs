namespace Crabwise.CommandWrap
{
    using System;
    using System.Collections;
    using System.Text;

    /// <summary>
    /// Represents a parameter to a command.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class determines how a parameter is written to the command prompt. It is used by default when a parameter 
    /// type is not specified in the <see cref="ParameterSyntaxAttribute.ParameterType"/> attribute. For most cases, 
    /// this class provides the desired behavior, however more control can be achieved by deriving from this class and 
    /// overriding the <see cref="Parameter.ToString()"/> method. You can then specify your custom parameter type in 
    /// the <see cref="ParameterSyntaxAttribute"/>. When deriving from this class, do not implement custom 
    /// constructors. Doing so will throw an exception and prevent your commands from working.
    /// </para>
    /// <para>
    /// This class formats parameters using a provided <see cref="SyntaxAttribute"/> object and an argument. By 
    /// default, the <see cref="SyntaxAttribute.Syntax"/> string is searched for the substring "{arg}". Whenever this 
    /// substring is found, it is replaced with the string representation of the argument. Additionally, if any spaces 
    /// are found in the argument, then the argument gets encapsulated in quotes ("). If the argument is null, then  
    /// the parameter is omitted entirely from the final syntax. Because of this, all parameter types must be 
    /// nullable. If a non-nullable type is found, then an exception is thrown.
    /// </para>
    /// </remarks>
    public class Parameter : IComparable<Parameter>
    {
        /// <summary>
        /// Argument of this parameter. Provided in the constructor.
        /// </summary>
        protected readonly object Argument;

        /// <summary>
        /// Position of this parameter. Provided by syntaxAttribute in the constructor.
        /// </summary>
        protected readonly int Position;

        /// <summary>
        /// Whether or not this parameter is required. Provided by syntaxAttribute in the constructor.
        /// </summary>
        protected readonly bool Required;

        /// <summary>
        /// Syntax of this parameter. Provided by syntaxAttribute in the constructor.
        /// </summary>
        protected readonly string Syntax;

        /// <summary>
        /// Initializes a new instance of the Parameter class.
        /// </summary>
        /// <param name="syntaxAttribute"><see cref="ParameterSyntaxAttribute"/> providing information about the 
        /// parameter.</param>
        /// <param name="argument">The argument provided to this parameter.</param>
        public Parameter(ParameterSyntaxAttribute syntaxAttribute, object argument)
        {
            this.Position = syntaxAttribute.Position;
            this.Required = syntaxAttribute.Required;
            this.Syntax = syntaxAttribute.Syntax;
            this.Argument = argument;
        }

        /// <summary>
        /// Compares this <see cref="Parameter"/> with another <see cref="Parameter"/>.
        /// </summary>
        /// <param name="other">The <see cref="Parameter"/> to compare with this <see cref="Parameter"/>.</param>
        /// <returns>A value less than zero if this instance is less than <paramref name="other"/>, zero if this 
        /// instance is equal to <paramref name="other"/>, or a value greater than zero if this instance is 
        /// greater than <paramref name="other"/>.</returns>
        /// <remarks>
        /// <see cref="Parameter"/> objects are compared using their position values.
        /// </remarks>
        public int CompareTo(Parameter other)
        {
            return this.Position.CompareTo(other.Position);
        }

        /// <summary>
        /// Returns this parameter formatted as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>This parameter formatted as a string.</returns>
        /// <remarks>
        /// <para>
        /// All argument types except for <see cref="System.Boolean"/>, <see cref="System.Collections.IEnumerable"/> and 
        /// <see cref="System.Enum"/> are represented using their <see cref="Object.ToString()"/> method. The 
        /// aforementioned types are handled differently to provide the most commonly desired functionality.
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Type</term>
        /// <description>String representation</description>
        /// </listheader>
        /// <item>
        /// <term><see cref="System.Boolean"/></term>
        /// <description>
        /// If the "{arg}" substring is found in <see cref="SyntaxAttribute.Syntax"/>, then "{arg}" is replaced with the 
        /// value of the boolean ("true" or "false"). If "{arg}" is not found, then the boolean determines whether or not 
        /// the parameter is omitted from the final syntax. If the value is true, then the parameter is included in the 
        /// syntax; if it's false then the parameter is omitted (equivalent to a null argument).
        /// </description>
        /// </item>
        /// <item>
        /// <term><see cref="System.Collections.IEnumerable"/></term>
        /// <description>
        /// The "{arg}" substring is replaced with a space-delimited list where each item in the 
        /// <see cref="System.Collections.IEnumerable"/> is printed using its <see cref="Object.ToString()"/> method. If a 
        /// space is found in an item, it gets encapsulated in quotes ("). Note: This behavior does not apply to 
        /// <see cref="System.String"/> even though it implements <see cref="System.Collections.IEnumerable"/>.
        /// </description>
        /// </item>
        /// <item>
        /// <term><see cref="System.Enum"/></term>
        /// <description>
        /// The name of the selected value in the <see cref="System.Enum"/> is printed.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public override string ToString()
        {
            if (this.Argument == null)
            {
                return string.Empty;
            }

            // Default bool behavior.
            bool? argumentAsBool = this.Argument as bool?;
            if (argumentAsBool != null && !this.Syntax.Contains("{arg}"))
            {
                return (bool)argumentAsBool ? this.Syntax : string.Empty;
            }

            // Default IEnumberable behavior.
            IEnumerable argumentAsIEnumberable = this.Argument as IEnumerable;
            StringBuilder argumentBuilder = new StringBuilder();
            if (argumentAsIEnumberable != null && !(this.Argument is string))
            {
                foreach (var item in argumentAsIEnumberable)
                {
                    var itemString = item.ToString();
                    if (string.IsNullOrEmpty(itemString))
                    {
                        continue;
                    }

                    itemString = this.EscapeSpaces(itemString);
                    argumentBuilder.Append(itemString + ' ');
                }

                var argumentString = argumentBuilder.ToString().Trim();
                return this.Syntax.Replace("{arg}", argumentString).Trim();
            }

            // Default Enum behavior
            Enum argumentAsEnum = this.Argument as Enum;
            if (argumentAsEnum != null)
            {
                var enumName = Enum.GetName(this.Argument.GetType(), this.Argument);
                return this.Syntax.Replace("{arg}", enumName).Trim();
            }

            string defaultArgumentString = this.EscapeSpaces(this.Argument.ToString());
            return this.Syntax.Replace("{arg}", defaultArgumentString).Trim();
        }

        /// <summary>
        /// Takes in an argument and encapsulates it in quotes if necessary.
        /// </summary>
        /// <param name="argument">The argument to escape.</param>
        /// <returns>If the provided argument contained spaces, then it is returned in quotes; otherwise, the argument 
        /// is just returned.</returns>
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