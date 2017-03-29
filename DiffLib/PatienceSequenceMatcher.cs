using System;
using System.Collections.Generic;
using System.Linq;

namespace DiffLib
{
    /// <summary>
    /// Represents a way to find longest common subsequences, using the patience LCS
    /// algorithm.
    /// </summary>
    /// <typeparam name="T">The type of item within the sequences.</typeparam>
    [SequenceMatcher("Patience")]
    public sealed class PatienceSequenceMatcher<T> : ISequenceMatcher<T>
    {
        // http://bazaar.launchpad.net/~bzr-pqm/bzr/bzr.dev/view/head:/bzrlib/_patiencediff_py.py

        #region Structs
        private struct NullablePair
        {
            public readonly int? A;
            public readonly int? B;

            public NullablePair(int? a, int? b)
            {
                A = a;
                B = b;
            }

            public NullablePair SetA(int? a) => new NullablePair(a, B);
            public NullablePair SetB(int? b) => new NullablePair(A, b);
        }

        private struct Pair
        {
            public readonly int A;
            public readonly int B;

            public Pair(int a, int b)
            {
                A = a;
                B = b;
            }

            public Pair SetA(int a) => new Pair(a, B);
            public Pair SetB(int b) => new Pair(A, b);
        } 
        #endregion

