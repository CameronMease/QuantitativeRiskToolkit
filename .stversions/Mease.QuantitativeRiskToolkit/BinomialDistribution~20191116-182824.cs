using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;

namespace Mease.QuantitativeRiskToolkit
{
    public class BinomialDistribution : EstimatedDistribution
    {
        private double _probability;
        private int _numberOfTrials;

        public BinomialDistribution() : base()
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

        public int NumberOfTrials
        {
            get => _numberOfTrials;
            set
            {
                if (_numberOfTrials != value)
                {
                    if (value < 0.0)
                    {
                        throw new ArgumentException("Value must be more than or eqaul to 0.", nameof(NumberOfTrials));
                    }

                    _numberOfTrials = value;
                }
            }
        }

        public override bool ContrainedToInt => true;

        protected override Vector<double> ComputeResult()
        {
            Random rand = new Random(RandomSeed);

            int[] intArray = new int[Simulation.NumberOfSamples];

            Binomial.Samples(rand, intArray, Probability, NumberOfTrials);

            var vector = Vector<double>.Build.DenseOfArray(intArray.ConvertToDouble());

            return vector;
        }
    }
}
