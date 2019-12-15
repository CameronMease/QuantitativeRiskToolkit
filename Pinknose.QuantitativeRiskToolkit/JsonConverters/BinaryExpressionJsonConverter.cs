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
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.JsonConverters
{
    public class BinaryExpressionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(BinaryExpression));

        public override object ReadJson(JsonReader reader, Type objectType,  object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            JToken jToken = ((JTokenReader)reader).CurrentToken;
            string methodName = jToken["NodeType"].Value<string>();

            var binaryMethod = JsonSerializerHelpers.GetExpressionCreatorMethodInfo(jToken);

            var leftMethod = jToken["Left"].ToObject(typeof(Expression));

            //BinaryExpression expression = binaryMethod.Invoke(null, jToken["Left"])

            return binaryMethod;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            JObject jObject = new JObject();

            jObject["ExpressionType"] = "Binary";
            jObject["NodeType"] = ((Expression)value).NodeType.ToString();
            jObject["Left"] = JObject.FromObject(((BinaryExpression)value).Left, serializer);
            jObject["Right"] = JObject.FromObject(((BinaryExpression)value).Right, serializer);

            jObject.WriteTo(writer);
        }

    }
}
