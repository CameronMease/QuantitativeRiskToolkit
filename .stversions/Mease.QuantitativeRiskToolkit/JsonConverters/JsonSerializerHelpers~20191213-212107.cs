using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mease.QuantitativeRiskToolkit.JsonConverters
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
    }
}
