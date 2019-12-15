using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
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
                    _distributionValue = Simulation.GetDistribution(DistributionGuid);
                }
                return _distributionValue;
            }
            protected set => _distributionValue = value;
        }

        [JsonProperty]
        internal Guid DistributionGuid 
        {
            get
            {
                if (DistributionValue == null)
                {
                    return _distributionGuid;
                }
                else
                {
                    return DistributionValue.Guid;
                }
            }
            set => _distributionGuid = value; 
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
        private Guid _distributionGuid;

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
