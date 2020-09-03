using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace AOC2019.Tests
{
    public class Day25Tests : TestBase
    {
        /*
         * --- Day 25: Cryostasis ---
         *
         * As you approach Santa's ship, your sensors report two
         * important details:
         *
         * First, that you might be too late: the internal temperature
         * is -40 degrees.
         *
         * Second, that one faint life signature is somewhere on the
         * ship.
         *
         * The airlock door is locked with a code; your best option is
         * to send in a small droid to investigate the situation. You
         * attach your ship to Santa's, break a small hole in the hull,
         * and let the droid run in before you seal it up again. Before
         * your ship starts freezing, you detach your ship and set it to
         * automatically stay within range of Santa's ship.
         *
         * This droid can follow basic instructions and report on its
         * surroundings; you can communicate with it through an Intcode
         * program (your puzzle input) running on an ASCII-capable
         * computer.
         *
         * As the droid moves through its environment, it will describe
         * what it encounters. When it says Command?, you can give it a
         * single instruction terminated with a newline (ASCII code 10).
         * Possible instructions are:
         *
         * - Movement via north, south, east, or west.
         * - To take an item the droid sees in the environment, use the
         *   command take <name of item>. For example, if the droid
         *   reports seeing a red ball, you can pick it up with take red
         *   ball.
         * - To drop an item the droid is carrying, use the command drop
         *   <name of item>. For example, if the droid is carrying a
         *   green ball, you can drop it with drop green ball.
         * - To get a list of all of the items the droid is currently
         *   carrying, use the command inv (for "inventory").
         *
         * Extra spaces or other characters aren't allowed -
         * instructions must be provided precisely.
         *
         * Santa's ship is a Reindeer-class starship; these ships use
         * pressure-sensitive floors to determine the identity of droids
         * and crew members. The standard configuration for these
         * starships is for all droids to weigh exactly the same amount
         * to make them easier to detect. If you need to get past such a
         * sensor, you might be able to reach the correct weight by
         * carrying items from the environment.
         *
         * Look around the ship and see if you can find the password for
         * the main airlock.
         *
         */
        public Day25Tests() : base(25) { }

        [Test]
        public void Part1()
        {
            var day25 = new Day25(input.readAllText());

            var commands = new List<string>
            {
                "",
                "north","west", "take mug",
                "west", "take easter egg",
                "east", "east", "south", "south", "take asterisk",
                "south", //"take escape pod",
                "west", //"take photons",
                "north", "take jam",
                "south", "east", "north", "east", "take klein bottle",
                "south", //"take molten lava",
                "west", "take tambourine",
                "west", "take cake",
                "east", "south", //"take infinite loop",
                "east", "take polygon",
                "north", "inv",
            };

            var response = commands.Select(day25.Run).ToList().Last();
            var inventory = response.Split('\n').Where(s => s.StartsWith("-")).Select(s => s.Remove(0, 2)).ToList();

            static string drop(string item) => item.Insert(0, "drop ");
            static string take(string item) => item.Insert(0, "take ");

            inventory.Select(drop).Select(day25.Run).ToList();

            for (var size = 1; size <= inventory.Count; size++)
            {
                var perms = inventory.Permutations(size);
                foreach (var perm in perms)
                {
                    perm.Select(take).Select(day25.Run).ToList();

                    response = day25.Run("east");
                    if (response.Contains("You may proceed"))
                    {
                        var code = response.Substring(response.IndexOf("typing") + 7);
                        code = code.Substring(0, code.IndexOf(" "));

                        code.Should().Be("201327120");
                        return;
                    }

                    perm.Select(drop).Select(day25.Run).ToList();
                }
            }
        }
        /*
         *
         *
         * $.$.#.#
         *     .
         *     @
         *     .
         *   $ $.$
         *   . . .
         * #.$.$ .
         *       .
         *   $.$.$ #.X
         *   . .   .
         *   # $...$
         */
    }
}
