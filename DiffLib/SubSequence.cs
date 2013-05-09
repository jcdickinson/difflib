using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib
{
    /// <summary>
    /// Represents a sub-sequence.
    /// </summary>
    public struct SubSequence : IEquatable<SubSequence>, IFormattable
    {
        /// <summary>
        /// Gets the starting index in the left-hand sequence.
        /// </summary>
        public readonly int LeftIndex;

        /// <summary>
        /// Gets the starting index in the right-hand sequence.
        /// </summary>
        public readonly int RightIndex;

        /// <summary>
        /// Gets the number of matched items in the left-hand sequence.
        /// </summary>
        public readonly int LeftLength;

        /// <summary>
        /// Gets the number of matched items in the right-hand sequence.
        /// </summary>
        public readonly int RightLength;

        /// <summary>
        /// Gets the ending index in the left-hand sequence.
        /// </summary>
        /// <value>
        /// The ending index in the left-hand sequence.
        /// </value>
        public int LeftEndIndex
        {
            get
            {
                return LeftIndex + LeftLength - 1;
            }
        }

        /// <summary>
        /// Gets the ending index in the right-hand sequence.
        /// </summary>
        /// <value>
        /// The ending index in the right-hand sequence.
        /// </value>
        public int RightEndIndex
        {
            get
            {
                return RightIndex + RightLength - 1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubSequence" /> struct.
        /// </summary>
        /// <param name="leftIndex">The starting index in the left-hand sequence.</param>
        /// <param name="rightIndex">The starting index in the right-hand sequence.</param>
        /// <param name="length">The number of matched items in both sequences.</param>
        public SubSequence(int leftIndex, int rightIndex, int commonLength)
        {
            if (leftIndex < 0) throw new ArgumentOutOfRangeException("leftIndex");
            if (rightIndex < 0) throw new ArgumentOutOfRangeException("rightIndex");
            if (commonLength < 0) throw new ArgumentOutOfRangeException("commonLength");

            LeftIndex = leftIndex;
            RightIndex = rightIndex;
            LeftLength = RightLength = commonLength;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubSequence" /> struct.
        /// </summary>
        /// <param name="leftIndex">The starting index in the left-hand sequence.</param>
        /// <param name="rightIndex">The starting index in the right-hand sequence.</param>
        /// <param name="leftLength">The number of matched items in the left-hand sequence.</param>
        /// <param name="rightLength">The number of matched items in the right-hand sequence.</param>
        public SubSequence(int leftIndex, int rightIndex, int leftLength, int rightLength)
        {
            if (leftIndex < 0) throw new ArgumentOutOfRangeException("leftIndex");
            if (rightIndex < 0) throw new ArgumentOutOfRangeException("rightIndex");
            if (leftLength < 0) throw new ArgumentOutOfRangeException("leftLength");
            if (rightLength < 0) throw new ArgumentOutOfRangeException("rightLength");

            LeftIndex = leftIndex;
            RightIndex = rightIndex;
            LeftLength = leftLength;
            RightLength = rightLength;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return (LeftIndex << 5) ^ (RightIndex << 3) ^ (LeftLength << 1) ^ (RightLength);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is SubSequence))
                return false;
            return Equals((SubSequence)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(SubSequence other)
        {
            return 
                other.LeftLength == LeftLength && other.RightLength == RightLength && 
                other.LeftIndex == LeftIndex && other.RightIndex == RightIndex;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString("S", null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) format = "S";
            switch (format)
            {
                case "S": // Sequence
                    if (LeftIndex < 0 && RightIndex < 0)
                    {
                        return "(<>)";
                    }
                    else if (LeftIndex < 0)
                    {
                        if (RightLength < 1)
                            return string.Format(formatProvider, "(> {0})", RightIndex);
                        else
                            return string.Format(formatProvider, "(> {0} -> {1})", RightIndex, RightIndex + RightLength - 1);
                    }
                    else if (RightIndex < 0)
                    {
                        if (LeftLength < 1)
                            return string.Format(formatProvider, "(< {0})", LeftIndex);
                        else
                            return string.Format(formatProvider, "(< {0} -> {1})", LeftIndex, LeftIndex + LeftLength - 1);
                    }
                    else if (LeftLength < 1 && RightLength < 1)
                    {
                        return string.Format(formatProvider, "({0}, {1})", LeftIndex, RightIndex);
                    }
                    else if (LeftLength < 1)
                    {
                        return string.Format(formatProvider, "({0}, {1} -> {2})", LeftIndex, RightIndex, RightIndex + RightLength - 1);
                    }
                    else if (RightLength < 1)
                    {
                        return string.Format(formatProvider, "({0} -> {1}, {2})", LeftIndex, LeftIndex + LeftLength - 1, RightIndex);
                    }
                    else
                    {
                        return string.Format(formatProvider, "({0} -> {1}, {2} -> {3})", LeftIndex, LeftIndex + LeftLength - 1, RightIndex, RightIndex + RightLength - 1);
                    }
                case "T": // Tuple
                    return string.Format(formatProvider, "{0}, {1}, {2}, {3}", LeftIndex, RightIndex, LeftLength, RightLength);
                default:
                    throw new ArgumentOutOfRangeException("format");
            }
        }
    }
}
