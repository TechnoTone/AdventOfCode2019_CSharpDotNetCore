using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public class Day25
    {
        private readonly OpComputer computer;

        public Day25(string program) => computer = new OpComputer(program);

        public string Run(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                computer.Input(input.ToCharArray().Select(c => (long) c).ToList());
                computer.Input(10);
            }
            var response = computer.RunUntilHaltOrAwaitingInput();
            return getResponse(response.Item2);
        }

        private static string getResponse(List<long> list)
        {
            return string.Join("", list.Select(n => (char) n));
        }
    }
}
