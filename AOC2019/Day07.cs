using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public static class Day07
    {
        private static int getFirstOutput(string program, IEnumerable<int> phases)
        {
            OpComputer ToAmplifier(int phase)
            {
                var amp = new OpComputer(program);
                amp.Input(phase);
                return amp;
            }

            var output = 0;

            phases.ToList().ConvertAll(ToAmplifier).ForEach(amp =>
            {
                amp.Input(output);
                amp.Run();
                output = amp.ReadOutput();
            });
                
            return output;
        }

        public static Tuple<List<int>, int> BestPhaseOutput_SinglePass(string program)
        {
            var phases = new[] {0, 1, 2, 3, 4};

            var highest = new Tuple<List<int>, int>(new List<int>(), 0);
                
            foreach (var perm in phases.Permutations())
            {
                var permutation = perm.ToList();
                var output = getFirstOutput(program, permutation);
                if (output > highest.Item2) highest = new Tuple<List<int>, int>(permutation, output);
            }

            return highest;
        }

        public static int getLoopedOutput(string program, IEnumerable<int> phases)
        {
            OpComputer ToAmplifier(int phase)
            {
                var amp = new OpComputer(program);
                amp.Input(phase);
                return amp;
            }

            var amps = phases.ToList().ConvertAll(ToAmplifier);

            var output = 0;
            while (true)
            {
                foreach (var amp in amps)
                {
                    amp.Input(output);
                    amp.Run();
                    if (amp.HasOutput)
                        output = amp.ReadOutput();
                }

                if (amps[4].Halted)
                    return output;
            }
        }

        public static Tuple<List<int>, int> BestPhaseOutput_Looped(string program)
        {
            var phases = new[] {5, 6, 7, 8, 9};

            var highest = new Tuple<List<int>, int>(new List<int>(), 0);
                
            foreach (var perm in phases.Permutations())
            {
                var permutation = perm.ToList();
                var output = getLoopedOutput(program, permutation);
                if (output > highest.Item2) highest = new Tuple<List<int>, int>(permutation, output);
            }

            return highest;
        }
        
    }
}
