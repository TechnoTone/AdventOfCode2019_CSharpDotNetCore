using System;
using FluentAssertions;
using NUnit.Framework;

namespace AOC2019.Tests
{
    /*
     * --- Day 4: Secure Container ---
     *
     * You arrive at the Venus fuel depot only to discover it's
     * protected by a password. The Elves had written the password on a
     * sticky note, but someone threw it out.
     * 
     * However, they do remember a few key facts about the password:
     * 
     * It is a six-digit number.
     * The value is within the range given in your puzzle input.
     * Two adjacent digits are the same (like 22 in 122345).
     * Going from left to right, the digits never decrease; they only
     * ever increase or stay the same (like 111123 or 135679).
     * Other than the range rule, the following are true:
     * 
     * 111111 meets these criteria (double 11, never decreases).
     * 223450 does not meet these criteria (decreasing digits).
     * 123789 does not meet these criteria (no double).
     * 
     * How many different passwords within the range given in your
     * puzzle input meet these criteria?
     *
     * --- Part Two ---
     *
     * An Elf just remembered one more important detail: the two
     * adjacent matching digits are not part of a larger group of
     * matching digits.
     *
     * Given this additional criterion, but still ignoring the range
     * rule, the following are now true:
     *
     * 112233 meets these criteria because the digits never decrease and
     *             all repeated digits are exactly two digits long.
     * 123444 no longer meets the criteria
     *             (the repeated 44 is part of a larger group of 444).
     * 111122 meets the criteria (even though 1 is repeated more than
     *             twice, it still contains a double 22).
     *
     * How many different passwords within the range given in your
     * puzzle input meet all of the criteria?
     *
     */
    public class Day04Tests : TestBase
    {
        private readonly Tuple<int,int> range;

        public Day04Tests() : base(4)
        {
            range = input.range();
        }

        [Test]
        [TestCase(11111, false)]  // 5 digits
        [TestCase(111111, false)] // below range
        [TestCase(234567, false)] // no repeats
        [TestCase(334545, false)] // decreasing sequence
        [TestCase(888888, true)]  // VALID (even though it is a big repeat group)
        [TestCase(999999, false)] // above range
        public void Part1Examples(int password, bool valid)
        {
            Day04.isValid1(password, 200000, 900000).Should().Be(valid);
        }

        [Test]
        [TestCase(111111, false)] // repeat group is too big
        [TestCase(112222, true)]
        public void Part2Examples(int password, bool valid) =>
            Day04.isValid2(password, 0, 999999).Should().Be(valid);

        [Test]
        public void Part1() => 
            Day04.validInRange1(range.Item1, range.Item2).Should().Be(1048);

        [Test]
        public void Part2() => 
            Day04.validInRange2(range.Item1, range.Item2).Should().Be(677);
    }
}
