using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public abstract class QrtValue
    {
        public abstract bool IsDistribution { get; }
        public abstract  Distribution DistributionValue { get; protected set; }
    }

    public class QrtValue<T> : QrtValue where T : struct
    {
        public T ScalarValue { get; private set; }
        public override Distribution DistributionValue { get; protected set; } = null;

        public static implicit operator QrtValue<T>(T value) 
        {
            QrtValue<T> obj = new QrtValue<T>()
            {
                ScalarValue = value
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
            };

            return obj;
        }
        
        public T this[int index]
        {
            get
            {
                if (IsDistribution)
                {
                    return (T)Convert.ChangeType(DistributionValue.Result()[index], typeof(T));
                }
                else
                {
                    return ScalarValue;
                }
            }
        }

        public bool ContrainedToInt =>
            typeof(T) == typeof(int) ||
            (IsDistribution && DistributionValue.ContrainedToInt);

        public override bool IsDistribution => DistributionValue != null;
    }
}
