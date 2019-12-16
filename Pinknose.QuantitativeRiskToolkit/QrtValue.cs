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
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit
{
    public sealed class QrtValue
    {
        //[JsonProperty]
        //public string TypeAssemblyQualifiedName => this.GetType().AssemblyQualifiedName;

        [JsonProperty]
        public double ScalarValue { get; internal set; }

        public object GetScalarValueObject() => ScalarValue;

        [JsonIgnore]
        public Distribution DistributionValue
        {
            get
            {
                if (IsDistribution && _distributionValue == null)
                {
                    if (Simulation.DistributionExists(DistributionGuid))
                    {
                        _distributionValue = Simulation.GetDistribution(DistributionGuid);
                    }
                    else
                    {
                        throw new KeyNotFoundException($"Distribution with GUID of {DistributionGuid} could not be found.");
                    }
                }
                return _distributionValue;
            }
            protected set
            {
                DistributionGuid = value.Guid;
                _distributionValue = value;
            }
        }

        [JsonProperty]
        internal Guid DistributionGuid 
        {
            get;
            set;  
        }

        public static implicit operator QrtValue(double value)
        {
            QrtValue obj = new QrtValue()
            {
                ScalarValue = value,
                IsDistribution = false,
                vectorOfScalars = Vector<double>.Build.Dense(Simulation.NumberOfSamples, value),
                ConstrainedToInt = false
            };

            return obj;
        }

        public static implicit operator QrtValue(int value)
        {
            QrtValue obj = new QrtValue()
            {
                ScalarValue = value,
                IsDistribution = false,
                vectorOfScalars = Vector<double>.Build.Dense(Simulation.NumberOfSamples, value),
                ConstrainedToInt = true
            };

            return obj;
        }

        public static implicit operator QrtValue(Distribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            QrtValue obj = new QrtValue()
            {
                DistributionValue = distribution,
                IsDistribution = true,
                ConstrainedToInt = distribution.ContrainedToInt
            };

            return obj;
        }

        private Vector<double> vectorOfScalars;
        private Distribution _distributionValue = null;

        [JsonIgnore]
        public Vector<double> RawVector
        {
            get
            {
                if (IsDistribution)
                {
                    return DistributionValue.GetResult();
                }
                else
                {
                    return vectorOfScalars;
                }
            }
        }

        public double this[int index]
        {
            get
            {
                if (IsDistribution)
                {
                    return DistributionValue.GetResult()[index];
                }
                else
                {
                    //return ScalarValue;
                    // Temp test code
                    return vectorOfScalars[index];
                }
            }
        }

        [JsonIgnore]
        public double Maximum
        {
            get
            {
                if (IsDistribution)
                {
                    return DistributionValue.Maximum;
                }
                else
                {
                    return ScalarValue;
                }
            }
        }

        [JsonIgnore]
        public double Minimum
        {
            get
            {
                if (IsDistribution)
                {
                    return DistributionValue.Minimum;
                }
                else
                {
                    return ScalarValue;
                }
            }
        }


        [JsonProperty]
        public bool IsDistribution { get; internal set; }

        [JsonProperty]
        public bool ConstrainedToInt { get; internal set; }
    }
}
