using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib
{
    /// <summary>
    /// Represents a way to find the difference between two
    /// sequences using a specific longest common subsequence
    /// algorithm.
    /// </summary>
    public sealed class Differencer<T>
    {
        private readonly ISequenceMatcher<T> _sequenceMatcher;
        private DifferenceSimplifier _simplifier = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Differencer{T}" /> class.
        /// </summary>
        /// <param name="sequenceMatcher">The sequence matcher to use to find longest common subsequences.</param>
        public Differencer(ISequenceMatcher<T> sequenceMatcher)
        {
            if (sequenceMatcher == null) throw new ArgumentNullException("sequenceMatcher");
            _sequenceMatcher = sequenceMatcher;
        }

        /// <summary>
        /// Adds a simplifier to the differences.
        /// </summary>
        /// <param name="simplifier">The simplifier to add.</param>
        /// <returns><c>this</c> instance.</returns>
        public Differencer<T> AddSimplifier(DifferenceSimplifier simplifier)
        {
            if (simplifier == null) throw new ArgumentNullException("simplifier");
            if (_simplifier == null) _simplifier = simplifier;
            else _simplifier = DifferenceSimplifiers.Combine(_simplifier, simplifier);
            return this;
        }

        /// <summary>
        /// Calculates the difference between two sequences.
        /// </summary>
        /// <param name="left">The left-hand sequence.</param>
        /// <param name="right">The right-hand sequence.</param>
        /// <returns>
        /// The operations that should be applied to <see cref="left" /> in order to
        /// acquire <see cref="right" />.
        /// </returns>
        public IEnumerable<DifferenceInstruction> FindDifferences(IList<T> left, IList<T> right)
        {
            if (left == null) throw new ArgumentNullException("left");
            if (right == null) throw new ArgumentNullException("right");

            var matches = _sequenceMatcher.FindMatchingBlocks(left, right);

            var lastLeftIndex = -1;
            var lastRightIndex = -1;

            foreach (var item in matches)
            {
                if (lastLeftIndex != -1)
                {
                    var diff = item.LeftIndex - lastLeftIndex;
                    if (diff > 0)
                        yield return new DifferenceInstruction(DifferenceOperation.Removed,
                            new SubSequence(lastLeftIndex, 0, diff, 0));
                }
                else if (item.LeftIndex > 0 && item.RightIndex == 0)
                {
                    yield return new DifferenceInstruction(DifferenceOperation.Removed,
                        new SubSequence(0, 0, item.LeftLength, 0));
                }

                if (lastRightIndex != -1)
                {
                    var diff = item.RightIndex - lastRightIndex;
                    if (diff > 0)
                        yield return new DifferenceInstruction(DifferenceOperation.Inserted,
                            new SubSequence(0, lastRightIndex, 0, diff));
                }
                else if (item.RightIndex > 0 && item.LeftIndex == 0)
                {
                    yield return new DifferenceInstruction(DifferenceOperation.Inserted,
                        new SubSequence(0, 0, 0, item.RightIndex));
                }

                if (item.LeftLength > 0 && item.LeftLength == item.RightLength)
                {
                    yield return new DifferenceInstruction(DifferenceOperation.Equal, item);
                }

                lastLeftIndex = item.LeftEndIndex + 1;
                lastRightIndex = item.RightEndIndex + 1;
            }

            if (lastLeftIndex < left.Count)
            {
                if (lastLeftIndex < 0) lastLeftIndex = 0;
                yield return new DifferenceInstruction(DifferenceOperation.Removed,
                            new SubSequence(lastLeftIndex, 0, left.Count - lastLeftIndex, 0));
            }

            if (lastRightIndex < right.Count)
            {
                if (lastRightIndex < 0) lastRightIndex = 0;
                if (lastLeftIndex < 0) lastLeftIndex = 0;
                yield return new DifferenceInstruction(DifferenceOperation.Inserted,
                            new SubSequence(lastLeftIndex, lastRightIndex, 0, right.Count - lastRightIndex));
            }
        }

    }
}
