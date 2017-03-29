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

        [Fact(DisplayName = "When diffing a file that adds a new line")]
        public void When_diffing_a_file_that_adds_a_new_line()
        {
            var left = Lines("boo");
            var right = Lines("boo", "");
            var instructions = Diff(left, right);

            Assert.Equal(2, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 0, 1, 1)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(1, 1, 0, 1)), instructions[1]);
        }

        [Fact(DisplayName = "When diffing a file that removes_a new line")]
        public void When_diffing_a_file_that_removes_a_new_line()
        {
            var left = Lines("boo", "");
            var right = Lines("boo");
            var instructions = Diff(left, right);

            Assert.Equal(2, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 0, 1, 1)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(1, 0, 1, 0)), instructions[1]);
        }

        [Fact(DisplayName = "When diffing a file that changes and adds a new line")]
        public void When_diffing_a_file_that_changes_and_adds_a_new_line()
        {
            var left = Lines("boo");
            var right = Lines("goo", "");
            var instructions = Diff(left, right);

            Assert.Equal(2, instructions.Length);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(0, 0, 1, 0)), instructions[0]);
            Assert.Equal(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(0, 0, 0, 2)), instructions[1]);
        }

        [Fact(DisplayName = "When diffing a file that adds content at the start")]
        public void When_diffing_a_file_that_adds_content_at_the_start()
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

        [Fact(DisplayName = "When diffing a file that removes content at the start")]
        public void When_diffing_a_file_that_removes_content_at_the_start()
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

        [Fact(DisplayName = "When diffing a file that removes content at the start and adds to the end")]
        public void When_diffing_a_file_that_removes_content_at_the_start_and_adds_to_the_end()
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
    }
}
