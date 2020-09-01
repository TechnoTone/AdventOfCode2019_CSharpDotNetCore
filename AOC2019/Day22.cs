using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AOC2019
{
    public static class Day22
    {
        private static int ShuffleCard(int totalCards, in int n, string instruction)
        {
            if (instruction == "deal into new stack")
                return totalCards - 1 - n;

            var x = int.Parse(instruction.Split(" ").Last());

            if (instruction.StartsWith("deal with increment"))
                return (n * x) % totalCards;

            if (instruction.StartsWith("cut"))
                return x > 0 ? (n - x + totalCards) % totalCards : (n - x) % totalCards;

            return -1;
        }

        public static int ShuffleCard(int totalCards, int cardToTrack, IEnumerable<string> instructions) =>
            instructions.Aggregate(cardToTrack,
                (current, instruction) => ShuffleCard(totalCards, current, instruction));

        private class Deck
        {
            public BigInteger Offset = BigInteger.Zero;
            public BigInteger Increment = BigInteger.One;
        }

        public static BigInteger InverseShuffleCard(IEnumerable<string> instructions)
        {
            BigInteger totalCards = 119315717514047;
            BigInteger iterations = 101741582076661;
            BigInteger cardToTrack = 2020;

            var deck = new Deck();

            foreach (var instruction in instructions)
                processInstruction(deck, totalCards, instruction);

            processIterations(deck, totalCards, iterations);

            return get(deck, totalCards, cardToTrack);
        }

        private static void processInstruction(
            Deck deck,
            BigInteger totalCards,
            string instruction)
        {
            if (instruction == "deal into new stack")
            {
                deck.Increment = (-deck.Increment).mod(totalCards);
                deck.Offset = (deck.Offset + deck.Increment).mod(totalCards);
                return;
            }

            var x = BigInteger.Parse(instruction.Split(" ").Last());

            if (instruction.StartsWith("cut"))
                deck.Offset =
                    (deck.Offset + x * deck.Increment).mod(totalCards);

            if (instruction.StartsWith("deal with increment"))
                deck.Increment =
                    (deck.Increment * x.inv(totalCards)).mod(totalCards);
        }

        private static void processIterations(Deck deck, BigInteger totalCards, BigInteger iterations)
        {
            deck.Offset = (
                deck.Offset
                * (1 - BigInteger.ModPow(deck.Increment, iterations, totalCards))
                * ((1 - deck.Increment) % totalCards).inv(totalCards)
            ).mod(totalCards);
            deck.Increment = BigInteger.ModPow(deck.Increment, iterations, totalCards);
        }

        private static BigInteger get(Deck deck, BigInteger totalCards, BigInteger i) =>
            (deck.Offset + i * deck.Increment) % totalCards;

        private static BigInteger mod(this BigInteger x, BigInteger m) =>
            (x + m) % m;

        private static BigInteger inv(this BigInteger x, BigInteger totalCards) =>
            BigInteger.ModPow(x, totalCards - 2, totalCards);
    }
}
