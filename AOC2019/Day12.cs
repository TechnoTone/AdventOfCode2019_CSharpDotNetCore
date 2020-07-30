using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public class Day12
    {
        public List<Moon> Moons { get; }

        public class Moon
        {
            public Vector3 Position { get; internal set; }
            public Vector3 Velocity { get; internal set; }

            public int Energy => Position.AbsSum * Velocity.AbsSum;

            public Moon(int x, int y, int z)
            {
                Position = new Vector3(x, y, z);
                Velocity = Vector3.Zero;
            }

            public Moon(int x, int y, int z, int vx, int vy, int vz)
            {
                Position = new Vector3(x, y, z);
                Velocity = new Vector3(vx, vy, vz);
            }

            public override string ToString() => $"{Position} / {Velocity}";
        }

        public Day12(IEnumerable<string> input) =>
            Moons = input.Select(parse).ToList();

        private static Moon parse(string input)
        {
            var values = input.TrimEnd('>').Split(',').Select(i => int.Parse(i.Split('=')[1])).ToArray();
            return new Moon(values[0], values[1], values[2]);
        }

        public void TakeStep()
        {
            Moons.ForEach(applyGravity);
            Moons.ForEach(applyVelocity);
        }

        public void TakeSteps(int n)
        {
            for (var i = 0; i < n; i++) TakeStep();
        }

        public void TakeTenSteps() => TakeSteps(10);

        private void applyGravity(Moon moon)
        {
            var position = moon.Position;

            Vector3 CompareTo(Vector3 pos) =>
                new Vector3(
                    position.X.CompareTo(pos.X),
                    position.Y.CompareTo(pos.Y),
                    position.Z.CompareTo(pos.Z));

            Moons.ForEach(m => m.Velocity += CompareTo(m.Position));
        }

        private void applyVelocity(Moon moon) =>
            moon.Position += moon.Velocity;

        public int TotalEnergy => Moons.Select(m => m.Energy).Sum();

        public long findRepetition()
        {
            var stepPatterns = new[] {new StepPattern(), new StepPattern(), new StepPattern()};

            var startPositions = Moons.Select(m => m.Position);
            var startXs = startPositions.Select(p => p.X).ToList();
            var startYs = startPositions.Select(p => p.Y).ToList();
            var startZs = startPositions.Select(p => p.Z).ToList();

            var n = 0;

            while (true)
            {
                TakeStep();
                n++;

                var currentPositions = Moons.Select(m => m.Position);

                if (startXs.SameContentAs(currentPositions.Select(p=>p.X).ToList()))
                    stepPatterns[0].NextPosition(n);
                if (startYs.SameContentAs(currentPositions.Select(p => p.Y).ToList()))
                    stepPatterns[1].NextPosition(n);
                if (startZs.SameContentAs(currentPositions.Select(p => p.Z).ToList()))
                    stepPatterns[2].NextPosition(n);

                if (stepPatterns.All(sp => sp.Complete))
                {
                    var divisions = getDivisions(stepPatterns.Select(sp => sp.cycleLength).ToList());
                    return divisions.Aggregate(1L, (a, b) => a * b);
                }

                if (n >= 9999999) return -1;
            }

        }

        private static IEnumerable<long> getDivisions(List<long> lst)
        {
            var n = 2;
            var divisions = new List<long>();
            while (lst.Any(i => i > n))
            {
                var divided = false;
                for (var i = 0; i < 3; i++)
                {
                    if (!lst[i].IsDivisibleBy(n)) continue;
                    lst[i] = lst[i] / n;
                    divided = true;
                }

                if (divided)
                    divisions.Add(n);
                else
                    n += 1;
            }

            lst.ForEach(divisions.Add);

            return divisions;
        }

        private class StepPattern
        {
            private const int SEARCH_LENGTH = 10;

            private readonly List<long> startSteps = new List<long>();
            private readonly List<long> endSteps = new List<long>();

            private long lastValue;
            public bool Complete { get; private set; }

            public long cycleLength => lastValue - endSteps.Sum();

            public void NextPosition(long value)
            {
                if (Complete) return;

                var stepSize = value - lastValue;
                lastValue = value;

                if (startSteps.Count < SEARCH_LENGTH)
                    startSteps.Add(stepSize);

                if (startSteps.Count == 1) return;

                if (startSteps[endSteps.Count] == stepSize)
                {
                    endSteps.Add(stepSize);
                    if (endSteps.Count < SEARCH_LENGTH) return;
                    if (startSteps.SameContentAs(endSteps))
                        Complete = true;
                    else
                        endSteps.RemoveAt(0);
                }
                else
                    endSteps.Clear();
            }
        }
    }
}
