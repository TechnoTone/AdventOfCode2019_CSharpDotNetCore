using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AOC2019
{
    public class Day10
    {
        public List<Point> Asteroids { get; }

        public Day10(IEnumerable<string> map) => Asteroids = parseMap(map.ToList());

        private static List<Point> parseMap(IReadOnlyList<string> map)
        {
            var asteroids = new List<Point>();

            for (var y = 0; y < map.Count; y++)
            for (var x = 0; x < map[0].Length; x++)
                if (map[y][x] == '#')
                    asteroids.Add(new Point(x, y));

            return asteroids;
        }

        public int visibleFrom(Point origin)
        {
            Point Selector(Point c) => c.Subtract(origin).Reduce();

            return Asteroids
                .Except(origin)
                .Select(Selector)
                .GroupBy(c => c)
                .Count();
        }

        public Point BestLocation() =>
            Asteroids.OrderBy(visibleFrom).Last();

        public int BestCount() =>
            Asteroids.Select(visibleFrom).Max();

        public List<Point> Vaporised(Point origin)
        {
            var targets = Asteroids
                .Except(origin)
                .Select(a => a.Subtract(origin))
                .OrderBy(a => a.Angle())
                .ThenBy(a => a.ManhattanDistance())
                .ToList();

            var vaporised = new List<Point>();

            while (targets.Count > 0)
            {
                var next = targets.FirstOrDefault();
                while (!next.Equals(Point.Empty))
                {
                    vaporised.Add(next.Add(origin));
                    targets.Remove(next);
                    next = targets.FirstOrDefault(t => t.Angle() > next.Angle());
                }
            }

            return vaporised;
        }
    }
}
