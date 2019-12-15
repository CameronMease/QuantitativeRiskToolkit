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
using MathNet.Numerics.Random;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit
{
    public class PertDistribution : EstimatedDistribution
    {
        private QrtValue _min;
        private QrtValue _max;
        private QrtValue _mostLikely;

        public PertDistribution() : base()
        {

        }

        public PertDistribution(QrtValue min, QrtValue mostLikely, QrtValue max) : base()
        {
            Min = min;
            Max = max;
            MostLikely = mostLikely;
        }

        public PertDistribution(QrtValue min, QrtValue mostLikely, QrtValue max, int randomSeed) : this(min, mostLikely, max)
        {
            RandomSeed = randomSeed;
        }

        public PertDistribution(Guid guid) : base(guid)
        {
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
        public QrtValue MostLikely
        {
            get => _mostLikely;
            set
            {
                _mostLikely = value;
                SetNeedsRecalculation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Algorithm adapted from: https://www.riskamp.com/beta-pert
        /// </remarks>
        [JsonProperty]
        public override bool ContrainedToInt => false;

        protected override Vector<double> ComputeResult()
        {
            RandomSource rand = Simulation.GetRandom(RandomSeed);

            double lambda = 4;

            double[] doubleArray = new double[Simulation.NumberOfSamples];
            
            if (!Min.IsDistribution && !MostLikely.IsDistribution && !Max.IsDistribution)
            {
                if (Min.ScalarValue > Max.ScalarValue)
                {
                    throw new Exception("Min most be less than or equal to Max.");
                }
                else if (MostLikely.ScalarValue > Max.ScalarValue)
                {
                    throw new Exception("MostLikely must be less than or equal to Max.");
                }
                else if (MostLikely.ScalarValue < Min.ScalarValue)
                {
                    throw new Exception("MostLikely must be more than or equal to Min.");
                }

                double range = Max.ScalarValue - Min.ScalarValue;

                if (range == 0)
                {
                    return Vector<double>.Build.Dense(Simulation.NumberOfSamples, Min.ScalarValue);
                }
                else
                {
                    var betaParams = GetBetaParameters(rand, Min.ScalarValue, MostLikely.ScalarValue, Max.ScalarValue, lambda);

                    Beta.Samples(Simulation.GetRandom(this.RandomSeed), doubleArray, betaParams.A, betaParams.B);

                    Vector<double> vector = Vector<double>.Build.DenseOfArray(doubleArray);

                    vector = (vector * range) + Min.ScalarValue;

                    return vector;
                }
            }
            else // One or more inputs are distributions
            {
                if (Min.Maximum > Max.Minimum)
                {
                    throw new Exception("Min most be less than or equal to Max.");
                }
                else if (MostLikely.Maximum > Max.Minimum)
                {
                    throw new Exception("MostLikely must be less than or equal to Max.");
                }
                else if (MostLikely.Minimum < Min.Maximum)
                {
                    throw new Exception("MostLikely must be more than or equal to Min.");
                }

                var tempMin = Min.RawVector;
                var tempMostLikely = MostLikely.RawVector;
                var tempMax = Max.RawVector;

                for (int i = 0; i < Simulation.NumberOfSamples; i++)
                {
                 
                    double range = tempMax[i] - tempMin[i];

                    if (range == 0)
                    {
                        doubleArray[i] = tempMin[i];
                    }
                    else
                    {
                        var betaParams = GetBetaParameters(rand, tempMin[i], tempMostLikely[i], tempMax[i], lambda);

                        doubleArray[i] = Beta.Sample(rand, betaParams.A, betaParams.B) * range + tempMin[i];
                    }
                }

                Vector<double> vector = Vector<double>.Build.DenseOfArray(doubleArray);

                return vector;
            }           
        }

        public static (double A, double B) GetBetaParameters(RandomSource random, double min, double mostLikely, double max, double lambda)
        {
            double a, b;
            double range = max - min;

            double mean = (min + max + lambda * mostLikely) / (lambda + 2.0);

            // special case if mean == mode
            if (mean == mostLikely)
            {
                a = (lambda / 2.0) + 1.0;
            }
            else
            {
                a = ((mean - min) * (2.0 * mostLikely - min - max)) /
                    ((mostLikely - mean) * (max - min));
            }

            b = (a * (max - mean)) / (mean - min);

            return (a, b);
        }
    }
}
