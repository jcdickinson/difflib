using Machine.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffLib.Specifications
{
    class Patience
    {
        protected static readonly Differencer<string> Differ = new PatienceSequenceMatcher<string>(StringComparer.Ordinal).CreateDifferencer();

        protected static string[] Lines(params string[] param) { return param; }

        protected static DifferenceInstruction[] Diff(string[] left, string[] right)
        {
            return Differ.FindDifferences(left, right).ToArray();
        }
    }

    [Subject(typeof(PatienceSequenceMatcher<>))]
    class when_diffing_a_file_that_adds_a_new_line : Patience
    {
        static string[] left;
        static string[] right;
        static DifferenceInstruction[] instructions;

        Establish context = () =>
        {
            left = Lines(
                "boo"
            );
            right = Lines(
                "boo",
                ""
            );
        };

        Because of = () => instructions = Diff(left, right);

        It should_have_two_instructions = () => instructions.Length.ShouldEqual(2);
        It should_give_an_equal_instruction_for_the_unchanged_code = () =>
            instructions[0].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 0, 1, 1)));
        It should_give_an_add_instruction_for_the_new_line = () =>
            instructions[1].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(1, 1, 0, 1)));
    }

    [Subject(typeof(PatienceSequenceMatcher<>))]
    class when_diffing_a_file_that_removes_a_new_line : Patience
    {
        static string[] left;
        static string[] right;
        static DifferenceInstruction[] instructions;

        Establish context = () =>
        {
            left = Lines(
                "boo",
                ""
            );
            right = Lines(
                "boo"
            );
        };

        Because of = () => instructions = Diff(left, right);

        It should_have_two_instructions = () => instructions.Length.ShouldEqual(2);
        It should_give_an_equal_instruction_for_the_unchanged_code = () =>
            instructions[0].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 0, 1, 1)));
        It should_give_a_remove_instruction_for_the_old_line = () =>
            instructions[1].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(1, 0, 1, 0)));
    }

    [Subject(typeof(PatienceSequenceMatcher<>))]
    class when_diffing_a_file_that_changes_and_adds_a_new_line : Patience
    {
        static string[] left;
        static string[] right;
        static DifferenceInstruction[] instructions;

        Establish context = () =>
        {
            left = Lines(
                "boo"
            );
            right = Lines(
                "goo",
                ""
            );
        };

        Because of = () => instructions = Diff(left, right);

        It should_have_two_instructions = () => instructions.Length.ShouldEqual(2);
        It should_give_a_remove_instruction_for_the_old_code = () =>
            instructions[0].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(0, 0, 1, 0)));
        It should_give_an_add_instruction_for_the_new_code = () =>
            instructions[1].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(0, 0, 0, 2)));
    }

    [Subject(typeof(Differencer<>))]
    class when_diffing_a_file_that_adds_content_at_the_start : Patience
    {
        static string[] left;
        static string[] right;
        static DifferenceInstruction[] instructions;

        Establish context = () =>
        {
            left = Lines(
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            right = Lines(
                "/**",
                " * Simple exception.",
                " */",
                "public  class MyException extends Exception {",
                "",
                "}"
            );
        };

        Because of = () => instructions = Diff(left, right);

        It should_have_two_instructions = () => instructions.Length.ShouldEqual(2);
        It should_give_an_add_instruction_for_the_new_code = () =>
            instructions[0].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(0, 0, 0, 3)));
        It should_give_an_equal_instruction_for_the_unchanged_code = () =>
            instructions[1].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(0, 3, 3, 3)));
    }

    [Subject(typeof(Differencer<>))]
    class when_diffing_a_file_that_removes_content_at_the_start : Patience
    {
        static string[] left;
        static string[] right;
        static DifferenceInstruction[] instructions;

        Establish context = () =>
        {
            left = Lines(
                "/**",
                " * Simple exception.",
                " */",
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            right = Lines(
                "public  class MyException extends Exception {",
                "",
                "}"
            );
        };

        Because of = () => instructions = Diff(left, right);

        It should_have_two_instructions = () => instructions.Length.ShouldEqual(2);
        It should_give_a_remove_instruction_for_the_old_code = () =>
            instructions[0].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(0, 0, 3, 0)));
        It should_give_an_equal_instruction_for_the_unchanged_code = () =>
            instructions[1].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(3, 0, 3, 3)));
    }

    [Subject(typeof(Differencer<>))]
    class when_diffing_a_file_that_removes_content_at_the_start_and_adds_to_the_end : Patience
    {
        static string[] left;
        static string[] right;
        static DifferenceInstruction[] instructions;

        Establish context = () =>
        {
            left = Lines(
                "/**",
                " * Simple exception.",
                " */",
                "public  class MyException extends Exception {",
                "",
                "}"
            );
            right = Lines(
                "public  class MyException extends Exception {",
                "",
                "}",
                "/**",
                " * Removed some comment.",
                " */"
            );
        };

        Because of = () => instructions = Diff(left, right);

        It should_have_two_instructions = () => instructions.Length.ShouldEqual(3);
        It should_give_a_remove_instruction_for_the_old_code = () =>
            instructions[0].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Removed, new SubSequence(0, 0, 3, 0)));
        It should_give_an_equal_instruction_for_the_unchanged_code = () =>
            instructions[1].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Equal, new SubSequence(3, 0, 3, 3)));
        It should_give_an_add_instruction_for_the_new_code = () =>
            instructions[2].ShouldEqual(new DifferenceInstruction(DifferenceOperation.Inserted, new SubSequence(6, 3, 0, 3)));
    }
}
