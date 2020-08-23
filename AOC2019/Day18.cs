using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AOC2019
{
    public class Day18
    {
        private readonly Vault vault;

        private class Vault
        {
            private readonly string[] vault;
            private readonly Point[] startPositions;
            private readonly Dictionary<char, Point> keys = new Dictionary<char, Point>();
            private readonly Dictionary<Point, List<KeyRoute>> graph;

            public Vault(IReadOnlyList<string> map, bool split)
            {
                var startPosition = Point.Empty;

                for (var y = 0; y < map.Count; y++)
                for (var x = 0; x < map[0].Length; x++)
                {
                    var p = new Point(x, y);
                    var c = map[y][x];

                    if (isEntrance(c)) startPosition = p;
                    else if (isKey(c)) keys.Add(c, p);
                }

                if (split)
                {
                    vault = map.ToArray();
                    modify(startPosition.X - 1, startPosition.Y - 1, ".#.");
                    modify(startPosition.X - 1, startPosition.Y, "###");
                    modify(startPosition.X - 1, startPosition.Y + 1, ".#.");

                    var offset1 = new Point(1, 1);
                    var offset2 = new Point(1, -1);
                    startPositions = new[]
                    {
                        startPosition.Add(offset1),
                        startPosition.Add(offset2),
                        startPosition.Subtract(offset1),
                        startPosition.Subtract(offset2),
                    };
                }
                else
                {
                    vault = map.ToArray();
                    modify(startPosition.X, startPosition.Y, ".");
                    startPositions = new[] {startPosition};
                }

                graph = buildRouteGraph();
            }

            private void modify(int x, int y, string s) =>
                vault[y] = vault[y].Remove(x, s.Length).Insert(x, s);

            private Dictionary<Point, List<KeyRoute>> buildRouteGraph() =>
                startPositions.Concat(keys.Values).ToDictionary(position => position, routesFrom);

            private static bool isKey(char c) => (uint) (c - 'a') <= 26;
            private static bool isDoor(char c) => (uint) (c - 'A') <= 26;
            private static bool isTunnel(char c) => c == '.';
            private static bool isEntrance(char c) => c == '@' || (uint) (c - '0') <= 10;
            private static uint keyFlag(char c) => (uint) Math.Pow(2, c - 'a');
            private static uint doorFlag(char c) => (uint) Math.Pow(2, c - 'A');

            private List<KeyRoute> routesFrom(Point position)
            {
                var routes = new List<KeyRoute>();
                var visited = new HashSet<Point>();

                var queue = new Queue<State>();
                queue.Enqueue(new State(position));

                while (queue.Any())
                {
                    var q = queue.Dequeue();

                    if (visited.Contains(q.Positions[0])) continue;
                    visited.Add(q.Positions[0]);

                    var c = vault[q.Positions[0].Y][q.Positions[0].X];

                    bool bKey;
                    if (bKey = isKey(c))
                        routes.Add(new KeyRoute(c, q.Distance, q.Keys));

                    if (isDoor(c))
                        foreach (var p in q.Positions[0].Surrounding4())
                            queue.Enqueue(new State(p, q.Distance + 1, q.Keys | doorFlag(c)));

                    else if (isTunnel(c) || bKey)
                        foreach (var p in q.Positions[0].Surrounding4())
                            queue.Enqueue(new State(p, q.Distance + 1, q.Keys));
                }

                return routes;
            }

            public int GetAllKeys()
            {
                var currentMinimum = int.MaxValue;
                var allKeys = (uint) Math.Pow(2, keys.Count) - 1;

                var queue = new Queue<State>();
                queue.Enqueue(new State(startPositions));
                var visited = new Dictionary<(Point[], uint), int>(new CustomComparer());

                while (queue.Any())
                {
                    var q = queue.Dequeue();

                    var tuple = (q.Positions, q.Keys);

                    if (visited.ContainsKey(tuple))
                        if (visited[tuple] > q.Distance)
                            visited[tuple] = q.Distance;
                        else
                            continue;
                    else visited.Add(tuple, q.Distance);

                    if (q.Keys == allKeys)
                    {
                        if (q.Distance < currentMinimum)
                            currentMinimum = q.Distance;
                        continue;
                    }

                    for (var i = 0; i < startPositions.Length; i++)
                    {
                        foreach (var route in graph[q.Positions[i]])
                        {
                            if ((q.Keys & keyFlag(route.Key)) > 0 || (route.Doors & q.Keys) != route.Doors)
                                continue;

                            var newPos = q.Positions.Select(x => x).ToArray();
                            newPos[i] = keys[route.Key];

                            queue.Enqueue(new State(
                                newPos,
                                q.Distance + route.Distance,
                                q.Keys | keyFlag(route.Key)));
                        }
                    }
                }

                return currentMinimum;
            }

            private class CustomComparer : IEqualityComparer<(Point[], uint)>
            {
                public bool Equals((Point[], uint) x, (Point[], uint) y) =>
                    x.Item1.Length == y.Item1.Length
                    && !x.Item1.Where((t, i) => t != y.Item1[i]).Any()
                    && x.Item2.Equals(y.Item2);

                public int GetHashCode((Point[], uint) obj)
                {
                    var (points, u) = obj;
                    var result = u.GetHashCode();
                    foreach (var p in points)
                        unchecked
                        {
                            result = HashCode.Combine(result, p);
                        }

                    return result;
                }
            }

            private class KeyRoute
            {
                public char Key { get; }
                public int Distance { get; }
                public uint Doors { get; }

                public KeyRoute(char key, int distance, uint doors)
                {
                    Key = key;
                    Distance = distance;
                    Doors = doors;
                }
            }

            private readonly struct State
            {
                public Point[] Positions { get; }
                public int Distance { get; }
                public uint Keys { get; }

                public State(Point position, int distance = 0, uint keys = 0) :
                    this(new[] {position}, distance, keys)
                {
                }

                public State(Point[] positions, int distance = 0, uint keys = 0)
                {
                    Positions = positions;
                    Distance = distance;
                    Keys = keys;
                }
            }
        }

        public Day18(IReadOnlyList<string> map, bool split = false) =>
            vault = new Vault(map, split);

        public int GetAllKeys() => vault.GetAllKeys();
    }
}
