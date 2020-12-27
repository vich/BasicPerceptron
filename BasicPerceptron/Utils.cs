using System;

namespace BasicPerceptron
{
    public static class Utils
    {
        public static bool MostlyOnes(int num)
        {
            var ones = CountSetBits(num);
            var bitsCount = (int) Math.Floor(Math.Log10(num) + 1);
            return ones > (bitsCount + 1) / 2;
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