using System;

namespace BasicPerceptron
{
    public static class Utils
    {
        public static bool MostlyOnes(int num, int bitsCount)
        {
            var ones = CountSetBits(num);
            return ones > bitsCount / 2;
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