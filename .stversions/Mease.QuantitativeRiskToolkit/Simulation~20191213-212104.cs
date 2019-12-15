using MathNet.Numerics.Random;
using Mease.QuantitativeRiskToolkit.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace Mease.QuantitativeRiskToolkit
{
    public static class Simulation
    {
        private static SortedDictionary<Guid, Distribution> distributions = new SortedDictionary<Guid, Distribution>();

        internal static void RegisterDistribution(Distribution distribution) => distributions.Add(distribution.Guid, distribution);

        internal static void ChangeDistributionGuid(Guid oldGuid, Guid newGuid)
        {
            var distr = distributions[oldGuid];
            distributions.Remove(oldGuid);
            distributions.Add(newGuid, distr);
        }

        internal static Distribution GetDistribution(Guid guid) => distributions[guid];

        internal static Distribution GetDistribution(string guid) => distributions[new Guid(guid)];

        public static int NumberOfSamples { get; set; } = 10000;

        public static RandomSource GetRandom(int seed)
        {
            return new MersenneTwister(seed, true);
        }

        public static RandomSource GetRandom()
        {
            return GetRandom(GetSeed());
        }

        public static int GetSeed()
        {
            int seed;

            if (!string.IsNullOrEmpty(RandomOrgApiKey))
            {
                try
                {
                    lock (seedsLock)
                    {
                        if (nextSeedIndex >= seeds.Length)
                        {
                            seeds = GetSeedsFromRandomOrg(SeedRequestCount);
                            nextSeedIndex = 0;
                        }

                        seed = seeds[nextSeedIndex];
                        nextSeedIndex++;
                    }
                }
                catch
                {
                    seed = RandomSeed.Robust();
                }
            }
            else
            {
                seed = RandomSeed.Robust();
            }

            return seed;
        }

        public static string RandomOrgApiKey { get; set; } = String.Empty;

        private static int _seedRequestCount = 2048;
        /// <summary>
        /// Number of seeds to request from Random.org at one time.  Cannot be more than 2048.
        /// </summary>
        public static int SeedRequestCount 
        { 
            get => _seedRequestCount;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Count must be more than 0.");
                }
                else if (value > 1250) // Limit set by random.org
                {
                    throw new ArgumentException($"Count cannot be more than 1250.");
                }

                _seedRequestCount = value;
            }
        }

        private static int nextSeedIndex = 0;
        private static int[] seeds = Array.Empty<int>();
        private static object seedsLock = new object();
        

        private static int[] GetSeedsFromRandomOrg(int count)
        {
            JObject json = (JObject)JsonConvert.DeserializeObject("{\"jsonrpc\":\"2.0\",\"method\":\"generateIntegers\",\"params\":{\"apiKey\":\"replace me\",\"n\":10,\"min\":0,\"max\":255,\"replacement\":true,\"base\":10},\"id\":12345}");
            json["params"]["apiKey"] = RandomOrgApiKey;
            json["params"]["n"] = count * 8;

            UriBuilder uriBuilder = new UriBuilder("https://api.random.org/json-rpc/2/invoke");

            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
                var result = httpClient.PostAsync(uriBuilder.Uri, content).Result;
                result.EnsureSuccessStatusCode();

                json = (JObject)JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }

            int[] tempSeeds = new int[count];

            for (int tempSeedIndex = 0; tempSeedIndex < count; tempSeedIndex++)
            {
                byte[] bytes = new byte[8];

                for (int i = 0; i < 8; i++)
                {
                    bytes[i] = json["result"]["random"]["data"][i * tempSeedIndex].Value<byte>();
                }

                tempSeeds[tempSeedIndex] = BitConverter.ToInt32(bytes, 0);
            }

            return tempSeeds;
        }

        public static string Serialize()
        {
            JsonSerializer serializer = JsonSerializer.Create(JsonSettings.SerializerSettings);
            //JsonSerializer distributionSerializer = JsonSerializer.Create(JsonSettings.DistributionSerializerSettings);
            
            JObject jObject = new JObject();        
            
            jObject["Distributions"] = 
                JArray.FromObject(distributions.Values.Where(d => !d.GetType().IsAssignableTo(typeof(DistributionExpression))), serializer);

            jObject["Expressions"] =
                JArray.FromObject(distributions.Values.Where(d => d.GetType().IsAssignableTo(typeof(DistributionExpression))), serializer);

            return jObject.ToString();
        }

        public static void Deserialize(string json)
        {
            JsonSerializer serializer = JsonSerializer.Create(JsonSettings.SerializerSettings);
            //JsonSerializer distributionSerializer = JsonSerializer.Create(JsonSettings.DistributionSerializerSettings);

            var tempDistributions = distributions;

            distributions = new SortedDictionary<Guid, Distribution>();

            JObject jObject = JObject.Parse(json);

            foreach (var distrJObject in jObject["Distributions"])
            {
                Type distrType = Type.GetType(distrJObject[nameof(Distribution.TypeAssemblyQualifiedName)].ToString());
                var distr = (Distribution)distrJObject.ToObject(distrType, serializer);
                distributions[distr.Guid] = distr;
            }

            foreach (var distrJObject in jObject["Expressions"])
            {
                Type distrType = Type.GetType(distrJObject[nameof(Distribution.TypeAssemblyQualifiedName)].ToString());
                var distr = (Distribution)distrJObject.ToObject(distrType, serializer);
                distributions[distr.Guid] = distr;
            }
        }
    }
}
