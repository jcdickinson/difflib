using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib
{
    /// <summary>
    /// Represents a way to find longest common subsequences.
    /// </summary>
    /// <typeparam name="T">The type of item within the sequences.</typeparam>
    public interface ISequenceMatcher<T>
    {
        /// <summary>
        /// Finds blocks within two sequences that match.
        /// </summary>
        /// <param name="left">The left-hand sequence.</param>
        /// <param name="right">The right-hand sequence.</param>
        /// <returns>
        /// A list of sequences that represents the blocks that are equal in both
        /// the left-hand and right-hand sequences, ordered by their appearance in both.
        /// </returns>
        IEnumerable<SubSequence> FindMatchingBlocks(IList<T> left, IList<T> right);
    }
}
