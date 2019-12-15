using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public abstract class EstimatedDistribution : Distribution
    {
        public EstimatedDistribution()
        {
            RandomSeed = (Int32)DateTime.Now.Ticks;
        }

        public EstimatedDistribution(int randomSeed)
        {
            RandomSeed = randomSeed;
        }

        private int _randomSeed;

        public int RandomSeed
        {
            get => _randomSeed;
            set
            {
                _randomSeed = value;
                SetNeedsRecalculation();
            }
        }
    }
}
