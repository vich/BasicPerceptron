using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

//https://en.wikipedia.org/wiki/Perceptron
namespace BasicPerceptron
{
    class Program
    {

        #region Members

        private const string TrainFile = "Train.txt";
        private const string TestFile = "Test.txt";
        private const int TrainSamples = 60;
        private const int TestSamples = 1_000_000;
        private const int Neurons = 21;
        private const double LearningRate = 0.1;
        private const int Bias = 1;
        private const bool OverrideSamples = true;

        #endregion Members
        
        static void Main(string[] args)
        {
            GenerateSamples();
            var trainedWeights = TrainPerceptron();
            TestPerceptron(trainedWeights);

            Console.ReadLine();
        }

        private static void TestPerceptron(double[] weights)
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"Start {nameof(TestPerceptron)}");
            var input = ReadData(TestFile, TestSamples, out var outputs);

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

            for (var i = 0; i < TestSamples; i++)
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

            var sb = new StringBuilder();
            sb.Append("weights: ");
            for (var index = 0; index < weights.Length; index++)
            {
                var weight = weights[index];
                if (index == weights.Length - 1)
                    sb.Append("Bias");
                sb.Append($"[{index}]-{weight}; ");
            }

            Console.WriteLine(sb);
            Console.WriteLine();
            Console.WriteLine($"Results: total mistake={countWrongZeros+countWrongOnes}-{100 * (countWrongZeros + countWrongOnes) / (double)outputs.Length}%," +
                              $" wrong zeros number={countWrongZeros}-{100 * countWrongZeros / (double)countExpectedZeros}%," +
                              $" wrong ones number={countWrongOnes}-{100 * countWrongOnes / (double)countExpectedOnes}%");
        }

        private static double[] TrainPerceptron()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            var input = ReadData(TrainFile, TrainSamples, out var outputs);
            var weights = RandomWeights(Neurons + 1); //+1 for bias

            double totalError = 1;
            var iteration = 1;
            while (totalError > 0.01)
            {
                if (iteration > 100 * TrainSamples)
                    break;

                totalError = 0;
                for (var i = 0; i < TrainSamples; i++)
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

            Console.WriteLine($"Training finished, stop watch={stopWatch.ElapsedMilliseconds} milliseconds");
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

            if (OverrideSamples || !File.Exists(TrainFile))
                samplesGenerator.GenerateSet(TrainSamples, TrainFile);
            if (OverrideSamples || !File.Exists(TestFile))
                samplesGenerator.GenerateSet(TestSamples, TestFile);
        }

        private static int CalculateOutput(IEnumerable<int> inputs, IReadOnlyList<double> weights)
        {
            /*var sum = weights[^1] * Bias;
            //dot product
            for (var index = 0; index < inputs.Count; index++)
            {
                var input = inputs[index];
                sum += input * weights[index];
            }*/
            
            var sum = weights[^1] * Bias + inputs.Select((input, index) => input * weights[index]).Sum();

            return (sum >= 0) ? 1 : 0;
        }
    }
}