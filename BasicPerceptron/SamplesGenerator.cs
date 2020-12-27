using System;
using System.Collections.Generic;
using System.IO;

namespace BasicPerceptron
{
    public class SamplesGenerator
    {
        #region Members

        private readonly int _minimum;
        private readonly int _maximum;
        private readonly Random _random;

        #endregion Members

        #region Constructor

        public SamplesGenerator(int bitCount, int minimum = 0)
        {
            _minimum = minimum;
            _maximum = (int) Math.Pow(2, bitCount);
            _random = new Random();
        }

        #endregion Constructor

        #region Public Methods

        public void GenerateSet(int count, string fileName)
        {
            var trainList = GenerateRandomList(count);
            WriteToFile(trainList, fileName);
        }

        #endregion Public Methods

        #region Private Methods

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
                var mostlyOnes = Utils.MostlyOnes(randomNumber);
                yield return (randomNumber, mostlyOnes);
            }
        }

        #endregion Private Methods
    }
}