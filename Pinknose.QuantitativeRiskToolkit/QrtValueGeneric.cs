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
    public abstract class QrtValue
    {
        [JsonProperty]
        public string TypeAssemblyQualifiedName => this.GetType().AssemblyQualifiedName;

        public abstract bool IsDistribution { get; }
        public abstract Distribution DistributionValue { get; protected set; }

        public abstract object GetScalarValueObject();
    }

    public class QrtValue<T> : QrtValue where T : struct
    {
        [JsonProperty]
        public T ScalarValue { get; private set; }

        public override object GetScalarValueObject() => ScalarValue;

        [JsonProperty]
        public override Distribution DistributionValue { get; protected set; } = null;

        public static implicit operator QrtValue<T>(T value)
        {
            QrtValue<T> obj = new QrtValue<T>()
            {
                ScalarValue = value,
                _isDistribution = false,
                vectorOfScalars = Vector<double>.Build.Dense(Simulation.NumberOfSamples, Convert.ToDouble(value))
            };

            return obj;
        }

        public static implicit operator QrtValue<T>(Distribution distribution)
        {
            if (distribution == null)
            {
                throw new ArgumentNullException(nameof(distribution));
            }

            if (typeof(T) == typeof(int) && !distribution.ContrainedToInt)
            {
                throw new ArgumentException("Distribution must be constrained to int.");
            }

            QrtValue<T> obj = new QrtValue<T>()
            {
                DistributionValue = distribution,
                _isDistribution = true
            };

            return obj;
        }

        Vector<double> vectorOfScalars;

        [JsonIgnore]
        public Vector<double> RawVector
        {
            get
            {
                if (_isDistribution)
                {
                    return DistributionValue.GetResult();
                }
                else
                {
                    return vectorOfScalars;
                }
            }
        }

        public T this[int index]
        {
            get
            {
                if (IsDistribution)
                {
                    return (T)(object)DistributionValue.GetResult()[index];
                }
                else
                {
                    //return ScalarValue;
                    // Temp test code
                    return (T)(object)vectorOfScalars[index];
                }
            }
        }

        [JsonIgnore]
        public T Maximum
        {
            get
            {
                if (_isDistribution)
                {
                    return (T)Convert.ChangeType(DistributionValue.Maximum, typeof(T));
                }
                else
                {
                    return ScalarValue;
                }
            }
        }

        [JsonIgnore]
        public T Minimum
        {
            get
            {
                if (IsDistribution)
                {
                    return (T)Convert.ChangeType(DistributionValue.Minimum, typeof(T));
                }
                else
                {
                    return ScalarValue;
                }
            }
        }

        [JsonProperty]
        public bool ContrainedToInt =>
            typeof(T) == typeof(int) ||
            (IsDistribution && DistributionValue.ContrainedToInt);

        private bool _isDistribution;

        [JsonProperty]
        public override bool IsDistribution => _isDistribution;
    }
}
