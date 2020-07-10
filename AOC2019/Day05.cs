using System.Collections.Generic;

namespace AOC2019
{
    public static class Day05
    {
        public static List<int> runTestProgram(List<int> program, int systemID)
        {
            var computer = new OpComputer(program);
            computer.input(systemID);
            computer.Run();

            var outputs = new List<int>();
            while (computer.hasOutput)
                outputs.Add(computer.readOutput());

            return outputs;
        }
    }
}
