using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib
{
    /// <summary>
    /// Represents a standard diff formatter.
    /// </summary>
    public sealed class StandardDiffFormatter<T> : IDifferenceFormatter<T>
    {
        private static readonly Func<T, string> DefaultToString = x =>
        {
            if (x == null) return "";
            else return x.ToString();
        };

        private readonly Func<T, string> _toString;
        private readonly int _contextLines = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardDiffFormatter{T}"/> class.
        /// </summary>
        public StandardDiffFormatter(int contextLines = 0)
            : this(DefaultToString, contextLines)
        {
            _contextLines = contextLines;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardDiffFormatter{T}"/> class.
        /// </summary>
        /// <param name="toString">A function that can be used to convert values of <typeparamref name="T"/> to <see cref="string"/> values.</param>
        public StandardDiffFormatter(Func<T, string> toString, int contextLines = 0)
        {
            _toString = toString ?? DefaultToString;
            _contextLines = contextLines;
        }

        /// <summary>
        /// Formats the specified difference instructions and
        /// writes them to the specified writer.
        /// </summary>
        /// <param name="left">The original left-hand content.</param>
        /// <param name="right">The original right-hand content.</param>
        /// <param name="instructions">The instructions to format.</param>
        /// <param name="target">The target writer.</param>
        public void Format(IList<T> left, IList<T> right, IEnumerable<DifferenceInstruction> instructions, System.IO.TextWriter target)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");
            if (instructions == null) throw new ArgumentNullException("instructions");
            if (target == null) throw new ArgumentNullException("target");

            var hasEqualOperation = false;
            var equal = new DifferenceInstruction();
            foreach (var instruction in instructions)
            {
                if (instruction.Operation != DifferenceOperation.Equal)
                {
                    var ct = 0;
                    if (hasEqualOperation)
                    {
                        ct = Math.Min(_contextLines, equal.SubSequence.LeftLength);
                    }

                    target.WriteLine("@@ -{0},{1} +{2},{3} @@",
                        instruction.SubSequence.LeftIndex + 1 - ct, instruction.SubSequence.LeftEndIndex + 2 - ct,
                        instruction.SubSequence.RightIndex + 1 - ct, instruction.SubSequence.RightEndIndex + 2 - ct);

                    if (hasEqualOperation)
                    {
                        for (var i = 0; i < ct; i++)
                        {
                            target.WriteLine(" {0}", _toString(left[equal.SubSequence.LeftIndex + i]));
                        }
                    }

                    for (var i = 0; i < instruction.SubSequence.LeftLength; i++)
                        target.WriteLine("-{0}", _toString(left[instruction.SubSequence.LeftIndex + i]));
                    for (var i = 0; i < instruction.SubSequence.RightLength; i++)
                        target.WriteLine("+{0}", _toString(right[instruction.SubSequence.RightIndex + i]));

                    hasEqualOperation = false;
                }
                else
                {
                    equal = instruction;
                    hasEqualOperation = true;
                }
            }
        }
    }
}
