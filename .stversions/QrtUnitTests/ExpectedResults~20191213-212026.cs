using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mease.QuantitativeRiskToolkit.Tests
{
    public static class ExpectedResults
    {
        static public readonly JObject Data = (JObject)JsonConvert.DeserializeObject(
            File.ReadAllText(@"ExpectedResults.json"));

        static public int NumberOfSamples => Data["NumberOfSamples"].Value<int>();


        public static T GetDistributionObject<T>() where T : Distribution
        {
            Type objType = typeof(T);

            var distrConfig = Data["DistributionResults"][objType.Name];
            var inputs = distrConfig["ConstructorInputs"];

            var constructors = objType.GetConstructors().Where(c => c.GetParameters().Length == inputs.Children().Count());

            foreach (JProperty input in inputs.Children())
            {
                constructors = constructors.Where(c => c.GetParameters().Any(p => p.Name == input.Name));
            }

            Assert.IsTrue(constructors.Count() == 1);

            var parameterInfo = constructors.ElementAt(0).GetParameters();

            object[] parameters = new object[parameterInfo.Length];
            
            for (int i = 0; i < parameterInfo.Length; i++)
            {
                var parameter = parameterInfo[i];

                if (parameter.ParameterType == typeof(int))
                {
                    //parameters[i] = inputs[parameter.Name].Value();
                }
            }

            //constructors.ElementAt(0).Invoke

            return null;
        }
    }
}
