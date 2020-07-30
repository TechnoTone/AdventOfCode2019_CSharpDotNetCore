using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AOC2019
{

    public static class LongExtensions
    {
        public static bool IsDivisibleBy(this long i, long n) => i % n == 0;
    }
    public static class StringExtensions
    {
        public static List<long> ParseCommaSeparatedIntegers(this string input) =>
            input.Split(',').ToList().ConvertAll(long.Parse);
    }

    public static class PointExtensions
    {
        public static int ManhattanDistance(this Point p) => Math.Abs(p.X) + Math.Abs(p.Y);

        public static void Move(this ref Point p, WireDirection direction)
        {
            switch (direction)
            {
                case WireDirection.Up:
                    p.X--;
                    break;
                case WireDirection.Down:
                    p.X++;
                    break;
                case WireDirection.Left:
                    p.Y--;
                    break;
                case WireDirection.Right:
                    p.Y++;
                    break;
            }
        }
        public static void Move(this ref Point p, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    p.X--;
                    break;
                case Direction.Down:
                    p.X++;
                    break;
                case Direction.Left:
                    p.Y--;
                    break;
                case Direction.Right:
                    p.Y++;
                    break;
            }
        }

        public static float Angle(this Point p) =>
            (float) (Math.Atan2(p.X, -p.Y) * (float) (180 / Math.PI) + 360) % 360;

        public static Point Subtract(this Point p, Point offset) =>
            new Point(p.X - offset.X, p.Y - offset.Y);

        public static Point Add(this Point p, Point offset) =>
            new Point(p.X + offset.X, p.Y + offset.Y);

        public static Point Reduce(this Point p)
        {
            var gcd = GCD(p.X, p.Y);
            return new Point(p.X / gcd, p.Y / gcd);
        }

        private static int GCD(int a, int b)
        {
            if (a == 0 && b == 0) return 1;

            a = Math.Abs(a);
            b = Math.Abs(b);

            while (a != 0 && b != 0)
                if (a > b) a %= b;
                else b %= a;

            return a == 0 ? b : a;
        }

        public static string ToSimpleString(this Point p) => $"{p.X},{p.Y}";
    }

    public static class EnumerableExtensions
    {
        private static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new[] {t});

            return Permutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] {t2}));
        }
        
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> sequence) => 
            sequence.Permutations(sequence.Count());

        public static string JoinToString (this IEnumerable<int> sequence) =>
            string.Join(',', sequence);

        public static string JoinToString (this IEnumerable<long> sequence) =>
            string.Join(',', sequence);

        public static IEnumerable<string> Chunk(this string input, int chunkSize) =>
            Enumerable.Range(0, input.Length / chunkSize)
                .Select(i => input.Substring(i * chunkSize, chunkSize));

        public static IEnumerable<T> Except<T>(this IEnumerable<T> sequence, T item) =>
            sequence.Where(el => !el.Equals(item));
    }

    public static class ListExtensions
    {
        public static bool SameContentAs(this List<int> sequence, List<int> other)
        {
            if (sequence.Count != other.Count) return false;
            return !sequence.Where((t, i) => t != other[i]).Any();
        }

        public static bool SameContentAs(this List<long> sequence, List<long> other)
        {
            if (sequence.Count != other.Count) return false;
            return !sequence.Where((t, i) => t != other[i]).Any();
        }
    }
}
