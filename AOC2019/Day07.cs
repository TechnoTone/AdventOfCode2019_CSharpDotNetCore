using System;
using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public static class Day07
    {
        private static long getFirstOutput(string program, IEnumerable<long> phases)
        {
            OpComputer ToAmplifier(long phase)
            {
                var amp = new OpComputer(program);
                amp.Input(phase);
                return amp;
            }

            long output = 0;

            phases.ToList().ConvertAll(ToAmplifier).ForEach(amp =>
            {
                amp.Input(output);
                output = amp.Run() switch
                {
                    (OpComputer.Response.Output, long value) => value,
                    _ => output
                };
            });
                
            return output;
        }

        public static Tuple<List<long>, long> BestPhaseOutput_SinglePass(string program)
        {
            var phases = new long[] {0, 1, 2, 3, 4};

            var highest = new Tuple<List<long>, long>(new List<long>(), 0);
                
            foreach (var perm in phases.Permutations())
            {
                var permutation = perm.ToList();
                var output = getFirstOutput(program, permutation);
                if (output > highest.Item2) highest = new Tuple<List<long>, long>(permutation, output);
            }

            return highest;
        }

        public static long getLoopedOutput(string program, IEnumerable<long> phases)
        {
            OpComputer ToAmplifier(long phase)
            {
                var amp = new OpComputer(program);
                amp.Input(phase);
                return amp;
            }

            var amps = phases.ToList().ConvertAll(ToAmplifier);

            long output = 0;
            while (true)
            {
                foreach (var amp in amps)
                {
                    amp.Input(output);
                    switch (amp.Run())
                    {
                        case (OpComputer.Response.Output, long value):
                            output = value;
                            break;
                        case (OpComputer.Response.Halt, _):
                            return output;
                    }
                }
            }
        }

        public static Tuple<List<long>, long> BestPhaseOutput_Looped(string program)
        {
            var phases = new long[] {5, 6, 7, 8, 9};

            var highest = new Tuple<List<long>, long>(new List<long>(), 0);
                
            foreach (var perm in phases.Permutations())
            {
                var permutation = perm.ToList();
                var output = getLoopedOutput(program, permutation);
                if (output > highest.Item2) highest = new Tuple<List<long>, long>(permutation, output);
            }

            return highest;
        }
        
    }
}
