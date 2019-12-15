using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public abstract class EstimatedDistribution : Distribution
    {
        public EstimatedDistribution()
        {
            RandomSeed = MathNet.Numerics.Random.RandomSeed.Robust();
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

        protected static readonly PropertyInfo RandomSeedPropertyInfo = typeof(EstimatedDistribution).GetProperty(nameof(EstimatedDistribution.RandomSeed));
    }
}
