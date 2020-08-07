using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AOC2019
{
    public class Day15
    {
        private readonly List<long> program;

        public Day15(List<long> program) => this.program = program;

        public enum MapLocation
        {
            Start,
            Unknown,
            Hallway,
            Wall,
            OxygenSystem
        }

        private static IEnumerable<CompassDirection> AllCompassDirections => new List<CompassDirection>
        {
            CompassDirection.North, CompassDirection.South, CompassDirection.West, CompassDirection.East
        };

        private class RepairDroid
        {
            private readonly OpComputer computer;

            public RepairDroid(List<long> program) => computer = new OpComputer(program);

            private void move(CompassDirection direction) => computer.Input((long) direction);

            private static CompassDirection moveBackwards(Stack<CompassDirection> route) =>
                route.Pop() switch
                {
                    CompassDirection.North => CompassDirection.South,
                    CompassDirection.South => CompassDirection.North,
                    CompassDirection.West => CompassDirection.East,
                    CompassDirection.East => CompassDirection.West,
                    _ => throw new ArgumentOutOfRangeException()
                };

            public Dictionary<Point, MapLocation> ExploreEntireShip()
            {
                var currentMapLocation = Point.Empty;
                var route = new Stack<CompassDirection>();

                var map = new Dictionary<Point, MapLocation>
                {
                    {currentMapLocation, MapLocation.Start}
                };

                MapLocation readMap(Point coordinate)
                {
                    return map.ContainsKey(coordinate) ? map[coordinate] : MapLocation.Unknown;
                }

                bool directionIsUnexplored(CompassDirection direction)
                {
                    var mapLocation = readMap(currentMapLocation.Shift(direction));
                    return mapLocation == MapLocation.Unknown;
                }

                bool hasUnexploredDirection() => AllCompassDirections.Any(directionIsUnexplored);
                CompassDirection nextUnexploredDirection() => AllCompassDirections.First(directionIsUnexplored);

                var direction = CompassDirection.None;

                while (route.Count > 0 || hasUnexploredDirection())
                {
                    while (hasUnexploredDirection())
                        switch (computer.Run())
                        {
                            case (OpComputer.Response.Halt, _):
                                throw new InvalidOperationException("Repair droid program halted unexpectedly.");
                            case (OpComputer.Response.Output, 0):
                                map[currentMapLocation.Shift(direction)] = MapLocation.Wall;
                                break;
                            case (OpComputer.Response.Output, 1):
                                currentMapLocation.Move(direction);
                                map[currentMapLocation] = MapLocation.Hallway;
                                route.Push(direction);
                                break;
                            case (OpComputer.Response.Output, 2):
                                currentMapLocation.Move(direction);
                                map[currentMapLocation] = MapLocation.OxygenSystem;
                                route.Push(direction);
                                break;
                            case (OpComputer.Response.AwaitingInput, _):
                                direction = nextUnexploredDirection();
                                move(direction);
                                break;
                        }

                    while (!hasUnexploredDirection() && route.Count > 0)
                        switch (computer.Run())
                        {
                            case (OpComputer.Response.Halt, _):
                                throw new InvalidOperationException("Repair droid program halted unexpectedly.");
                            case (OpComputer.Response.Output, 0):
                                throw new InvalidOperationException("Repair droid hit a wall unexpectedly.");
                            case (OpComputer.Response.Output, _):
                                currentMapLocation.Move(direction);
                                break;
                            case (OpComputer.Response.AwaitingInput, _):
                                direction = moveBackwards(route);
                                move(direction);
                                break;
                        }
                }

                return map;
            }
        }

        public Dictionary<Point, MapLocation> ShipExplorationResult() =>
            new RepairDroid(program).ExploreEntireShip();

        public int StepsToOxygenSystem()
        {
            var shipMap = ShipExplorationResult();

            var locations = new List<Point> {Point.Empty};
            var visited = new HashSet<Point> {Point.Empty};

            var stepsTaken = 0;

            bool isPossibility(Point location) =>
                shipMap[location] switch
                {
                    MapLocation.Hallway => !visited.Contains(location),
                    MapLocation.OxygenSystem => !visited.Contains(location),
                    _ => false
                };

            while (true)
            {
                var next = new List<Point>();
                foreach (var location in locations)
                {
                    if (shipMap[location] == MapLocation.OxygenSystem) return stepsTaken;
                    visited.Add(location);
                    next.AddRange(AllCompassDirections.Select(d => location.Shift(d)).Where(isPossibility));
                }

                locations = next;
                stepsTaken++;
            }
        }

        public int MinutesToFillWithOxygen()
        {
            var shipMap = ShipExplorationResult();

            var locationOfOxygenSystem = shipMap.First(x => x.Value == MapLocation.OxygenSystem).Key;

            var locations = new List<Point> {locationOfOxygenSystem};
            var visited = new HashSet<Point> {locationOfOxygenSystem};

            var minutes = 0;

            bool isPossibility(Point location) =>
                shipMap[location] switch
                {
                    MapLocation.Hallway => !visited.Contains(location),
                    MapLocation.Start => !visited.Contains(location),
                    _ => false
                };

            while (locations.Count > 0)
            {
                var next = new List<Point>();
                foreach (var location in locations)
                {
                    visited.Add(location);
                    next.AddRange(AllCompassDirections.Select(d => location.Shift(d)).Where(isPossibility));
                }

                locations = next;
                minutes++;
            }

            return minutes - 1;
        }
    }
}
