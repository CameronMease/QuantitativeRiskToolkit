using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit
{
    public class EventFrequencyDistribution : EstimatedDistribution
    {
        private QrtValue _frequencyEstimate;
        private QrtValue _probabilityOfOccurence = 1;

        public EventFrequencyDistribution() : base()
        {
        }

        public EventFrequencyDistribution(Guid guid) : base(guid)
        {
        }

        public EventFrequencyDistribution(int randomSeed) : base(randomSeed)
        {
        }

        [JsonProperty]
        public QrtValue FrequencyEstimate 
        { 
            get => _frequencyEstimate;
            set
            {
                if (value != _frequencyEstimate)
                {
                    _frequencyEstimate = value;
                    this.SetNeedsRecalculation();
                }
            }
        }

        [JsonProperty]
        public QrtValue ProbabilityOfOccurence
        {
            get => _probabilityOfOccurence;
            set
            {
                if (value != _probabilityOfOccurence)
                {
                    _probabilityOfOccurence = value;
                    this.SetNeedsRecalculation();
                }
            }
        }

        public override bool ConstrainedToInt => true;

        protected override Vector<double> ComputeResult()
        {
            Vector<double> result = Vector<double>.Build.Dense(Simulation.NumberOfSamples);

            Random bernoulliRand = Simulation.GetRandom(RandomSeed);
            Random binomialRand = Simulation.GetRandom((int)bernoulliRand.NextDouble());

            for (int i = 0; i < Simulation.NumberOfSamples; i++)
            {
                var fraction = FrequencyEstimate[i] % 1;
                result[i] = FrequencyEstimate[i] - fraction;

                if (fraction != 0.0)
                {
                    result[i] += Bernoulli.Sample(bernoulliRand, fraction);
                }

                if (ProbabilityOfOccurence[i] < 1.0)
                {
                    result[i] = Binomial.Sample(binomialRand, ProbabilityOfOccurence[i], (int)result[i]);
                }
            }

            return result;
        }
    }
}
