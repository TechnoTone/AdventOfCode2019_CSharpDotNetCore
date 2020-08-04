using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public class Day14
    {
        private const long ONE_TRILLION = 1000000000000;
        private readonly Reactor reactor;

        private class Reactor
        {
            private readonly Dictionary<string, (long, List<ChemicalQuantity>)> reactions;

            public Reactor(Dictionary<string, (long, List<ChemicalQuantity>)> reactions) =>
                this.reactions = reactions;

            public long OreToMakeFuel(long desiredFuel)
            {
                var depths = reactions.Keys.ToDictionary(chemical => chemical, depth);
                depths.Add("ORE", 0);

                var store = reactions.Values
                    .SelectMany(x => x.Item2)
                    .Select(cq => cq.Chemical)
                    .Distinct()
                    .ToDictionary(chemical => chemical, _ => 0L);
                store.Add("FUEL", 0);

                var queue = new Dictionary<string, long> {{"FUEL", desiredFuel}};

                while (queue.Keys.Any(chemical => chemical != "ORE"))
                {
                    var chemical = queue.Keys.OrderByDescending(key => depths[key]).First();
                    while (store[chemical] < queue[chemical])
                    {
                        var (quantity, ingredients) = reactions[chemical];
                        var batchSize = (queue[chemical] - store[chemical] + quantity - 1) / quantity;
                        store[chemical] += quantity * batchSize;
                        foreach (var cq in ingredients)
                        {
                            if (queue.ContainsKey(cq.Chemical))
                                queue[cq.Chemical] += cq.Quantity * batchSize;
                            else
                                queue.Add(cq.Chemical, cq.Quantity * batchSize);
                        }
                    }

                    store[chemical] -= queue[chemical];
                    queue.Remove(chemical);
                }

                return queue.Values.Sum();
            }

            private long depth(string chemical) =>
                chemical == "ORE" ? 0 : 1 + reactions[chemical].Item2.Max(x => depth(x.Chemical));
        }

        public Day14(IEnumerable<string> inputs) => reactor = new Reactor(Parse(inputs));

        public static Dictionary<string, (long, List<ChemicalQuantity>)> Parse(IEnumerable<string> input) =>
            input.Select(parseReaction)
                .ToDictionary(
                    reaction => reaction.ChemicalOutput.Chemical,
                    reaction => (reaction.ChemicalOutput.Quantity, reaction.ChemicalInputs));

        private static Reaction parseReaction(string input) => new Reaction(input);

        private class Reaction
        {
            public List<ChemicalQuantity> ChemicalInputs { get; }
            public ChemicalQuantity ChemicalOutput { get; }

            public Reaction(string input)
            {
                static (string inputChemicals, string outputChemical) splitReaction(string input)
                {
                    var split = input.Split(" => ");
                    return (split[0], split[1]);
                }

                var (inputs, output) = splitReaction(input);

                ChemicalInputs = inputs.Split(", ").Select(parseChemical).ToList();

                ChemicalOutput = parseChemical(output);
            }

            public override string ToString() => $"{string.Join(", ", ChemicalInputs)} => {ChemicalOutput}";
        }

        private static ChemicalQuantity parseChemical(string input) => new ChemicalQuantity(input);

        public class ChemicalQuantity
        {
            public long Quantity { get; }
            public string Chemical { get; }

            public ChemicalQuantity(string input)
            {
                static (long quantity, string chemical) splitChemical(string input)
                {
                    var split = input.Split(" ");
                    return (long.Parse(split[0]), split[1]);
                }

                (Quantity, Chemical) = splitChemical(input);
            }

            public override string ToString() => $"{Quantity} {Chemical}";
        }

        public long OreToMakeFuel() => reactor.OreToMakeFuel(1);

        public long FuelFrom1TrillionOre()
        {
            var costPerUnitOfFuel = reactor.OreToMakeFuel(1);

            var closestFuel = 1L;
            var closestOre = costPerUnitOfFuel;

            while (closestOre + costPerUnitOfFuel < ONE_TRILLION)
            {
                HelperFunctions.Log(closestFuel.ToString());
                closestFuel = (long) (closestFuel * ((double) ONE_TRILLION / closestOre));
                closestOre = reactor.OreToMakeFuel(closestFuel);
            }

            return closestFuel;
        }
    }
}
