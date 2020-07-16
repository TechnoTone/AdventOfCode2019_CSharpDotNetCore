using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public static class StringExtensions
    {
        public static List<int> ParseCommaSeparatedIntegers(this string input) => 
            input.Split(',').ToList().ConvertAll(int.Parse);
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

        public static IEnumerable<string> Chunk(this string input, int chunkSize) =>
            Enumerable.Range(0, input.Length / chunkSize)
                .Select(i => input.Substring(i * chunkSize, chunkSize));

    }
}
