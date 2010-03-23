namespace Crabwise.CommandWrap.Library
{
    using System;

    /// <summary>
    /// Describes the syntax of a command parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ParameterSyntaxAttribute : SyntaxAttribute, IComparable<ParameterSyntaxAttribute>
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
        /// Gets or sets the ordering priority of this parameter. Parameters with higher priorities are placed 
        /// first in the command prompt string.
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this parameter is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets a custom parameter type to be used instead of the default one.
        /// </summary>
        public Type ParameterType { get; set; }

        /// <summary>
        /// Compares this <see cref="ParameterSyntaxAttribute"/> with another 
        /// <see cref="ParameterSyntaxAttribute"/>.
        /// </summary>
        /// <param name="other">The <see cref="ParameterSyntaxAttribute"/> to compare.</param>
        /// <returns>A value less than zero if this instance is less than <paramref name="other"/>, zero if this 
        /// instance is equal to <paramref name="other"/>, or a value greater than zero if this instance is 
        /// greater than <paramref name="other"/>.</returns>
        public int CompareTo(ParameterSyntaxAttribute other)
        {
            return other.Priority.CompareTo(this.Priority);
        }
    }
}