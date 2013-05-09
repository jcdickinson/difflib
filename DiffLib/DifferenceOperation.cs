using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib
{
    /// <summary>
    /// Represents a difference operation.
    /// </summary>
    public enum DifferenceOperation
    {
        /// <summary>
        /// The sequences are equal.
        /// </summary>
        Equal = 0,
        /// <summary>
        /// The sequence on the right-hand side has been added.
        /// </summary>
        Inserted = 1,
        /// <summary>
        /// The sequence on the left-hand side has been replaced by the sequence on the right-hand side.
        /// </summary>
        Replaced = 2,
        /// <summary>
        /// The sequence on the left-hand side has been removed.
        /// </summary>
        Removed = 3
    }
}
