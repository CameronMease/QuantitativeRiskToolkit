using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Mease.QuantitativeRiskToolkit
{
    public sealed class DistributionExpression : Distribution
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

        public DistributionExpression(Guid guid) : base(guid)
        {
        }

        private void Obj_RecalculationRequired(object sender, EventArgs e)
        {
            SetNeedsRecalculation();
        }

        protected override Vector<double> ComputeResult() => Expression.Lambda<Func<Vector<double>>>(Expression).Compile()();

        [JsonProperty]
        public Expression Expression { get; private set; }

        private bool _constrainedToInt;

        [JsonProperty]
        public override bool ContrainedToInt => _constrainedToInt;
    }
}
