using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public class DistributionExpression : Distribution
    {
        private DistributionExpression() : base()
        {

        }

        internal DistributionExpression(Expression expression, params Distribution[] objects) : base()
        {
            Expression = expression;

            _constrainedToInt = true;

            foreach (Distribution obj in objects)
            {
                obj.RecalculationRequired += Obj_RecalculationRequired;

                if (!obj.ContrainedToInt)
                {
                    _constrainedToInt = false;
                }
            }
        }

        private void Obj_RecalculationRequired(object sender, EventArgs e)
        {
            SetNeedsRecalculation();
        }

        protected override Vector<double> ComputeResult()
        {
            return Expression.Lambda<Func<Vector<double>>>(Expression).Compile()();
        }

        private Expression Expression { get; set; }

        private bool _constrainedToInt;

        public override bool ContrainedToInt => _constrainedToInt;

        internal static DistributionExpression CreateResampleExpression(Distribution distribution)
        {
            var newDistribution = new DistributionExpression();
            distribution.RecalculationRequired += newDistribution.Obj_RecalculationRequired;

            Type duhh = typeof(Vector<double>);

            newDistribution.Expression =
                Expression.Call(
                    Expression.Call(Expression.Constant(distribution), ResultMethodInfo), ShuffleMethodInfo);

            return newDistribution;
        }

        private static readonly MethodInfo ShuffleMethodInfo =
            typeof(Vector<Double>).GetMethod("Shuffle", new Type[] { typeof(Random)});
    }
}
