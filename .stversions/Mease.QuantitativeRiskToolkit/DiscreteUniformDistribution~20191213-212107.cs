using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
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
