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
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit
{
    public class ContinuousUniformDistribution : EstimatedDistribution
    {
        private QrtValue _min;
        private QrtValue _max;

        public ContinuousUniformDistribution() : base()
        {

        }

        public ContinuousUniformDistribution(Guid guid) : base(guid)
        {

        }

        public ContinuousUniformDistribution(QrtValue min, QrtValue max) : base()
        {
            Min = min;
            Max = max;
        }

        public ContinuousUniformDistribution(double min, double max, int randomSeed) : this(min, max)
        {
            RandomSeed = randomSeed;
        }

        [JsonProperty]
        public QrtValue Min
        {
            get => _min;
            set
            {
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
                _max = value;
                SetNeedsRecalculation();
            }
        }

        [JsonProperty]
        public override bool ContrainedToInt => false;

        protected override Vector<double> ComputeResult()
        {
            Random rand = Simulation.GetRandom(RandomSeed);
            double[] doubleArray = new double[Simulation.NumberOfSamples];

            if (!Min.IsDistribution && !Max.IsDistribution)
            {
                ContinuousUniform.Samples(rand, doubleArray, Min.ScalarValue, Max.ScalarValue);

            }
            else
            {
                var tempMin = Min.RawVector;
                var tempMax = Max.RawVector;

                for (int i = 0; i < Simulation.NumberOfSamples; i++)
                {
                    doubleArray[i] = ContinuousUniform.Sample(rand, tempMin[i], tempMax[i]);
                }
            }

            var vector = Vector<double>.Build.DenseOfArray(doubleArray);
            return vector;
        }
    }
}
