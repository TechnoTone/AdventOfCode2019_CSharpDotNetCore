using System;
using System.Collections.Generic;

namespace AOC2019
{
    public static class Day05
    {
        public static List<int> runTestProgram(List<int> program, int systemID)
        {
            var computer = new OpComputer(program);
            computer.Input(systemID);

            var outputs = new List<int>();

            while (!computer.Halted)
            {
                computer.Run();
                while (computer.HasOutput)
                    outputs.Add(computer.ReadOutput());
            }

            return outputs;
        }
    }
}
