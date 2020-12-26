using System;
using System.Collections.Generic;
using System.IO;

namespace BasicPerceptron
{
    public static class SamplesGenerator
    {
        private const string TrainFile = "Train.txt";
        private const string TestFile = "Test.txt";
        private const int BitsCount = 21;
        private const int Minimum = 0;
        private static readonly int Maximum = (int) Math.Pow(2, BitsCount);

        private static readonly Random Random = new Random();

        public static void Generate(int trainCount, int testCount)
        {
            var trainList = GenerateRandomList(trainCount);
            var testList = GenerateRandomList(testCount);

            WriteToFile(trainList, TrainFile);
            WriteToFile(testList, TestFile);
        }

        private static void WriteToFile(IEnumerable<(int, bool)> data, string trainFile)
        {
            using (var streamWriter = new StreamWriter(trainFile))
            {
                foreach (var (number, mostlyOnes) in data)
                {
                    var binFormat = Convert.ToString(number, 2);
                    streamWriter.WriteLine($"{binFormat.PadLeft(15, '0')} {mostlyOnes}");
                }
            }
        }

        private static IEnumerable<(int, bool)> GenerateRandomList(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var randomNumber = Random.Next(Minimum, Maximum);
                var mostlyOnes = MostlyOnes(randomNumber);
                yield return (randomNumber, mostlyOnes);
            }
        }

        private static bool MostlyOnes(int num)
        {
            var ones = CountSetBits(num);
            return ones > (BitsCount + 1) / 2;
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