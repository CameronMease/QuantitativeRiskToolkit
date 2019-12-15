using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Pinknose.QuantitativeRiskToolkit.JsonConverters
{
    public class ExpressionJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType.IsAssignableTo(typeof(Expression));

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

            JToken token = ((JTokenReader)reader).CurrentToken;
            string nodeType = token["NodeType"].Value<string>();
            Expression expression;

            switch (nodeType)
            {
                case "Add":
                    expression = Expression.Add(token["Left"].ToObject<Expression>(serializer), token["Right"].ToObject<Expression>(serializer));
                    break;

                case "Call":
                    string methodName = token["Method"].Value<string>();
                    object callingObject = token["Object"].ToObject()
                    break;

                default:
                    throw new NotImplementedException();
            }

            return expression;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var expression = (Expression)value;

            JToken token = new JObject();

            token["NodeType"] = ((Expression)value).NodeType.ToString();

            var objectType = value.GetType();

            if (objectType.IsAssignableTo(typeof(ConstantExpression)))
            {
                SerializeConstantExpression((ConstantExpression)expression, ref token, serializer);
            }
            else if (objectType.IsAssignableTo(typeof(MethodCallExpression)))
            {
                SerializeMethodCallExpression((MethodCallExpression)expression, ref token, serializer);
            }
            else if (objectType.IsAssignableTo(typeof(BinaryExpression)))
            {
                SerializeBinaryExpression((BinaryExpression)expression, ref token, serializer);
            }
            else
            {
                throw new NotImplementedException();
            }

            token.WriteTo(writer);
        }

        private static void SerializeMethodCallExpression(MethodCallExpression expression, ref JToken token, JsonSerializer serializer)
        {
            if (expression.Method.Name == nameof(Distribution.GetResult))
            {
                token["Object"] = JObject.FromObject(expression.Object, serializer);
                token["Method"] = expression.Method.Name;
            }
            else if (expression.Method.DeclaringType == typeof(Vector<double>))
            {
                token["Type"] = expression.Method.DeclaringType.ToString();
                token["Method"] = expression.Method.Name;
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

            token["Arguments"] = JArray.FromObject(arguments, serializer);
        }

        private static void SerializeConstantExpression(ConstantExpression expression, ref JToken token, JsonSerializer serializer)
        {
            if (expression.Value.IsInteger())
            {
                token["Value"] = (Int32)expression.Value;
            }
            else if (expression.Value.IsFloating())
            {
                token["Value"] = (Double)expression.Value;
            }
            else
            {
                token["Value"] = JObject.FromObject(expression.Value, serializer);
            }
        }

        private static void SerializeBinaryExpression(BinaryExpression expression, ref JToken token, JsonSerializer serializer)
        { 
            token["Left"] = JObject.FromObject(expression.Left, serializer);
            token["Right"] = JObject.FromObject(expression.Right, serializer);
        }
    }
}
