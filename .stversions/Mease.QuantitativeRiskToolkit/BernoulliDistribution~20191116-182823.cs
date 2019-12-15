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
        private double _probability;

        public BernoulliDistribution() : base()
        {
        }

        public double Probability 
        { 
            get => _probability;
            set
            {
                if (_probability != value)
                {
                    if (value < 0.0 || value > 1.0)
                    {
                        throw new ArgumentException("Value must be between 0 and 1.", nameof(Probability));
                    }

                    _probability = value;
                }
            }
        }

        public override bool ContrainedToInt => true;

        protected override Vector<double> ComputeResult()
        {
            Random rand = new Random(RandomSeed);

            int[] intArray = new int[Simulation.NumberOfSamples];

            Bernoulli.Samples(rand, intArray, Probability);

            var vector = Vector<double>.Build.DenseOfArray(intArray.ConvertToDouble());

            return vector;
        }
    }
}
