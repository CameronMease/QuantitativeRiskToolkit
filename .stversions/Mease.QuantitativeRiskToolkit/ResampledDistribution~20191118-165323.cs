using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Mease.QuantitativeRiskToolkit
{
    public class ResampledDistribution : EstimatedDistribution
    {
        internal ResampledDistribution(Distribution distrbution)
        {

        }

        public override bool ContrainedToInt => throw new NotImplementedException();

        protected override Vector<double> ComputeResult()
        {
            throw new NotImplementedException();
        }

        private static readonly MethodInfo ShuffleMethodInfo =
            typeof(DistributionExpression).GetMethod(nameof(DistributionExpression.Shuffle));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vector"></param>
        /// <param name="rand"></param>
        /// <remarks>Modified from https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net</remarks>
        public static Vector<double> Shuffle(Vector<double> vector, int seed)
        {
            Vector<double> newVector = Vector<double>.Build.DenseOfVector(vector);
            var rand = Simulation.GetRandom(seed);

            int n = newVector.Count;
            while (n > 1)
            {
                int k = rand.Next(n--);
                double temp = newVector[n];
                newVector[n] = newVector[k];
                newVector[k] = temp;
            }

            return newVector;
        }
    }
}
