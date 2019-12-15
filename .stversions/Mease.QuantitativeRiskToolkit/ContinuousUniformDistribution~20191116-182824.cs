using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public class ContinuousUniformDistribution : EstimatedDistribution
    {
        private double _min;
        private double _max;

        public ContinuousUniformDistribution() : base()
        {

        }

        public ContinuousUniformDistribution(double min, double max) : base()
        {
            Min = min;
            Max = max;
        }

        public ContinuousUniformDistribution(double min, double max, int randomSeed) : this(min, max)
        {
            RandomSeed = randomSeed;
        }

        public double Min
        {
            get => _min;
            set
            {
                _min = value;
                SetNeedsRecalculation();
            }
        }

        public double Max
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
            Random rand = new Random(RandomSeed);

            double[] array = new double[Simulation.NumberOfSamples];

            ContinuousUniform.Samples(rand, array, Min, Max);

            var vector = Vector<double>.Build.DenseOfArray(array);

            return vector;
        }
    }
}
