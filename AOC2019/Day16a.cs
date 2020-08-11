using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace AOC2019
{
    public class Day16a
    {
        public int[] Signal { get; private set; } = new int[0];
        public int[][] Patterns { get; } = new int[0][];


        public Day16a(string input)
        {
             Signal = input.ToCharArray().Select(x => (int) x - 48).ToArray();

             var basePattern = new[] {0, 1, 0, -1};

             var range = Enumerable.Range(1, input.Length).ToArray();
             Patterns = range.Select(n => range.Select(m => basePattern[((m / n)) % 4]).ToArray()).ToArray();
        }

        public void Next100Phases()
        {
            for (var i = 0; i < 100; i++) NextPhase();
        }

        public void NextPhase()
        {
            var range = Enumerable.Range(0, Signal.Length).ToArray();

            int next(int ix) => Abs(range.Select(i => Signal[i] * Patterns[ix][i]).Sum() % 10);

            Signal = range.Select(next).ToArray();
        }
    }

    public class Day16b
    {
        public int[] ReversedSignal { get; private set; } = new int[0];

        public int Result => int.Parse(ReversedSignal.TakeLast(8).Reverse().JoinToStringNoSeparator());

        public Day16b(string input)
        {
            var inputDigits = input.ToCharArray().Select(x => (int) x - 48).ToArray();
            var offset = long.Parse(input.Substring(0, 7));
            var length = input.Length;

            var initialState = new List<int>();
            for (var i = offset; i < length * 10000; i++)
                initialState.Insert(0,inputDigits[(int) (i % length)]);

            ReversedSignal = initialState.ToArray();
        }

        public void Next100Phases()
        {
            for (var i = 0; i < 100; i++) NextPhase();
        }

        private void NextPhase()
        {
            var nextSignal = new List<int>();
            var total = 0;

            foreach (var n in ReversedSignal)
            {
                total += n;
                nextSignal.Add(total % 10);
            }

            ReversedSignal = nextSignal.ToArray();
        }
    }

}
