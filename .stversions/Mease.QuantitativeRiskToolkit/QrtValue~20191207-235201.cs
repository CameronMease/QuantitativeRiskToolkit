using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public class QrtValue
    {
        [JsonProperty]
        public string TypeAssemblyQualifiedName => this.GetType().AssemblyQualifiedName;

        [JsonProperty]
        public double ScalarValue { get; internal set; }

        public object GetScalarValueObject() => ScalarValue;

        [JsonProperty]
        public Distribution DistributionValue 
        {
            get
            {
                if (IsDistribution && _distributionValue == null)
                {
                    _distributionValue = Simulation.GetDistribution(DistributionPlaceholderGuid);
                }
                return _distributionValue;
            }
            protected set => _distributionValue = value; 
        }

        [JsonIgnore]
        internal Guid DistributionPlaceholderGuid { get; set; }

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

        Vector<double> vectorOfScalars;
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
