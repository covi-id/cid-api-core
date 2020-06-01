using System;

namespace CoviIDApiCore.Utilities
{
    public class Helpers
    {
        public static int GenerateRandom4DigitNumber()
        {
            const int min = 1000;
            const int max = 9999;
            var random = new Random();
            return random.Next(min, max);
        }
    }
}