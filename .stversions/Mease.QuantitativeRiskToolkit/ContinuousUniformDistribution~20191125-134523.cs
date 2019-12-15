using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public class ContinuousUniformDistribution : EstimatedDistribution
    {
        private QrtValue<double> _min;
        private QrtValue<double> _max;

        public ContinuousUniformDistribution() : base()
        {

        }

        public ContinuousUniformDistribution(QrtValue<double> min, QrtValue<double> max) : base()
        {
            Min = min;
            Max = max;
        }

        public ContinuousUniformDistribution(double min, double max, int randomSeed) : this(min, max)
        {
            RandomSeed = randomSeed;
        }

        public QrtValue<double> Min
        {
            get => _min;
            set
            {
                _min = value;
                SetNeedsRecalculation();
            }
        }

        public QrtValue<double> Max
        {
            get => _max;
            set
            {
                _max = value;
                SetNeedsRecalculation();
            }
        }

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
