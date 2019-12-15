using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public class ContinuousUniformDistribution : EstimatedDistribution
    {
        private QrtValue _min;
        private QrtValue _max;

        public ContinuousUniformDistribution() : base()
        {

        }

        public ContinuousUniformDistribution(Guid guid) : base(guid)
        {

        }

        public ContinuousUniformDistribution(QrtValue min, QrtValue max) : base()
        {
            Min = min;
            Max = max;
        }

        public ContinuousUniformDistribution(double min, double max, int randomSeed) : this(min, max)
        {
            RandomSeed = randomSeed;
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
        public override bool ContrainedToInt => false;

        protected override Vector<double> ComputeResult()
        {
            Random rand = Simulation.GetRandom(RandomSeed);
            double[] doubleArray = new double[Simulation.NumberOfSamples];

            if (!Min.IsDistribution && !Max.IsDistribution)
            {
                ContinuousUniform.Samples(rand, doubleArray, Min.ScalarValue, Max.ScalarValue);

            }
            else
            {
                var tempMin = Min.RawVector;
                var tempMax = Max.RawVector;

                for (int i = 0; i < Simulation.NumberOfSamples; i++)
                {
                    doubleArray[i] = ContinuousUniform.Sample(rand, tempMin[i], tempMax[i]);
                }
            }

            var vector = Vector<double>.Build.DenseOfArray(doubleArray);
            return vector;
        }
    }
}
