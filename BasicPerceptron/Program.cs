using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BasicPerceptron
{
    class Program
    {
        private const string TrainFile = "Train.txt";
        private const string TestFile = "Test.txt";
        private const int Samples = 1000;
        private const int Neurons = 21;
        private const double LearningRate = 1;
        
        static void Main(string[] args)
        {
            GenerateSamples();
            var trainedWeights = TrainPerceptron();


            Console.ReadLine();
        }

        private static double[] TrainPerceptron()
        {
            var trainingSet = File.ReadAllLines(TrainFile);

            var input = new int[Samples, Neurons];
            var outputs = new int[Samples];
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

            var random = new Random();
            var weights = new double[Neurons];
            for (var i = 0; i < Neurons; i++)
            {
                weights[i] = random.NextDouble();
            }

            double totalError = 1;
            var iteration = 1;
            while (totalError > 0.2)
            {
                if (iteration > 10000)
                    break;

                totalError = 0;
                for (var i = 0; i < Samples; i++)
                {
                    var output = CalculateOutput(input.GetRow(i), weights);

                    var error = outputs[i] - output;

                    for (var j = 0; j < Neurons; j++)
                    {
                        weights[j] += LearningRate * error * input[i, j];
                    }

                    totalError += Math.Abs(error);
                    Console.WriteLine($"iteration={iteration++}, totalError={totalError}");
                }

            }

            Console.WriteLine("Results:");
            for (var i = 0; i < Neurons; i++)
                Console.WriteLine(CalculateOutput(input.GetRow(i), weights));

            return weights;
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
            var sum = inputs.Select((t, i) => t * weights[i]).Sum();
            return (sum >= 0) ? 1 : 0;
        }
    }
}