        private readonly IEqualityComparer<T> _equality;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatienceSequenceMatcher{T}"/> class.
        /// </summary>
        public PatienceSequenceMatcher()
            : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatienceSequenceMatcher{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public PatienceSequenceMatcher(IEqualityComparer<T> comparer)
        {
            _equality = comparer ?? EqualityComparer<T>.Default;
        }

        /// <summary>
        /// Finds unique and common lines between <paramref name="a"/> and
        /// <paramref name="b"/>. They are then sorted according to their appearance
        /// in <paramref name="a"/>, while keeping track of the index in <paramref name="b"/>. Finally
        /// that list is sorted according to a patience sort and the longest common subsequence is
        /// extracted.
        /// </summary>
        /// <param name="a">The first list of values.</param>
        /// <param name="b">The second list of values.</param>
        /// <param name="aLow">The starting index in <paramref name="a"/> (inclusive).</param>
        /// <param name="bLow">The starting index in <paramref name="b"/> (inclusive).</param>
        /// <param name="aHigh">The ending index in <paramref name="a"/> (exclusive).</param>
        /// <param name="bHigh">The ending index in <paramref name="b"/> (exclusive).</param>
        /// <returns>
        /// A list of tuples that represent the longest common subsequence:
        /// (Index of item in <paramref name="a"/>, Index of item in <paramref name="b"/>).
        /// </returns>
        private IEnumerable<Pair> UniqueLcs(IList<T> a, IList<T> b, int aLow, int bLow, int aHigh, int bHigh)
        {
            var index = new Dictionary<T, NullablePair>(_equality);
            var act = aHigh - aLow;
            var bct = bHigh - bLow;

            // set index[line in a] = position of line in a unless
            // a is a duplicate, in which case it's set to None

            for (var i = 0; i < act; i++)
            {
                var line = a[aLow + i];
                if (index.ContainsKey(line))
                    index[line] = index[line].SetA(null);
                else
                    index[line] = new NullablePair(i, null);
            }

            // make btoa[i] = position of line i in a, unless
            // that line doesn't occur exactly once in both,
            // in which case it's set to None

            var btoa = new int?[bct];
            for (var i = 0; i < bct; i++)
            {
                var line = b[bLow + i];

                NullablePair next;
                if (index.TryGetValue(line, out next) && next.A.HasValue)
                {
                    if (next.B.HasValue)
                    {
                        btoa[next.B.Value] = null;
                        index[line] = index[line].SetA(null);
                    }
                    else
                    {
                        index[line] = index[line].SetB(i);
                        btoa[i] = next.A.Value;
                    }
                }
            }

            // this is the Patience sorting algorithm
            // see http://en.wikipedia.org/wiki/Patience_sorting

            var backpointers = new int?[bct];
            var stacksAndLasts = new List<Pair>();
            var k = 0;

            for (var bpos = 0; bpos < btoa.Length; bpos++)
            {
                var apos = btoa[bpos];
                if (!apos.HasValue) continue;

                // as an optimization, check if the next line comes at the end,
                // because it usually does
                if (stacksAndLasts.Count != 0 && stacksAndLasts[stacksAndLasts.Count - 1].A < apos.Value)
                    k = stacksAndLasts.Count;

                // as an optimization, check if the next line comes right after
                // the previous line, because usually it does
                else if (stacksAndLasts.Count != 0 &&
                    stacksAndLasts[k].A < apos &&
                    (k == stacksAndLasts.Count - 1 || stacksAndLasts[k + 1].A > apos))
                    k++;

                // find the location of the stack
                else
                {
                    k = 0;
                    for (var i = 0; i < stacksAndLasts.Count; i++)
                    {
                        if (stacksAndLasts[i].A > apos)
                        {
                            k = i;
                            break;
                        }
                    }
                }

                if (k > 0)
                    backpointers[bpos] = stacksAndLasts[k - 1].B;

                if (k < stacksAndLasts.Count)
                    stacksAndLasts[k] = new Pair(apos.Value, bpos);
                else
                    stacksAndLasts.Add(new Pair(apos.Value, bpos));
            }

            if (stacksAndLasts.Count == 0) return Enumerable.Empty<Pair>();

            var j = new int?(stacksAndLasts[stacksAndLasts.Count - 1].B);
            var result = new List<Pair>();

            while (j.HasValue)
            {
                result.Add(new Pair(btoa[j.Value].Value, j.Value));
                j = backpointers[j.Value];
            }
            result.Reverse();

            return result;
        }

        /// <summary>
        /// Recursively applies <see cref="PatienceSequenceMatcher"/> to two lists.
        /// </summary>
        /// <param name="a">The first list.</param>
        /// <param name="b">The second list.</param>
        /// <param name="aLow">The starting index in <paramref name="a"/> (inclusive).</param>
        /// <param name="bLow">The starting index in <paramref name="b"/> (inclusive).</param>
        /// <param name="aHigh">The ending index in <paramref name="a"/> (exclusive).</param>
        /// <param name="bHigh">The ending index in <paramref name="b"/> (exclusive).</param>
        /// <param name="maxRecursion">The maximum number of recursive steps to take.</param>
        /// <returns>The resulting subsequences.</returns>
        private IEnumerable<Pair> RecurseMatches(IList<T> a, IList<T> b, int aLow, int bLow, int aHigh, int bHigh, int maxRecursion)
        {
            if (maxRecursion < 0 || aLow >= aHigh || bLow >= bHigh) yield break;

            var added = false;
            var lastAPos = aLow - 1;
            var lastBPos = bLow - 1;

            foreach (var pair in UniqueLcs(a, b, aLow, bLow, aHigh, bHigh))
            {
                // recurse between lines which are unique in each file and match
                var apos = pair.A + aLow;
                var bpos = pair.B + bLow;

                // Most of the time, you will have a sequence of similar entries
                if (lastAPos + 1 != apos || lastBPos + 1 != bpos)
                {
                    foreach (var item in RecurseMatches(a, b, lastAPos + 1, lastBPos + 1, apos, bpos, maxRecursion - 1))
                    {
                        added = true;
                        yield return item;
                    }
                }

                lastAPos = apos;
                lastBPos = bpos;
                added = true;
                yield return new Pair(apos, bpos);
            }

            // find matches between the last match and the end
            if (added)
            {
                foreach (var item in RecurseMatches(a, b, lastAPos + 1, lastBPos + 1, aHigh, bHigh, maxRecursion - 1))
                    yield return item;
            }

            // find matching lines at the very beginning
            else if (_equality.Equals(a[aLow], b[bLow]))
            {
                while (aLow < aHigh && bLow < bHigh && _equality.Equals(a[aLow], b[bLow]))
                    yield return new Pair(aLow++, bLow++);
                foreach (var item in RecurseMatches(a, b, aLow, bLow, aHigh, bHigh, maxRecursion - 1))
                    yield return item;
            }

            // find matching lines at the very end
            else if (_equality.Equals(a[aHigh - 1], b[bHigh - 1]))
            {
                var nahi = aHigh - 1;
                var nbhi = bHigh - 1;

                while (nahi > aLow && nbhi > bLow && _equality.Equals(a[nahi - 1], b[nbhi - 1]))
                {
                    nahi--;
                    nbhi--;
                }

                foreach (var item in RecurseMatches(a, b, lastAPos + 1, lastBPos + 1, nahi, nbhi, maxRecursion - 1))
                    yield return item;
                for (var i = 0; i < aHigh - nahi; i++)
                    yield return new Pair(nahi + i, nbhi + i);
            }
        }

        /// <summary>
        /// Finds regions in lists of <see cref="Pair"/> where they both
        /// increment at the same time.
        /// </summary>
        /// <param name="list">The list to find sequences within.</param>
        /// <returns>The matching sequences.</returns>
        private IEnumerable<SubSequence> CollapseSequences(IEnumerable<Pair> list)
        {
            var starta = new int?();
            var startb = new int?();
            var length = 0;

            foreach (var pair in list)
            {
                var a = pair.A;
                var b = pair.B;

                if (starta.HasValue && a == starta + length && b == startb + length)
                    length += 1;
                else
                {
                    if (starta.HasValue)
                        yield return new SubSequence(starta.Value, startb.Value, length);
                    starta = a;
                    startb = b;
                    length = 1;
                }
            }

            if (length != 0)
                yield return new SubSequence(starta.Value, startb.Value, length);
        }

        /// <summary>
        /// Finds blocks within two sequences that match.
        /// </summary>
        /// <param name="left">The left-hand sequence.</param>
        /// <param name="right">The right-hand sequence.</param>
        /// <returns>
        /// A list of sequences that represents the blocks that are equal in both
        /// the left-hand and right-hand sequences, ordered by their appearance in both.
        /// </returns>
        public IEnumerable<SubSequence> FindMatchingBlocks(IList<T> left, IList<T> right)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            var matches = RecurseMatches(left, right, 0, 0, left.Count, right.Count, 10);
            return CollapseSequences(matches);
        }
    }
}
