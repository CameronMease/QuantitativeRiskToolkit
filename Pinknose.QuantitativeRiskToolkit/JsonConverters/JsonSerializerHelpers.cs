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
using System.Reflection;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.JsonConverters
{
    public static class JsonSerializerHelpers
    {
        public static void OptInWriteJson(JsonWriter writer, object value, JsonSerializer serializer, bool stubDistributions)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueType = value.GetType();
            var props = valueType.GetProperties();

            JObject jObject = new JObject();

            foreach (var prop in props)
            {
                var customAttributes = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), true);

                if (customAttributes.Length > 0)
                {
                    var jsonPropAttribute = (JsonPropertyAttribute)customAttributes[0];

                    var propertyValue = prop.GetValue(value);

                    if (propertyValue == null && prop.PropertyType == typeof(string))
                    {
                        propertyValue = "";
                    }

                    string propertyName;
                    
                    if (!string.IsNullOrEmpty(jsonPropAttribute.PropertyName))
                    {
                        propertyName = jsonPropAttribute.PropertyName;
                    }
                    else
                    {
                        propertyName = prop.Name;
                    }

                    if (prop.PropertyType.IsAssignableTo(typeof(Distribution)) && stubDistributions)
                    {
                        jObject[propertyName] = "DFDFDF";
                    }
                    else
                    {
                        jObject[propertyName] = JToken.FromObject(propertyValue, serializer);
                    }
                }
            }

            jObject.WriteTo(writer);
        }

        public static MethodInfo GetExpressionCreatorMethodInfo(JToken expressionToken)
        {
            string expressionType = expressionToken["ExpressionType"].Value<string>();
            string expressionNodeType = expressionToken["NodeType"].Value<string>();

            switch (expressionNodeType)
            {
                case "Call":
                    return typeof(Expression).GetMethod("Call", new Type[] { typeof(Expression), typeof(MethodInfo) });
                case "Add":
                    return typeof(Expression).GetMethod(expressionNodeType, new Type[] { typeof(Expression), typeof(Expression) });
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
