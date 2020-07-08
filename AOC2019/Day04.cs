using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public static class Day04
    {
        public static bool isValid1(int password, int min, int max)
        {
            var s = password.ToString();

            if (s.Length != 6) return false;
            if (password < min) return false;
            if (s.Distinct().Count() == 6) return false;
            if (string.Concat(s.OrderBy(c => c)) != s) return false;
            if (password > max) return false;

            return true;
        }

        public static bool isValid2(int password, int min, int max)
        {
            if (!isValid1(password, min, max)) return false;

            return
                password
                    .ToString()
                    .ToCharArray()
                    .GroupBy(c => c)
                    .Any(g => g.Count() == 2);
        }

        public static int validInRange1(int min, int max) => 
            ValidInRange(min, max, isValid1);

        public static int validInRange2(int min, int max) => 
            ValidInRange(min, max, isValid2);

        private static int ValidInRange(int min, int max, Func<int, int, int, bool> fn)
        {
            var count = 0;

            for (var i = min; i < max; i++)
                if (fn(i, min, max))
                    count++;

            return count;
        }
    }
}
