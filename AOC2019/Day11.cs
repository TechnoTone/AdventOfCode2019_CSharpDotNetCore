using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace AOC2019
{
    public class Day11
    {
        private readonly List<long> program;

        public Day11(List<long> program) => this.program = program;

        public Dictionary<Point, bool> Run(int firstInput){

            var computer = new OpComputer(program);

            var position = Point.Empty;
            var direction = Direction.Up;
            var white = new Dictionary<Point, bool>();

            var isPaint = false;

            int checkCamera() => white.GetValueOrDefault(position) ? 1 : 0;

            computer.Input(firstInput);

            while (true)
            {
                var (response, output) = computer.Run();

                switch (response)
                {
                    case OpComputer.Response.AwaitingInput:
                        break;
                    case OpComputer.Response.Output:
                        if (isPaint = !isPaint)
                            white[position] = (output == 1);
                        else
                        {
                            direction = (output, direction) switch
                            {
                                (1, Direction.Up) => Direction.Right,
                                (1, Direction.Right) => Direction.Down,
                                (1, Direction.Down) => Direction.Left,
                                (1, Direction.Left) => Direction.Up,
                                (0, Direction.Up) => Direction.Left,
                                (0, Direction.Left) => Direction.Down,
                                (0, Direction.Down) => Direction.Right,
                                (0, Direction.Right) => Direction.Up,
                                _ => throw new InvalidProgramException()
                            };
                            position.Move(direction);
                            computer.Input(checkCamera());
                        }

                        break;
                    case OpComputer.Response.Halt:
                        return white;
                }
            }
        }
    }
}
