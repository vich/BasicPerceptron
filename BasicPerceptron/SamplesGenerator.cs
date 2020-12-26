using System;
using System.Collections.Generic;
using System.IO;

namespace BasicPerceptron
{
    public class SamplesGenerator
    {
        private readonly int _bitsCount;
        private readonly int _minimum;
        private readonly int _maximum;
        private readonly Random _random ;

        public SamplesGenerator(int bitCount, int minimum = 0)
        {
            _bitsCount = bitCount;
            _minimum = minimum;
            _maximum = (int) Math.Pow(2, _bitsCount);
            _random = new Random();
        }
        
        public void GenerateSet(int count, string fileName)
        {
            var trainList = GenerateRandomList(count);
            WriteToFile(trainList, fileName);
        }

        private static void WriteToFile(IEnumerable<(int, bool)> data, string trainFile)
        {
            using (var streamWriter = new StreamWriter(trainFile))
            {
                foreach (var (number, mostlyOnes) in data)
                {
                    byte mostlyOnesByte = mostlyOnes ? 1 : 0;
                    var binFormat = Convert.ToString(number, 2);
                    streamWriter.WriteLine($"{binFormat.PadLeft(15, '0')} {mostlyOnesByte}");
                }
            }
        }

        private IEnumerable<(int, bool)> GenerateRandomList(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var randomNumber = _random.Next(_minimum, _maximum);
                var mostlyOnes = MostlyOnes(randomNumber);
                yield return (randomNumber, mostlyOnes);
            }
        }

        private bool MostlyOnes(int num)
        {
            var ones = CountSetBits(num);
            return ones > (_bitsCount + 1) / 2;
        }

        private static int CountSetBits(int n)
        {
            var count = 0;
            while (n > 0)
            {
                count += n & 1;
                n >>= 1;
            }

            return count;
        }
    }
}