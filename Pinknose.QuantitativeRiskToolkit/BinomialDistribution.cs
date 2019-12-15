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
    public class BinomialDistribution : EstimatedDistribution
    {
        private double _probability;
        private int _numberOfTrials;

        public BinomialDistribution() : base()
        {
        }

        [JsonProperty]
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

        [JsonProperty]
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
            Random rand = Simulation.GetRandom(RandomSeed);

            int[] intArray = new int[Simulation.NumberOfSamples];

            Binomial.Samples(rand, intArray, Probability, NumberOfTrials);

            var vector = Vector<double>.Build.DenseOfArray(intArray.ConvertToDouble());

            return vector;
        }
    }
}
