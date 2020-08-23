using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public class Day19
    {
        private readonly string program;

        public Day19(string program) => this.program = program;

        private bool sendTestDrone(long x, long y) =>
            new OpComputer(program).RunUntilHalt(new List<long> {x, y})[0] == 1;

        public int GridScanCount(int sizeToScan)
        {
            IEnumerable<int> range() => Enumerable.Range(0, sizeToScan);
            return range().Sum(x => range().Count(y => sendTestDrone(x, y)));
        }

        public int FindTopLeftOf100Square()
        {
            var y = 200;
            var x = 0;
            while (true)
            {
                y++;
                while (!sendTestDrone(x, y)) x++;
                if (sendTestDrone(x, y - 99) && sendTestDrone(x + 99, y - 99))
                    return x * 10000 + y - 99;
            }
        }
    }
}
