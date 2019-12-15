/*
 *   [SHORT DESCRIPTION]
 *   
 *   Copyright(C) 2019  Cameron Mease (cameron@pinknose.net)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;

namespace Pinknose.QuantitativeRiskToolkit
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
