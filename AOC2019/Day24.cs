using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public static class Day24
    {
        public class SimpleGrid
        {
            private string[] grid;
            private readonly int width;
            private readonly int height;

            public SimpleGrid(string[] gridData)
            {
                grid = gridData;
                width = grid[0].Length;
                height = grid.Length;
            }

            public void Forward(long minutes) => minutes.Repeat(Forward);

            private void Forward()
            {
                bool isBug(int x, int y) =>
                    x >= 0 && y >= 0 && x < width && y < height && grid[y][x] == '#';

                int adjacentBugs(int x, int y) =>
                    (isBug(x - 1, y) ? 1 : 0) +
                    (isBug(x + 1, y) ? 1 : 0) +
                    (isBug(x, y - 1) ? 1 : 0) +
                    (isBug(x, y + 1) ? 1 : 0);

                bool aliveNextIteration(int x, int y) => adjacentBugs(x, y) switch
                {
                    1 => true,
                    2 => !isBug(x, y),
                    _ => false
                };

                grid = grid.Select((row, y) =>
                    string.Join("", row.Select((cell, x) =>
                        aliveNextIteration(x, y) ? "#" : "."))).ToArray();
            }


            public string Read() => string.Join('\n', grid);

            public void ForwardUntilRepeat()
            {
                var history = new HashSet<string>();

                while (!history.Contains(Read()))
                {
                    history.Add(Read());
                    Forward();
                }
            }

            public long BiodiversityRating() =>
                string.Join("", grid)
                    .Select((c, i) => c == '#' ? (long) Math.Pow(2, i) : 0)
                    .Sum();
        }

        public class RecursiveGrid
        {
            private string[] grid;

            private HashSet<Vector3> _bugs;
            private HashSet<Vector3> bugs
            {
                get => _bugs;
                set
                {
                    _bugs = value;
                    (bottom, top) = bugs.Select(v => v.Z).Range();
                }
            }

            private int bottom = 0;
            private int top = 0;

            public RecursiveGrid(string[] gridData)
            {
                grid = gridData;

                bugs = grid.SelectMany((row, y) =>
                        row.Select((c, x) => (c, x))
                            .Where(cell => cell.c == '#')
                            .Select(cell => new Vector3(cell.x, y, 0)))
                    .ToHashSet();
            }

            public void Forward(long minutes) => minutes.Repeat(Forward);

            private static IEnumerable<int> r5() => Enumerable.Range(0, 5);

            private void Forward()
            {
                int adjacentBugs(Vector3 v)
                {
                    if (v.X == 2 && v.Y == 2) return 0;

                    var adjustment = 0;

                    if (v.Y == 0 && bugs.Contains(new Vector3(2, 1, v.Z - 1))) adjustment++;
                    if (v.X == 0 && bugs.Contains(new Vector3(1, 2, v.Z - 1))) adjustment++;
                    if (v.X == 4 && bugs.Contains(new Vector3(3, 2, v.Z - 1))) adjustment++;
                    if (v.Y == 4 && bugs.Contains(new Vector3(2, 3, v.Z - 1))) adjustment++;

                    if (v.X == 2 && v.Y == 1) adjustment += bugs.Count(b => b.Y == 0 && b.Z == v.Z + 1);
                    if (v.X == 1 && v.Y == 2) adjustment += bugs.Count(b => b.X == 0 && b.Z == v.Z + 1);
                    if (v.X == 3 && v.Y == 2) adjustment += bugs.Count(b => b.X == 4 && b.Z == v.Z + 1);
                    if (v.X == 2 && v.Y == 3) adjustment += bugs.Count(b => b.Y == 4 && b.Z == v.Z + 1);

                    return adjustment +
                           (bugs.Contains(v.Add(0, -1, 0)) ? 1 : 0) +
                           (bugs.Contains(v.Add(-1, 0, 0)) ? 1 : 0) +
                           (bugs.Contains(v.Add(1, 0, 0)) ? 1 : 0) +
                           (bugs.Contains(v.Add(0, 1, 0)) ? 1 : 0);
                }

                bool bugNextIteration(Vector3 v) => adjacentBugs(v) switch
                {
                    1 => true,
                    2 => !bugs.Contains(v),
                    _ => false
                };

                IEnumerable<Vector3> levelCells(int level) =>
                    r5().SelectMany(y => r5().Select(x => new Vector3(x, y, level)));

                IEnumerable<Vector3> nextBugsOnLevel(int level) =>
                    levelCells(level).Where(bugNextIteration);

                bugs = Enumerable
                    .Range(bottom - 1, top - bottom + 3)
                    .SelectMany(nextBugsOnLevel)
                    .ToHashSet();
            }

            public (int, int) Levels() => (bottom, top);

            public int BugCount() => bugs.Count;

            public IEnumerable<string> Read(int level)
            {
                return r5()
                    .Select(y => r5().Select(x => bugs.Contains(new Vector3(x, y, level)) ? '#' : '.'))
                    .Select(row => string.Join("", row));
            }
        }
    }
}
