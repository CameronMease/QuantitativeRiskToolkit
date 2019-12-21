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
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Pinknose.QuantitativeRiskToolkit
{
    public class FixedDistribution : Distribution
    {
        public FixedDistribution(params double[] distribution)
        {
            vector = Vector<double>.Build.DenseOfArray(distribution);
            SetCalculationsUpToDate();
        }

        public override bool ConstrainedToInt => false;

        private Vector<double> vector;

        protected override Vector<double> ComputeResult()
        {
            return vector;
        }
    }
}
