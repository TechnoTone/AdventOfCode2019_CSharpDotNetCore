using System.Collections.Generic;
using System.Linq;

namespace AOC2019
{
    public static class Day08
    {
        public static int Checksum(string imageData, int size)
        {
            int countDigits(string s, string ch) =>
                size - s.Replace(ch, "").Length;

            var minZeroes = size;
            var countOnes = 0;
            var countTwos = 0;

            for (var pos = 0; pos < imageData.Length; pos += size)
            {
                var s = imageData.Substring(pos, size);
                var nZeros = countDigits(s, "0");

                if (nZeros >= minZeroes) continue;
                minZeroes = nZeros;
                countOnes = countDigits(s, "1");
                countTwos = countDigits(s, "2");
            }

            return countOnes * countTwos;
        }

        public static IEnumerable<string> ImageLayers(string imageData, int size) =>
            imageData.Chunk(size);

        public static string RenderImage(string imageData, int size)
        {
            var result = "";
            var layers = ImageLayers(imageData, size);

            char getPixel(int i)
            {
                foreach (var layer in layers) if (layer[i] != '2') return layer[i];
                return '2';
            }

            for (var i = 0; i < size; i++) result += getPixel(i);

            return result;
        }
    }
}
