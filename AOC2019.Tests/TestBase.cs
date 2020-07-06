using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AOC2019.Tests
{
    public class TestBase
    {
        internal readonly InputReader input;

        internal class InputReader
        {
            private const string INPUT_PATH = "../../../../input";
            private readonly int day;

            public InputReader(int day)
            {
                this.day = day;
            }

            private IEnumerable<string> asLines() =>
                File.ReadAllLines($"{INPUT_PATH}/day{day:00}.txt");

            internal List<string> asList() => 
                asLines().ToList();

            internal List<int> asListOfInts() =>
                asList().ConvertAll(int.Parse);
        }

        protected TestBase(int day)
        {
            input = new InputReader(day);
        }
    }
}
