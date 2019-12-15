using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace Mease.QuantitativeRiskToolkit
{
    public class BernoulliDistribution : EstimatedDistribution
    {
        private QrtValue<double> _probability;

        public BernoulliDistribution() : base()
        {
        }

        public BernoulliDistribution(QrtValue<double> probability) : base()
        {
            Probability = probability;
        }

        public BernoulliDistribution(QrtValue<double> probability, int randomSeed) : base()
        {
            Probability = probability;
            RandomSeed = randomSeed;
        }

        public QrtValue<double> Probability 
        { 
            get => _probability;
            set
        {
                if ((!value.IsDistribution && (value.ScalarValue < 0.0 || value.ScalarValue > 1.0)) ||
                    (value.IsDistribution && (value.DistributionValue.Result().Min() < 0.0 || value.DistributionValue.Result().Max() > 1.0)))
                {
                    throw new ArgumentException("Value must be between 0 and 1.", nameof(Probability));
                }

                _probability = value;
            }
        }

        public override bool ContrainedToInt => true;

        protected override Vector<double> ComputeResult()
        {
            Random rand = Simulation.GetRandom(RandomSeed);

            int[] intArray = new int[Simulation.NumberOfSamples];

            if (Probability.IsDistribution)
            {
                for (int i = 0; i < Simulation.NumberOfSamples; i++)
                {
                    intArray[i] = Bernoulli.Sample(Probability.DistributionValue.Result()[i]);
                }
            }
            else
            {
                Bernoulli.Samples(rand, intArray, Probability.ScalarValue);
            }

            var vector = Vector<double>.Build.DenseOfArray(intArray.ConvertToDouble());

            return vector;
        }
    }
}
