using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.String;

namespace AOC2019
{
    public class OpComputer
    {
        private readonly List<int> memory;
        private int pos = 0;

        public string readMemory() => Join(',', memory);
        public int readMemoryPosition(int pos) => memory[pos];

        private enum OpCodes
        {
            OpAdd = 1,
            OpMultiple = 2,
            OpHalt = 99
        }

        public OpComputer(string program)
        {
            memory =
                program
                    .Split(",")
                    .ToList()
                    .ConvertAll(int.Parse);
        }

        public OpComputer(List<int> program)
        {
            memory = program;
        }

        public void Run()
        {
            while (true)
            {
                var op = readOp();
                int result;

                switch (op)
                {
                    case OpCodes.OpAdd:
                        result = memory[read()] + memory[read()];
                        memory[read()] = result;
                        break;

                    case OpCodes.OpMultiple:
                        result = memory[read()] * memory[read()];
                        memory[read()] = result;
                        break;

                    case OpCodes.OpHalt:
                        return;
                 
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private int read() => memory[pos++];
        private OpCodes readOp() => (OpCodes) read();
    }

}
