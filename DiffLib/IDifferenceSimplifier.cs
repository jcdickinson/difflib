using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib
{
    /// <summary>
    /// Represents a way to simplify a diff while maintaining its effect.
    /// </summary>
    /// <param name="instructions">The instructions to simplify.</param>
    /// <returns>The list of simplified instructions.</returns>
    public delegate IEnumerable<DifferenceInstruction> DifferenceSimplifier(IEnumerable<DifferenceInstruction> instructions);

    /// <summary>
    /// Represents a repository of <see cref="DifferenceSimplifier"/> implementations.
    /// </summary>
    public static class DifferenceSimplifiers
    {
        /// <summary>
        /// Gets the merge instructions.
        /// </summary>
        /// <value>
        /// The merge instructions.
        /// </value>
        public static DifferenceSimplifier MergeInstructions { get; } = MergeInstructionsImpl;

        /// <summary>
        /// Combines a list of <see cref="DifferenceSimplifier"/> delegates into a single one.
        /// </summary>
        /// <param name="simplifiers">The simplifiers.</param>
        /// <returns>The new single <see cref="DifferenceSimplifier"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="simplifiers"/> is <c>null</c>.</exception>
        public static DifferenceSimplifier Combine(params DifferenceSimplifier[] simplifiers)
        {
            return Combine((IEnumerable<DifferenceSimplifier>)simplifiers);
        }

        /// <summary>
        /// Combines a list of <see cref="DifferenceSimplifier"/> delegates into a single one.
        /// </summary>
        /// <param name="simplifiers">The simplifiers.</param>
        /// <returns>The new single <see cref="DifferenceSimplifier"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="simplifiers"/> is <c>null</c>.</exception>
        public static DifferenceSimplifier Combine(IEnumerable<DifferenceSimplifier> simplifiers)
        {
            if (simplifiers == null) throw new ArgumentNullException("simplifiers");

            DifferenceSimplifier current = null;
            foreach (var item in simplifiers)
            {
                if (current == null)
                    current = item;
                else
                {
                    var previous = current;
                    current = x => current(previous(x));
                }
            }

            if (current == null)
                current = x => x;

            return current;
        }

        private static IEnumerable<DifferenceInstruction> MergeInstructionsImpl(IEnumerable<DifferenceInstruction> instructions)
        {
            DifferenceInstruction? previous = null;
            foreach (var instruction in instructions)
            {
                if (previous.HasValue)
                {
                    if ((previous.Value.Operation == DifferenceOperation.Removed &&
                        instruction.Operation == DifferenceOperation.Inserted) ||
                        (previous.Value.Operation == DifferenceOperation.Inserted &&
                        instruction.Operation == DifferenceOperation.Removed))
                    {
                        if (instruction.Operation == DifferenceOperation.Inserted &&
                            previous.Value.SubSequence.LeftLength == instruction.SubSequence.RightLength)
                        {
                            yield return new DifferenceInstruction(DifferenceOperation.Replaced,
                                new SubSequence(
                                    previous.Value.SubSequence.LeftIndex, instruction.SubSequence.RightIndex,
                                    previous.Value.SubSequence.LeftLength, instruction.SubSequence.RightLength));
                            previous = null;
                        }
                        else if (instruction.Operation == DifferenceOperation.Removed &&
                            instruction.SubSequence.LeftLength == previous.Value.SubSequence.RightLength)
                        {
                            yield return new DifferenceInstruction(DifferenceOperation.Replaced,
                                new SubSequence(
                                    instruction.SubSequence.LeftIndex, previous.Value.SubSequence.RightIndex,
                                    instruction.SubSequence.LeftLength, previous.Value.SubSequence.RightLength));
                            previous = null;
                        }
                        else
                        {
                            yield return previous.Value;
                            previous = instruction;
                        }
                    }
                    else
                    {
                        yield return previous.Value;
                        previous = instruction;
                    }
                }
                else previous = instruction;
            }

            if (previous.HasValue)
                yield return previous.Value;
        }
    }
}
