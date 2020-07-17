using System.Collections.Generic;

namespace AOC2019
{
    public static class Day09
    {
        public static List<long> runTest(string program) =>
            new OpComputer(program).RunUntilHalt(1);

        public static List<long> runBoost(string program) =>
            new OpComputer(program).RunUntilHalt(2);
    }
}
