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
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.JsonConverters
{
    public class QrtValueJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(QrtValue));

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            JToken jToken = ((JTokenReader)reader).CurrentToken;

            var qrtValue = new QrtValue
            {
                IsDistribution = jToken[nameof(QrtValue.IsDistribution)].Value<bool>(),
                ConstrainedToInt = jToken[nameof(QrtValue.ConstrainedToInt)].Value<bool>()
            };

            if (!qrtValue.IsDistribution)
            {
                qrtValue.ScalarValue = jToken[nameof(QrtValue.ScalarValue)].Value<double>();
            }
            else
            {
                qrtValue.DistributionGuid = new Guid(jToken[nameof(QrtValue.DistributionGuid)].Value<string>());
            }            
            
            return qrtValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var qrtValue = (QrtValue)value;

            JObject jObject = new JObject();

            jObject[nameof(QrtValue.IsDistribution)] = qrtValue.IsDistribution;

            if (!qrtValue.IsDistribution)
            {
                jObject[nameof(QrtValue.ScalarValue)] = (double)qrtValue.GetScalarValueObject();
            }
            else
            {
                jObject[nameof(QrtValue.DistributionGuid)] = qrtValue.DistributionValue.Guid.ToString();
            }

            jObject[nameof(QrtValue.ConstrainedToInt)] = qrtValue.ConstrainedToInt;

            jObject.WriteTo(writer);
        }
    }
}
