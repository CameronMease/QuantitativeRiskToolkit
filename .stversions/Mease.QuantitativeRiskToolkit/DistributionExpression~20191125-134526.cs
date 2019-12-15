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

        protected override Vector<double> ComputeResult() => Expression.Lambda<Func<Vector<double>>>(Expression).Compile()();

        private Expression Expression { get; set; }

        private bool _constrainedToInt;

        public override bool ContrainedToInt => _constrainedToInt;
    }
}
