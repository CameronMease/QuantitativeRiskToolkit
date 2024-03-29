﻿/*
 *   MethodCallExpressionJsonConverter Class - For serialization/deserialzation of the MedhodCallExpression class.
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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.JsonConverters
{
    public class MethodCallExpressionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(MethodCallExpression));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var expression = (MethodCallExpression)value;

            JObject jObject = new JObject();

            jObject["ExpressionType"] = "Call";
            jObject["NodeType"] = ((Expression)value).NodeType.ToString();

            if (expression.Method.Name == nameof(Distribution.GetResult))
            { 
                jObject["Object"] = JObject.FromObject(expression.Object, serializer);
                jObject["Method"] = expression.Method.Name;
            }
            else if (expression.Method.DeclaringType == typeof(Vector<double>))
            {
                jObject["Type"] = expression.Method.DeclaringType.ToString();
                jObject["Method"] = expression.Method.Name;
            }
            else
            {
                throw new NotSupportedException($"Serialization of method calls is limited to serialiation of {nameof(Distribution.GetResult)}.");
            }

            List<JObject> arguments = new List<JObject>();

            foreach (var argument in expression.Arguments)
            {
                arguments.Add(JObject.FromObject(argument, serializer));
            }

            jObject["Arguments"] = JArray.FromObject(arguments, serializer);

            jObject.WriteTo(writer);
        }

    }
}
