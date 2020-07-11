using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public static class Day06
    {
        public static int OrbitCounts(List<string> input)
        {
            var orbits = Orbits(input);

            var total = 0;
            var iteration = 0;
            var children = orbits["COM"];
            while (children.Count > 0)
            {
                iteration++;
                total += (children.Count * iteration);
                var next = new List<string>();
                children.ForEach(c =>
                {
                    if (orbits.ContainsKey(c))
                        next.AddRange(orbits[c]);
                });
                children = next;
            }

            return total;
        }

        public static int Transfers(List<string> input)
        {
            var orbits = input.ConvertAll(toOrbit);
            var youParents = getParents("YOU", orbits);
            var sanParents = getParents("SAN", orbits);

            while (youParents[0] == sanParents[0])
            {
                youParents.RemoveAt(0);
                sanParents.RemoveAt(0);
            }

            return youParents.Count + sanParents.Count;
        }

        private static List<string> getParents(string s, List<Orbit> orbits)
        {
            var results = new List<string>();
            while (orbits.Exists(o => o.Child == s))
            {
                var x = orbits.Find(o => o.Child == s);
                s = x.Parent;
                results.Add(s);
            }

            results.Reverse();
            return results;
        }

        private static Dictionary<string, List<string>> Orbits(List<string> input)
        {
            var orbits = input.ConvertAll(toOrbit);

            var result = new Dictionary<string, List<string>>();
            orbits.ForEach(o =>
            {
                if (result.ContainsKey(o.Parent))
                    result[o.Parent].Add(o.Child);
                else
                    result.Add(o.Parent, new List<string> {o.Child});

            });

            return result;
        }

        private static Orbit toOrbit(string input) => new Orbit(input);

        private readonly struct Orbit
        {
            public readonly string Parent;
            public readonly string Child;
            
            public Orbit(string input)
            {
                var ss = input.Split(')');
                Parent = ss[0];
                Child = ss[1];
            }

            public override string ToString() => $"{Parent}){Child}";
        }
    }
}
