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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.JsonConverters
{
    public static class JsonSettings
    {
        //private static BinaryExpressionJsonConverter binaryExpressionJsonConverter = new BinaryExpressionJsonConverter();
        //private static ConstantExpressionJsonConverter constantExpressionJsonConverter = new ConstantExpressionJsonConverter();
        //private static MethodCallExpressionJsonConverter methodCallExpressionJsonConverter = new MethodCallExpressionJsonConverter();
        private static QrtValueJsonConverter qrtValueJsonConverter = new QrtValueJsonConverter();
        private static DistributionMinimalJsonConverter distributionMinimalJsonConverter = new DistributionMinimalJsonConverter();
        private static DistributionExpressionJsonConverter distributionExpressionJsonConverter = new DistributionExpressionJsonConverter();
        private static DistributionJsonConverter distributionJsonConverter = new DistributionJsonConverter();
        private static ExpressionJsonConverter expressionJsonConverter = new ExpressionJsonConverter();

        public static JsonSerializerSettings SerializerSettings
        {            
            get
            {
                var settings = new JsonSerializerSettings();

                settings.Converters.Add(distributionJsonConverter);
                //settings.Converters.Add(binaryExpressionJsonConverter);
                settings.Converters.Add(expressionJsonConverter);
                //settings.Converters.Add(methodCallExpressionJsonConverter);
                settings.Converters.Add(qrtValueJsonConverter);
                //settings.Converters.Add(distributionExpressionJsonConverter);

                return settings;
            }
        }

        /*
        public static JsonSerializerSettings DistributionSerializerSettings
        {
            get
            {
                var settings = new JsonSerializerSettings();

                settings.Converters.Add(binaryExpressionJsonConverter);
                settings.Converters.Add(constantExpressionJsonConverter);
                settings.Converters.Add(methodCallExpressionJsonConverter);
                settings.Converters.Add(qrtValueJsonConverter);
                settings.Converters.Add(distributionExpressionJsonConverter);

                return settings;
            }
        }
        */

        

        
    }
}
