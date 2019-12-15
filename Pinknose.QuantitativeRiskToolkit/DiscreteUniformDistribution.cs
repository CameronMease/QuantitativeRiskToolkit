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

using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit
{
    public class DiscreteUniformDistribution : EstimatedDistribution
    {
        private QrtValue _min;
        private QrtValue _max;

        public DiscreteUniformDistribution() : base()
        {
            Min = 0;
            Max = 0;
        }

        public DiscreteUniformDistribution(Guid guid) : base(guid)
        {
        }

        public DiscreteUniformDistribution(QrtValue min, QrtValue max) : base()
        {
            if (min == null)
            {
                throw new ArgumentNullException(nameof(min));
            }

            if (max == null)
            {
                throw new ArgumentNullException(nameof(max));
            }               

            Min = min;
            Max = max;
        }

        public DiscreteUniformDistribution(int min, int max, int randomSeed) : this(min, max)
        {
            RandomSeed = randomSeed;
        }

        [JsonProperty]
        public QrtValue Min 
        { 
            get => _min;
            set
            {
                if (!value.ConstrainedToInt)
                {
                    throw new ArgumentException("Argument must be constrained to integer values.");
                }

                _min = value;

                SetNeedsRecalculation();
            }
        }

        [JsonProperty]
        public QrtValue Max 
        { 
            get => _max;
            set
            {
                if (!value.ConstrainedToInt)
                {
                    throw new ArgumentException("Argument must be constrained to integer values.");
                }

                _max = value;
                SetNeedsRecalculation();
            }
        }

        [JsonProperty]
        public override bool ContrainedToInt => true;

        protected override Vector<double> ComputeResult()
        {
            Random rand = Simulation.GetRandom(RandomSeed);

            int[] intArray = new int[Simulation.NumberOfSamples];

            if (!Min.IsDistribution && !Max.IsDistribution)
            {
                DiscreteUniform.Samples(rand, intArray, (int)Min.ScalarValue, (int)Max.ScalarValue);
            }
            else
            {
                for (int i = 0; i < Simulation.NumberOfSamples; i++)
                {
                    intArray[i] = DiscreteUniform.Sample(rand, (int)Min[i], (int)Max[i]);
                }
            }

            var vector = Vector<double>.Build.DenseOfArray(intArray.ConvertToDouble());

            return vector;
        }
    }
}
