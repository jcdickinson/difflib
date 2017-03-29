using System;
using System.Linq;
using Xunit;

namespace DiffLib.Specifications
{
    public class PatienceTests
    {
        private static readonly Differencer<string> Differ = new PatienceSequenceMatcher<string>(StringComparer.Ordinal).CreateDifferencer();

        private static string[] Lines(params string[] param) { return param; }

        private static DifferenceInstruction[] Diff(string[] left, string[] right)
        {
            return Differ.FindDifferences(left, right).ToArray();
        }

        [Fact(DisplayName = "PatienceDiff FindMatchingBlocks - Add new line")]
        public void PatienceDiff_FindMatchingBlocks_AddNewLine()
        {
            var left = Lines("boo");
            var right = Lines("boo", "");
            var instructions = Diff(left, right);

            Assert.Equal(2, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 0, 1, 1)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(1, 1, 0, 1)), instructions[1]);
        }

        [Fact(DisplayName = "PatienceDiff FindMatchingBlocks - Remove new line")]
        public void PatienceDiff_FindMatchingBlocks_RemoveNewLine()
        {
            var left = Lines("boo", "");
            var right = Lines("boo");
            var instructions = Diff(left, right);

            Assert.Equal(2, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 0, 1, 1)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(1, 0, 1, 0)), instructions[1]);
        }

        [Fact(DisplayName = "PatienceDiff FindMatchingBlocks - Change and add new line")]
        public void PatienceDiff_FindMatchingBlocks_ChangeAndNewLine()
        {
            var left = Lines("boo");
            var right = Lines("goo", "");
            var instructions = Diff(left, right);

            Assert.Equal(2, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(0, 0, 1, 0)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(0, 0, 0, 2)), instructions[1]);
        }

        [Fact(DisplayName = "PatienceDiff FindMatchingBlocks - Add content to start")]
        public void PatienceDiff_FindMatchingBlocks_AddContentStart()
        {
            var left = Lines(
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            var right = Lines(
                "/**",
                " * Simple exception.",
                " */",
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            var instructions = Diff(left, right);

            Assert.Equal(2, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(0, 0, 0, 3)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 3, 3, 3)), instructions[1]);
        }

        [Fact(DisplayName = "PatienceDiff FindMatchingBlocks - Remove content from start")]
        public void PatienceDiff_FindMatchingBlocks_RemoveContentStart()
        {
            var left = Lines(
                "/**",
                " * Simple exception.",
                " */",
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            var right = Lines(
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            var instructions = Diff(left, right);

            Assert.Equal(2, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(0, 0, 3, 0)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(3, 0, 3, 3)), instructions[1]);
        }

        [Fact(DisplayName = "PatienceDiff FindMatchingBlocks - Remove content from start and add to end")]
        public void PatienceDiff_FindMatchingBlocks_RemoveContentStartAddEnd()
        {
            var left = Lines(
                "/**",
                " * Simple exception.",
                " */",
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            var right = Lines(
                "public  class MyException extends Exception {",
                "",
                "}",
                "/**",
                " * Removed some comment.",
                " */"
            );
            var instructions = Diff(left, right);

            Assert.Equal(3, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(0, 0, 3, 0)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(3, 0, 3, 3)), instructions[1]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(6, 3, 0, 3)), instructions[2]);

        }

        [Fact(DisplayName = "PatienceDiff FindMatchingBlocks - Add content to middle")]
        public void PatienceDiff_FindMatchingBlocks_AddContentMid()
        {
            var left = Lines(
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            var right = Lines(
                "public  class MyException extends Exception {",
                "",
                "/**",
                " * Simple exception.",
                " */",
                "}"
            );
            var instructions = Diff(left, right);

            Assert.Equal(3, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 0, 2, 2)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(0, 2, 0, 3)), instructions[1]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(2, 5, 1, 1)), instructions[2]);

        }
    }
}
