using System;
using System.Collections.Generic;

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

    /// <summary>
    /// Provides metadata information about a <see cref="ISequenceMatcher&lt;T&gt;"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SequenceMatcherAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the sequence matcher.
        /// </summary>
        /// <value>
        /// The name of the sequence matcher.
        /// </value>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceMatcherAttribute" /> class.
        /// </summary>
        /// <param name="name">The name of the sequence matcher.</param>
        public SequenceMatcherAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            Name = name;
        }
    }

    /// <summary>
    /// Represents mixins for <see cref="ISequenceMatcher"/>.
    /// </summary>
    public static class ISequenceMatcherMixins
    {
        /// <summary>
        /// Creates a new <see cref="Differencer&lt;T&gt;"/> that utilizes the specified
        /// <see cref="ISequenceMatcher&lt;T&gt;"/>.
        /// </summary>
        /// <typeparam name="T">The type of item within the sequences.</typeparam>
        /// <param name="sequenceMatcher">The sequence matcher.</param>
        /// <returns>The <see cref="Differencer&lt;T&gt;"/>.</returns>
        public static Differencer<T> CreateDifferencer<T>(this ISequenceMatcher<T> sequenceMatcher)
        {
            return new Differencer<T>(sequenceMatcher);
        }
    }
}
