using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public class Day02
    {
        private readonly List<int> program;

        public Day02(List<int> program)
        {
            this.program = program;
        }
        
        public int findSolution(int expectedResult)
        {
            var n = 0;

            while (n < 100)
            {
                n++;
                for (var i = 0; i < n; i++)
                {
                    if (runProgramWith(n, i) == expectedResult)
                        return (n * 100 + i);

                    if (runProgramWith(i, n) == expectedResult)
                        return (i * 100 + n);
                }
            }

            return 0;
        }

        private int runProgramWith(int n, int v)
        {
            var temp = program.ToList();
            temp[1] = n;
            temp[2] = v;
            var computer = new OpComputer(temp);
            computer.Run();
            var result = computer.readMemoryPosition(0);
            return result;
        }

    }
}
