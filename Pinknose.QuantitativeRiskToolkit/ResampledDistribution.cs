/*
 *   [SHORT DESCRIPTION]
 *   
 *   Copyright(C) 2019  Cameron Mease (cameron@pinknose.net)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;

namespace Pinknose.QuantitativeRiskToolkit
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
            Type duhh1 = typeof(Pinknose.QuantitativeRiskToolkit.Extensions);

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

        public override bool ConstrainedToInt => linkedDistribution.ConstrainedToInt;

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
