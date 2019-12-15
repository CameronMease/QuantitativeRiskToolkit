﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;

namespace Mease.QuantitativeRiskToolkit
{
    public class BernoulliDistribution : EstimatedDistribution
    {
        private QrtValue _probability;

        public BernoulliDistribution() : base()
        {
            Probability = 1;
        }

        public BernoulliDistribution(QrtValue probability) : base()
        {
            //TODO: Range check probability
            Probability = probability;
        }

        public BernoulliDistribution(Guid guid) : base(guid)
        {
        }

        public BernoulliDistribution(QrtValue probability, int randomSeed) : base()
        {
            //TODO: Range check probability
            Probability = probability;
            RandomSeed = randomSeed;
        }

        [JsonProperty]
        public QrtValue Probability 
        { 
            get => _probability;
            set
        {
                if ((!value.IsDistribution && (value.ScalarValue < 0.0 || value.ScalarValue > 1.0)) ||
                    (value.IsDistribution && (value.DistributionValue.GetResult().Min() < 0.0 || value.DistributionValue.GetResult().Max() > 1.0)))
                {
                    throw new ArgumentException("Value must be between 0 and 1.", nameof(Probability));
                }

                _probability = value;
            }
        }

        [JsonProperty]
        public override bool ContrainedToInt => true;

        protected override Vector<double> ComputeResult()
        {
            Random rand = Simulation.GetRandom(RandomSeed);

            int[] intArray = new int[Simulation.NumberOfSamples];

            if (Probability.IsDistribution)
            {
                for (int i = 0; i < Simulation.NumberOfSamples; i++)
                {
                    intArray[i] = Bernoulli.Sample(Probability.DistributionValue.GetResult()[i]);
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