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

using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit
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

        public DistributionExpression(Guid guid, Expression expression) : base(guid)
        {
            Expression = expression;
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
