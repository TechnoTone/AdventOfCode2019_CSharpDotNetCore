using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AOC2019
{
    public class Day17
    {
        private readonly OpComputer computer;
        public string CameraView { get; }

        private const char CAMERA_SCAFFOLD = '#';
        private const char CAMERA_EMPTY_SPACE = '.';
        private const char CAMERA_ROBOT_UP = '^';
        private const char CAMERA_ROBOT_DOWN = 'v';
        private const char CAMERA_ROBOT_LEFT = '<';
        private const char CAMERA_ROBOT_RIGHT = '>';

        public Day17(List<long> program)
        {
            computer = new OpComputer(program);
            var output = computer.RunUntilHalt();
            CameraView = GetCameraView(output);
        }

        private static string GetCameraView(List<long> output) =>
            new string(output.Select(x => (char)x).ToArray());

        public static int CalculateAlignment(string[] mapStrings) =>
            new Map(mapStrings).Intersections().Select(p => p.X * p.Y).Sum();

        private class Map
        {
            private readonly Dictionary<Point,char> map;

            private readonly int width;
            private readonly int height;

            public Map(IReadOnlyList<string> mapStrings)
            {
                var nonEmptyLines = mapStrings.Where(s => !string.IsNullOrEmpty(s)).ToList();
                width = nonEmptyLines[0].Length;
                height = nonEmptyLines.Count;

                map = allCoordinates().ToDictionary(p => p, p => mapStrings[p.Y][p.X]);
            }

            private IEnumerable<Point> allCoordinates()
            {
                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    yield return new Point(x, y);
            }

            private char at(Point p) =>
                map.ContainsKey(p) ? map[p] : CAMERA_EMPTY_SPACE;

            private bool isIntersection(Point p) =>
                at(p) == CAMERA_SCAFFOLD &&
                at(p.Shift(Direction.Up)) == CAMERA_SCAFFOLD &&
                at(p.Shift(Direction.Down)) == CAMERA_SCAFFOLD &&
                at(p.Shift(Direction.Left)) == CAMERA_SCAFFOLD &&
                at(p.Shift(Direction.Right)) == CAMERA_SCAFFOLD;


            private bool isScaffold(Point p) => map.ContainsKey(p) && map[p] == CAMERA_SCAFFOLD;

            private bool isRobot(Point p) => isRobot(map[p]);

            private static bool isRobot(char c) => c switch
            {
                CAMERA_ROBOT_UP => true,
                CAMERA_ROBOT_DOWN => true,
                CAMERA_ROBOT_LEFT => true,
                CAMERA_ROBOT_RIGHT => true,
                _ => false
            };

            private static char directionChar(Direction direction) => direction switch
            {
                Direction.Up => '^',
                Direction.Down => 'v',
                Direction.Left => '<',
                Direction.Right => '>',
                _ => 'X'
            };


            public IEnumerable<Point> Intersections() => allCoordinates().Where(isIntersection);

            public (Point, Direction) GetRobot()
            {
                var position = map.Keys.First(isRobot);
                var direction = map[position] switch
                {
                    CAMERA_ROBOT_UP => Direction.Up,
                    CAMERA_ROBOT_DOWN => Direction.Down,
                    CAMERA_ROBOT_LEFT => Direction.Left,
                    CAMERA_ROBOT_RIGHT => Direction.Right,
                    _ => throw new Exception()
                };

                return (position, direction);
            }

            public bool CanMoveForward((Point, Direction) robot) =>
                isScaffold(robot.Item1.Shift(robot.Item2));

            public void MoveRobotForward(ref (Point, Direction) robot)
            {
                map[robot.Item1] = CAMERA_SCAFFOLD;
                robot.Item1.Move(robot.Item2);
                map[robot.Item1] = directionChar(robot.Item2);
            }

            public void TurnRobotLeft(ref (Point, Direction) robot)
            {
                robot.Item2 = robot.Item2 switch
                {
                    Direction.Up => Direction.Left,
                    Direction.Right => Direction.Up,
                    Direction.Down => Direction.Right,
                    Direction.Left => Direction.Down,
                    _ => throw new Exception()
                };
                map[robot.Item1] = directionChar(robot.Item2);
            }
        }

        private interface IProgramComponent { }

        private class MovementFunction : IProgramComponent
        {
            public char Name;
            public override string ToString() { return Name.ToString(); }
        }

        public class TurnThenStep : IProgramComponent, IEquatable<TurnThenStep>
        {
            private readonly Direction direction;
            public int Steps;

            private string DirectionString => direction switch
            {
                Direction.Left => "L",
                Direction.Right => "R",
                _ => ""
            };

            public TurnThenStep(Direction direction) => this.direction = direction;

            public override string ToString() => $"{DirectionString},{Steps}";

            public bool Equals(TurnThenStep other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return direction == other.direction && Steps == other.Steps;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == GetType() && Equals((TurnThenStep) obj);
            }

            public override int GetHashCode() { return HashCode.Combine((int) direction, Steps); }
        }

        public static IEnumerable<TurnThenStep> GetRoute(string[] cameraView)
        {
            var map = new Map(cameraView);
            var robot = map.GetRobot();

            TurnThenStep next = null;

            while (true)
            {
                if (map.CanMoveForward(robot))
                {
                    if (next != null) next.Steps++;
                    map.MoveRobotForward(ref robot);
                }
                else
                {
                    if (next != null) yield return next;

                    map.TurnRobotLeft(ref robot);
                    if (map.CanMoveForward(robot))
                    {
                        next = new TurnThenStep(Direction.Left);
                        continue;
                    }

                    map.TurnRobotLeft(ref robot);
                    map.TurnRobotLeft(ref robot);

                    if (!map.CanMoveForward(robot)) yield break;

                    next = new TurnThenStep(Direction.Right);
                }
            }

        }

        public class RobotProgram
        {
            public List<char> MovementRoutine = new List<char>();
            public readonly Dictionary<char, List<TurnThenStep>> MovementFunctions = new Dictionary<char, List<TurnThenStep>>();

            private static IEnumerable<long> compileString<T>(List<T> lst) =>
                string.Join(',', lst).ToCharArray().Select(x => (long) x).Append(10);

            public List<long> Compile()
            {
                var output = new List<long>();

                output.AddRange(compileString(MovementRoutine));
                MovementFunctions.Values.Select(compileString).ToList().ForEach(output.AddRange);
                output.AddRange(new long[] {'n', 10});

                return output;
            }
        }

        public static RobotProgram GetRobotProgram(IEnumerable<TurnThenStep> route)
        {
            var program = new RobotProgram();
            var commands = route.Select(x => (IProgramComponent) x).ToList();

            var pos = 0;
            var name = 'A';

            char nextFunctionName() => name++;

            while (pos < commands.Count)
            {
                var len = 5;
                while (commands[pos] is TurnThenStep)
                {
                    var candidate = commands.Skip(pos).Take(len).ToList();
                    if (candidate.All(x => x is TurnThenStep) && commands.SubListIndex(pos + len, candidate) >= 0)
                    {
                        var nextRoutine = new MovementFunction
                        {
                            Name = nextFunctionName()
                        };
                        program.MovementFunctions.Add(nextRoutine.Name,
                            candidate.Select(x => x as TurnThenStep).ToList());
                        var matchIx = commands.SubListIndex(pos, candidate);
                        while (matchIx >= 0)
                        {
                            commands.RemoveRange(matchIx, len);
                            commands.Insert(matchIx, nextRoutine);
                            matchIx = commands.SubListIndex(pos, candidate);
                        }
                    }

                    len--;
                }

                pos++;
            }

            program.MovementRoutine =
                commands.Select(x => ((MovementFunction) x).Name).ToList();

            return program;
        }

        public void AwakenRobot()
        {
            computer.WriteMemoryPosition(0, 2);
        }

        public long runRobotProgram(RobotProgram robotProgram)
        {
            computer.Input(robotProgram.Compile());
            var outputs = computer.RunUntilHalt();

            return outputs.Last();
        }
    }
}
