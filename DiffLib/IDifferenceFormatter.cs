using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib
{
    /// <summary>
    /// Represents a way to format difference instructions.
    /// </summary>
    public interface IDifferenceFormatter<T>
    {
        /// <summary>
        /// Formats the specified difference instructions and
        /// writes them to the specified writer.
        /// </summary>
        /// <param name="left">The original left-hand content.</param>
        /// <param name="right">The original right-hand content.</param>
        /// <param name="instructions">The instructions to format.</param>
        /// <param name="target">The target writer.</param>
        void Format(IList<T> left, IList<T> right, IEnumerable<DifferenceInstruction> instructions, TextWriter target);
    }

    /// <summary>
    /// Represents mixins for <see cref="IDifferenceFormatter&lt;T&gt;"/>.
    /// </summary>
    public static class IDifferenceFormatterMixins
    {
        /// <summary>
        /// Formats the specified difference instructions and
        /// writes them to the specified writer.
        /// </summary>
        /// <param name="left">The original left-hand content.</param>
        /// <param name="right">The original right-hand content.</param>
        /// <param name="instructions">The instructions to format.</param>
        /// <param name="target">The target writer.</param>
        public static string Format<T>(this IDifferenceFormatter<T> formatter, IList<T> left, IList<T> right, IEnumerable<DifferenceInstruction> instructions)
        {
            if (formatter == null) throw new ArgumentNullException("formatter");
            using (var stringwriter = new StringWriter())
            {
                formatter.Format(left, right, instructions, stringwriter);
                return stringwriter.ToString();
            }
        }
    }
}
