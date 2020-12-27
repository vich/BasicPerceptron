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
        private const int Samples = 100;
        private const int Neurons = 21;
        private const double LearningRate = 0.1;
        private const int Bias = 1;

        #endregion Members


        static void Main(string[] args)
        {
            GenerateSamples();
            var trainedWeights = TrainPerceptron();
            
            Console.ReadLine();
        }

        private static double[] TrainPerceptron()
        {
            var input = ReadData(TrainFile, out var outputs);
            var weights = RandomWeights(Neurons + 1);

            double totalError = 1;
            var iteration = 1;
            while (totalError > 0.2)
            {
                if (iteration > 100 * Samples)
                    break;

                totalError = 0;
                for (var i = 0; i < Samples; i++)
                {
                    var iRow = input.GetRow(i);
                    var output = CalculateOutput(iRow, weights);
                    var error = outputs[i] - output;

                    for (var j = 0; j < Neurons; j++)
                    {
                        weights[j] += LearningRate * error * input[i, j];
                    }

                    weights[^1] += LearningRate * error * Bias;

                    totalError += Math.Abs(error);
                }

                Console.WriteLine($"iteration={iteration}, totalError={totalError}");
                iteration++;
            }

            Console.WriteLine("Results:");
            for (var i = 0; i < Samples; i++)
                Console.WriteLine(CalculateOutput(input.GetRow(i), weights));

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

        private static int[,] ReadData(string path, out int[] outputs)
        {
            var trainingSet = File.ReadAllLines(path);

            var input = new int[Samples, Neurons];
            outputs = new int[Samples];
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

            if (!File.Exists(TrainFile))
                samplesGenerator.GenerateSet(Samples, TrainFile);
            if (!File.Exists(TestFile))
                samplesGenerator.GenerateSet(Samples, TestFile);
        }

        private static int CalculateOutput(IEnumerable<int> inputs, IReadOnlyList<double> weights)
        {
            var sum = weights[^1] * Bias + inputs.Select((t, i) => t * weights[i]).Sum();

            return (sum >= 0) ? 1 : 0;
        }
    }
}