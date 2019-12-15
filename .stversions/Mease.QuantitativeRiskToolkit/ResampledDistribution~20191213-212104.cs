using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;

namespace Mease.QuantitativeRiskToolkit
{
    public class ResampledDistribution : EstimatedDistribution
    {
        internal ResampledDistribution() : base()
        {

        }

        internal ResampledDistribution(Distribution distrbution)
        {
            linkedDistribution = distrbution;
            linkedDistribution.RecalculationRequired += LinkedDistribution_RecalculationRequired;

            Type duhh = typeof(Vector<double>);
            Type duhh1 = typeof(Mease.QuantitativeRiskToolkit.Extensions);

            recalcExpression =
                Expression.Call(ShuffleMethodInfo,
                    Expression.Call(Expression.Constant(linkedDistribution), ResultMethodInfo),
                    Expression.Property(Expression.Constant(this), RandomSeedPropertyInfo));
        }

        public ResampledDistribution(Guid guid) : base(guid)
        {
        }

        private void LinkedDistribution_RecalculationRequired(object sender, EventArgs e)
        {
            SetNeedsRecalculation();
        }

        private Distribution linkedDistribution;
        private Expression recalcExpression;

        public override bool ContrainedToInt => linkedDistribution.ContrainedToInt;

        protected override Vector<double> ComputeResult()
        {
            return Expression.Lambda<Func<Vector<double>>>(recalcExpression).Compile()();
        }

        private static readonly MethodInfo ShuffleMethodInfo =
            typeof(ResampledDistribution).GetMethod(nameof(ResampledDistribution.Shuffle));

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
