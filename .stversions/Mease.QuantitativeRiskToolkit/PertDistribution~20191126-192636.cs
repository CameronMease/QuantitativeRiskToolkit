using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public class PertDistribution : EstimatedDistribution
    {
        private QrtValue<double> _min;
        private QrtValue<double> _max;
        private QrtValue<double> _mostLikely;

        public PertDistribution() : base()
        {

        }

        public PertDistribution(QrtValue<double> min, QrtValue<double> mostLikely, QrtValue<double> max) : base()
        {
            Min = min;
            Max = max;
        }

        public PertDistribution(QrtValue<double> min, QrtValue<double> mostLikely, QrtValue<double> max, int randomSeed) : this(min, mostLikely, max)
        {
            RandomSeed = randomSeed;
        }

        [JsonProperty]
        public QrtValue<double> Min
        {
            get => _min;
            set
            {
                _min = value;
                SetNeedsRecalculation();
            }
        }

        [JsonProperty]
        public QrtValue<double> Max
        {
            get => _max;
            set
            {
                _max = value;
                SetNeedsRecalculation();
            }
        }

        [JsonProperty]
        public QrtValue<double> MostLikely
        {
            get => _mostLikely;
            set
            {
                _mostLikely = value;
                SetNeedsRecalculation();
            }
        }

        [JsonProperty]
        public override bool ContrainedToInt => false;

        protected override Vector<double> ComputeResult()
        {
            Random rand = Simulation.GetRandom(RandomSeed);

            double[] doubleArray = new double[Simulation.NumberOfSamples];

            if (!Min.IsDistribution && !MostLikely.IsDistribution && !Max.IsDistribution)
            {
                double a = (4 * (MostLikely.ScalarValue - Min.ScalarValue)) / (Max.ScalarValue - Min.ScalarValue) + 1;

                if (x.min > x.max || x.mode > x.max || x.mode < x.min) stop("invalid parameters");

                x.range < -x.max - x.min;
                if (x.range == 0) return (rep(x.min, n));

                mu < -(x.min + x.max + lambda * x.mode) / (lambda + 2);

                // special case if mu == mode
                if (mu == x.mode)
                {
                    v < -(lambda / 2) + 1
                }
                else
                {
                    v < -((mu - x.min) * (2 * x.mode - x.min - x.max)) /
                        ((x.mode - mu) * (x.max - x.min));
                }

                w < -(v * (x.max - mu)) / (mu - x.min);
                return (rbeta(n, v, w) * x.range + x.min);

                ContinuousUnifomples(rand, doubleArray, Min.ScalarValue, Max.ScalarValue);
            }
            else
            {
                for (int i = 0; i < Simulation.NumberOfSamples; i++)
                {
                    doubleArray[i] = ContinuousUniform.Sample(rand, Min[i], Max[i]);
                }
            }

            var vector = Vector<double>.Build.DenseOfArray(doubleArray);

            return vector;
        }
    }
}
