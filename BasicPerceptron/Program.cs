using System;

namespace BasicPerceptron
{
    class Program
    {
        static void Main(string[] args)
        {
            SamplesGenerator.Generate(10000, 10000);
            
            var input = new int[,] { { 1, 0 }, { 1, 1 }, { 0, 1 }, { 0, 0 } };
            int[] outputs = { 0, 1, 0, 0 };

            var r = new Random();

            double[] weights = { r.NextDouble(), r.NextDouble(), r.NextDouble() };

            const double learningRate = 1;
            double totalError = 1;

            while (totalError > 0.2)
            {
                totalError = 0;
                for (var i = 0; i < 4; i++)
                {
                    var output = CalculateOutput(input[i, 0], input[i, 1], weights);

                    var error = outputs[i] - output;

                    weights[0] += learningRate * error * input[i, 0];
                    weights[1] += learningRate * error * input[i, 1];
                    weights[2] += learningRate * error * 1;

                    totalError += Math.Abs(error);
                }

            }

            Console.WriteLine("Results:");
            for (var i = 0; i < 4; i++)
                Console.WriteLine(CalculateOutput(input[i, 0], input[i, 1], weights));

            Console.ReadLine();

        }

        private static int CalculateOutput(double input1, double input2, double[] weights)
        {
            var sum = input1 * weights[0] + input2 * weights[1] + 1 * weights[2];
            return (sum >= 0) ? 1 : 0;
        }
    }
}