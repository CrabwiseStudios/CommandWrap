namespace Crabwise.CommandWrap.Library
{
    using System;

    /// <summary>
    /// Describes the syntax of a command parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ParameterSyntaxAttribute : SyntaxAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterSyntaxAttribute"/> class.
        /// </summary>
        /// <param name="syntax">The syntax of the parameter.</param>
        public ParameterSyntaxAttribute(string syntax)
            : base(syntax)
        {
            this.ParameterType = typeof(Parameter);
        }

        /// <summary>
        /// Gets or sets the ordering position of this parameter. Parameters with a higher positioning are placed 
        /// toward the end of the command prompt string.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this parameter is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a custom parameter type to be used instead of the default one.
        /// </summary>
        public Type ParameterType { get; set; }
    }
}