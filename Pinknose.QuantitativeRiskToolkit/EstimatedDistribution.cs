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

using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit
{
    public abstract class EstimatedDistribution : Distribution
    {
        public EstimatedDistribution()
        {
            RandomSeed = MathNet.Numerics.Random.RandomSeed.Robust();
        }

        public EstimatedDistribution(int randomSeed)
        {
            RandomSeed = randomSeed;
        }

        protected EstimatedDistribution(Guid guid) : base(guid)
        {
        }

        private int _randomSeed;

        [JsonProperty]
        public int RandomSeed
        {
            get => _randomSeed;
            set
            {
                _randomSeed = value;
                SetNeedsRecalculation();
            }
        }

        protected static readonly PropertyInfo RandomSeedPropertyInfo = typeof(EstimatedDistribution).GetProperty(nameof(EstimatedDistribution.RandomSeed));
    }
}
