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
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;


namespace Pinknose.QuantitativeRiskToolkit.JsonConverters
{
    public class DistributionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(Distribution));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            JToken jToken = ((JTokenReader)reader).CurrentToken;

            Distribution distribution;
            var distributionGuid = new Guid(jToken["Guid"].Value<string>());

            // Check if the distribution already has already been deserialized
            if (Simulation.DistributionExists(distributionGuid))
            {
                return Simulation.GetDistribution(distributionGuid);
            }
            else
            {
                ConstructorInfo constructor;

                if (objectType.IsAssignableTo(typeof(DistributionExpression)))
                {
                    constructor = objectType.GetConstructor(new Type[] { typeof(Guid), typeof(Expression) });

                    if (constructor == null)
                    {
                        throw new NullReferenceException($"Constructor for {objectType} is null.");
                    }

                    distribution = (DistributionExpression)constructor.Invoke(new object[] { distributionGuid, jToken["Expression"].ToObject<Expression>(serializer) });
                }
                else
                {
                    constructor = objectType.GetConstructor(new Type[] { typeof(Guid) });
                    distribution = (Distribution)constructor.Invoke(new object[] { distributionGuid });
                }

                //serializer.Populate(reader, distribution);
                PopulateDistribution(jToken, distribution, serializer);

                return distribution;
            }
        }

        private static void PopulateDistribution(JToken token, Distribution distribution, JsonSerializer serializer)
        {
            Type distrType = distribution.GetType();

            var properties = distrType.GetProperties().Where(p => 
                p.CustomAttributes.Any(ca => ca.AttributeType == typeof(JsonPropertyAttribute)));

            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    var jsonPropertyAttribute = (JsonPropertyAttribute)property.GetCustomAttribute(typeof(JsonPropertyAttribute));
                    var jsonTokenName = jsonPropertyAttribute.PropertyName ?? property.Name;

                    JToken propToken = token[jsonTokenName];
                    var value = propToken.ToObject(property.PropertyType, serializer);
                    property.SetValue(distribution, value);
                }
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JsonSerializerHelpers.OptInWriteJson(writer, value, serializer, true);
        }
    }
}
