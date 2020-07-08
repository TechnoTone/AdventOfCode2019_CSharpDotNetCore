using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace AOC2019
{
    public static class Day03
    {
        public static int distance(string wire1, string wire2)
        {
            var w1 = parseWire(wire1);
            var w2 = parseWire(wire2);
            var intersections =
                w1.Intersect(w2).ToList();

            return intersections.ConvertAll(p => p.distance).Min();
        }

        public static int steps(string wire1, string wire2)
        {
            var w1 = parseWire(wire1);
            var w2 = parseWire(wire2);
            var intersections =
                w1.Intersect(w2).ToList();

            return intersections
                .ConvertAll(p => w1.IndexOf(p) + w2.IndexOf(p) + 2)
                .Min();
        }

        private static List<Coordinate> parseWire(string wire1)
        {
            var coordinates = new List<Coordinate>();
            var coordinate = new Coordinate();
            
            foreach (var s in wire1.Split(','))
            {
                var segment = WireSegment.parse(s);
                while (segment.distance > 0)
                {
                    coordinate.Move(segment.direction);
                    coordinates.Add(coordinate);
                    segment.distance--;
                }
            }

            return coordinates;
        }

    }

    internal struct WireSegment
    {
        public readonly Direction direction;
        public int distance;

        private WireSegment(char direction, int distance)
        {
            this.direction = (Direction) direction;
            this.distance = distance;
        }
        
        public static WireSegment parse(string s) =>
            new WireSegment(s[0], int.Parse(s.Substring(1)));

    }
}
