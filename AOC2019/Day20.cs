using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AOC2019
{
    public class Day20
    {
        private readonly Maze maze;

        public Day20(List<string> map) => maze = new Maze(map);

        public class Maze
        {
            public readonly Dictionary<string, Point> OuterPortalNames;
            public readonly Dictionary<string, Point> InnerPortalNames;
            public readonly Dictionary<Point, string> OuterPortalPoints;
            public readonly Dictionary<Point, string> InnerPortalPoints;
            public readonly Dictionary<Point, Point> Portals;
            public Point Entrance { get; }
            public Point Exit { get; }
            public Dictionary<Point, List<Path>> Paths { get; }

            public Maze(List<string> map)
            {
                var width = map[0].Length - 4;
                var height = map.Count - 4;

                var portals = new List<(string, Point)>();

                string vPortalName(int x, int y) => new string(new[] {map[y][x], map[y + 1][x]});
                string hPortalName(int x, int y) => map[y].Substring(x, 2);

                for (var x = 0; x < width - 1; x++)
                {
                    if (map[0][x + 2] != ' ')
                        portals.Add((vPortalName(x + 2, 0), new Point(x, 0)));

                    if (map[height + 2][x + 2] != ' ')
                        portals.Add((vPortalName(x + 2, height + 2), new Point(x, height - 1)));
                }

                for (var y = 0; y < height - 1; y++)
                {
                    if (map[y + 2][0] != ' ')
                        portals.Add((hPortalName(0, y + 2), new Point(0, y)));
                    if (map[y + 2][width + 2] != ' ')
                        portals.Add((hPortalName(width + 2, y + 2), new Point(width - 1, y)));
                }

                OuterPortalNames = portals.ToDictionary(x => x.Item1, x => x.Item2);
                OuterPortalPoints = portals.ToDictionary(x => x.Item2, x => x.Item1);

                Entrance = OuterPortalNames["AA"];
                Exit = OuterPortalNames["ZZ"];

                OuterPortalPoints.Remove(OuterPortalNames["AA"]);
                OuterPortalPoints.Remove(OuterPortalNames["ZZ"]);
                OuterPortalNames.Remove("AA");
                OuterPortalNames.Remove("ZZ");

                portals.Clear();

                map = map.Skip(2).Take(height).Select(s => s.Substring(2, width)).ToList();

                var holeTop = map.Select((s, i) => (s, i)).First(x => x.s.Contains(' ')).i;
                var holeBottom = map.Select((s, i) => (s, i)).Last(x => x.s.Contains(' ')).i;
                var holeLeft = map[holeTop].IndexOf(' ');
                var holeRight = map[holeBottom].LastIndexOf(' ');

                for (var x = holeLeft + 1; x < holeRight; x++)
                {
                    if (map[holeTop][x] != ' ')
                        portals.Add((vPortalName(x, holeTop), new Point(x, holeTop - 1)));

                    if (map[holeBottom][x] != ' ')
                        portals.Add((vPortalName(x, holeBottom - 1), new Point(x, holeBottom + 1)));
                }

                for (var y = holeTop + 1; y < holeBottom; y++)
                {
                    if (map[y][holeLeft] != ' ')
                        portals.Add((hPortalName(holeLeft, y), new Point(holeLeft - 1, y)));
                    if (map[y][holeRight] != ' ')
                        portals.Add((hPortalName(holeRight - 1, y), new Point(holeRight + 1, y)));
                }

                InnerPortalNames = portals.ToDictionary(x => x.Item1, x => x.Item2);
                InnerPortalPoints = portals.ToDictionary(x => x.Item2, x => x.Item1);

                Portals = OuterPortalNames.Keys.ToDictionary(s => OuterPortalNames[s], s => InnerPortalNames[s]);

                Paths = buildPathsGraph(map);
            }

            private Dictionary<Point, List<Path>> buildPathsGraph(List<string> map) =>
                OuterPortalNames.Values.Concat(InnerPortalNames.Values).Concat(new[] {Entrance})
                    .ToDictionary(position => position, position=>pathsFrom(position,map));

            private List<Path> pathsFrom(Point origin, IReadOnlyList<string> map)
            {
                var width = map[0].Length;
                var height = map.Count;

                var paths = new List<Path>();
                var visited = new List<Point>();
                var queue = new Queue<Path>();

                queue.Enqueue(new Path(origin, 0));

                bool isPortal(Point p) =>
                    p != origin &&
                    OuterPortalNames.Values.Concat(InnerPortalNames.Values).Concat(new[] {Exit}).Contains(p);

                bool isPath(Point p) =>
                    p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height && map[p.Y][p.X] == '.';

                while (queue.Any())
                {
                    var current = queue.Dequeue();

                    if (visited.Contains(current.Destination)) continue;
                    visited.Add(current.Destination);

                    if (isPortal(current.Destination))
                        paths.Add(current);
                    else
                        foreach (var p in current.Destination.Surrounding4().Where(isPath))
                            queue.Enqueue(new Path(p, current.Distance + 1));
                }

                return paths;
            }

            public bool IsInnerPortal(Point point) => InnerPortalPoints.ContainsKey(point);
            public bool IsOuterPortal(Point point) => OuterPortalPoints.ContainsKey(point);

            public bool IsExit(Point point) => point == Exit;
        }

        public class Path
        {
            public Point Destination { get; }
            public int Distance { get; }

            public Path(Point destination, int distance)
            {
                Destination = destination;
                Distance = distance;
            }
        }

        private class State
        {
            public Point Point { get; }
            public int Level { get; }
            public int Distance { get; }
            public HashSet<Point> Visited { get; }

            public State(Point point, int level = 0, int distance = 0, HashSet<Point> visited = null)
            {
                Point = point;
                Level = level;
                Distance = distance;
                Visited = visited ?? new HashSet<Point>();
            }
        }

        public int ShortestRoute(bool recursive = false)
        {
            var queue = new Queue<State>();
            queue.Enqueue(new State(maze.Entrance));

            var bestDistance = int.MaxValue;

            while (queue.Any())
            {
                var current = queue.Dequeue();

                if (maze.IsExit(current.Point))
                {
                    if (!recursive || current.Level == 0)
                        bestDistance = Math.Min(bestDistance, current.Distance);

                    continue;
                }

                if (current.Distance > bestDistance) continue;

                var visited = new HashSet<Point>(current.Visited) {current.Point};

                foreach (var next in maze.Paths[current.Point])
                    if (maze.IsInnerPortal(next.Destination))
                        queue.Enqueue(new State(
                            maze.OuterPortalNames[maze.InnerPortalPoints[next.Destination]],
                            current.Level + 1,
                            current.Distance + next.Distance + 1,
                            visited));
                    else if (maze.IsOuterPortal(next.Destination))
                    {
                        if (current.Level > 0 || !recursive)
                            queue.Enqueue(new State(
                                maze.InnerPortalNames[maze.OuterPortalPoints[next.Destination]],
                                current.Level - 1,
                                current.Distance + next.Distance + 1,
                                visited));
                    }
                    else if (current.Level == 0 || !recursive)
                        queue.Enqueue(new State(
                            next.Destination,
                            current.Level,
                            current.Distance + next.Distance,
                            visited));
            }

            return bestDistance;
        }
    }
}
