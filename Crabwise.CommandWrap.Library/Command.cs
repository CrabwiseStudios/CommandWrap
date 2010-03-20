namespace Crabwise.CommandWrap.Library
{
    /// <summary>
    /// Provides an abstract representation of a command prompt command.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Determines whether two <see cref="Command"/> objects have the same <see cref="String"/>.
        /// </summary>
        /// <param name="obj"><see cref="Object"/> to compare.</param>
        /// <returns><b>true</b> if obj is a <see cref="Command"/> and its <see cref="String"/> representation is the 
        /// same as this instance; otherwise, <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            var command = obj as Command;
            if (command == null)
            {
                return false;
            }

            return command.ToString() == this.ToString();
        }

        /// <summary>
        /// Gets the hash code for the Command.
        /// </summary>
        /// <returns>An <see cref="Int32"/> containing the hash value generated for this command.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets the command prompt <see cref="String"/> representation for this <see cref="Command"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that contains the command prompt representation of this 
        /// <see cref="Command"/>.</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}