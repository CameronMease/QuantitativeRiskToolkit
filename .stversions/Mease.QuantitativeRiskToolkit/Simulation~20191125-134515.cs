using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public static class Simulation
    {
        public static int NumberOfSamples { get; set; } = 10000;

        public static Random GetRandom(int seed)
        {
            return new MersenneTwister(seed, true);
        }

        public static Random GetRandom()
        {
            return GetRandom(RandomSeed.Robust());
        }
    }
}
