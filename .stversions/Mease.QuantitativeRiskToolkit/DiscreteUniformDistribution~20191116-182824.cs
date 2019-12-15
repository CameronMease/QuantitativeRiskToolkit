using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public class DiscreteUniformDistribution : EstimatedDistribution
    {
        private QrtValue<int> _min;
        private QrtValue<int> _max;

        public DiscreteUniformDistribution() : base()
        {

        }

        public DiscreteUniformDistribution(QrtValue<int> min, QrtValue<int> max) : base()
        {
            Min = min;
            Max = max;
        }

        public DiscreteUniformDistribution(int min, int max, int randomSeed) : this(min, max)
        {
            RandomSeed = randomSeed;
        }

        public QrtValue<int> Min 
        { 
            get => _min;
            set
            {
                _min = value;

                SetNeedsRecalculation();
            }
        }

        public QrtValue<int> Max 
        { 
            get => _max;
            set
            {
                _max = value;
                SetNeedsRecalculation();
            }
        }

        public override bool ContrainedToInt => true;

        protected override Vector<double> ComputeResult()
        {
            Random rand = new Random(RandomSeed);

            int[] intArray = new int[Simulation.NumberOfSamples];

            if (!Min.IsDistribution && !Max.IsDistribution)
            {
                DiscreteUniform.Samples(rand, intArray, Min.Value, Max.Value);
            }
            else
            {
                for (int i = 0; i < Simulation.NumberOfSamples; i++)
                {
                    intArray[i] = DiscreteUniform.Sample(rand, Min[i], Max[i]);
                }
            }

            var vector = Vector<double>.Build.DenseOfArray(intArray.ConvertToDouble());

            return vector;
        }
    }
}
