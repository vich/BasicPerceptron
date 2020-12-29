using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BasicPerceptron
{
    class Program
    {

        #region Members

        private const string TrainFile = "Train.txt";
        private const string TestFile = "Test.txt";
        private const int Neurons = 21;

        private static int _trainSamples = 100;
        private static int _testSamples = 1000000;
        private static double _learningRate = 0.1;
        private static int _bias = 1;
        private static bool _overrideSamples = true;

        #endregion Members

        static void Main(string[] args)
        {
            ReadParamsFromUser();

            GenerateSamples();
            var trainedWeights = TrainPerceptron();
            TestPerceptron(trainedWeights);

            Console.ReadLine();
        }

        private static void ReadParamsFromUser()
        {
            try
            {
                Console.WriteLine(
                    $"Init Values: {nameof(_trainSamples)}={_trainSamples},{nameof(_testSamples)}={_testSamples}, " +
                    $"{nameof(_learningRate)}={_learningRate}, {nameof(_bias)}={_bias}, " +
                    $"{nameof(_overrideSamples)}={_overrideSamples}");

                Console.WriteLine($"Use custom parameters (0=false, otherwise true)");
                var customParams = int.Parse(Console.ReadLine());

                if (customParams != 0)
                {
                    Console.WriteLine($"Enter number of train samples (int)");
                    _trainSamples = int.Parse(Console.ReadLine());
                    Console.WriteLine($"Enter number of test samples (int)");
                    _testSamples = int.Parse(Console.ReadLine());
                    Console.WriteLine($"Enter learning rate (double)");
                    _learningRate = double.Parse(Console.ReadLine());
                    Console.WriteLine($"Use Bias (0=false, otherwise true)");
                    _bias = int.Parse(Console.ReadLine()) == 0 ? 0 : 1;
                    Console.WriteLine($"Override samples (0=false, otherwise true)");
                    _overrideSamples = int.Parse(Console.ReadLine()) == 0;
                }
            }
            catch
            {
                Console.WriteLine(
                    $"Init Values: {nameof(_trainSamples)}={_trainSamples},{nameof(_testSamples)}={_testSamples}, " +
                    $"{nameof(_learningRate)}={_learningRate}, {nameof(_bias)}={_bias}, " +
                    $"{nameof(_overrideSamples)}={_overrideSamples}");
            }
        }

        private static void TestPerceptron(double[] weights)
        {
            var input = ReadData(TestFile, _testSamples, out var outputs);

            var countWrongZeros = 0;
            var countWrongOnes = 0;
            var countExpectedZeros = 0;
            var countExpectedOnes = 0;

            foreach (var output in outputs)
            {
                if (output == 1)
                    countExpectedOnes++;
                else
                    countExpectedZeros++;
            }

            for (var i = 0; i < _testSamples; i++)
            {
                var actual = CalculateOutput(input.GetRow(i), weights);
                var expected = outputs[i];

                if (expected != actual)
                {
                    if (expected == 1)
                        countWrongOnes++;
                    else
                        countWrongZeros++;
                }
            }

            Console.WriteLine($"Results: total mistake={countWrongZeros+countWrongOnes}-{(countWrongZeros+ countWrongOnes) / (double)outputs.Length}%," +
                              $" wrong zeros number={countWrongZeros}-{countWrongZeros / (double)countExpectedZeros}%," +
                              $" wrong ones number={countWrongOnes}-{countWrongOnes / (double)countExpectedOnes}%");
        }

        private static double[] TrainPerceptron()
        {
            var input = ReadData(TrainFile, _trainSamples, out var outputs);
            var weights = RandomWeights(Neurons + 1);

            double totalError = 1;
            var iteration = 1;
            while (totalError > 0.2)
            {
                if (iteration > 100 * _trainSamples)
                    break;

                totalError = 0;
                for (var i = 0; i < _trainSamples; i++)
                {
                    var iRow = input.GetRow(i);
                    var output = CalculateOutput(iRow, weights);
                    var error = outputs[i] - output;

                    for (var j = 0; j < Neurons; j++)
                    {
                        weights[j] += _learningRate * error * input[i, j];
                    }

                    weights[^1] += _learningRate * error * _bias;

                    totalError += Math.Abs(error);
                }

                Console.WriteLine($"iteration={iteration}, totalError={totalError}");
                iteration++;
            }

            return weights;
        }

        private static double[] RandomWeights(int neurons)
        {
            var random = new Random();
            var weights = new double[neurons];
            for (var i = 0; i < neurons; i++)
            {
                weights[i] = random.NextDouble();
            }

            return weights;
        }

        private static int[,] ReadData(string path, int count, out int[] outputs)
        {
            var trainingSet = File.ReadAllLines(path);

            var input = new int[count, Neurons];
            outputs = new int[count];
            for (var index = 0; index < trainingSet.Length; index++)
            {
                var splintedLine = trainingSet[index].Split(" ");
                var strArr = splintedLine[0].ToCharArray();
                var neuron = Array.ConvertAll(strArr, c => byte.Parse(c.ToString()));
                for (var i = 0; i < neuron.Length; i++)
                {
                    input[index, i] = neuron[i];
                }

                outputs[index] = byte.Parse(splintedLine[1]);
            }

            return input;
        }

        private static void GenerateSamples()
        {
            var samplesGenerator = new SamplesGenerator(Neurons);

            if (_overrideSamples || !File.Exists(TrainFile))
                samplesGenerator.GenerateSet(_trainSamples, TrainFile);
            if (_overrideSamples || !File.Exists(TestFile))
                samplesGenerator.GenerateSet(_testSamples, TestFile);
        }

        private static int CalculateOutput(IEnumerable<int> inputs, IReadOnlyList<double> weights)
        {
            var sum = weights[^1] * _bias + inputs.Select((t, i) => t * weights[i]).Sum();

            return (sum >= 0) ? 1 : 0;
        }
    }
}