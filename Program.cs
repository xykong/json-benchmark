using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;

namespace json_benchmark
{
    [DataContract]
    public class Product
    {
        [DataMember] public string Name;
        [DataMember] public DateTime Expiry;
        [DataMember] public string[] Sizes;
    }

    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    //[SimpleJob(RunStrategy.ColdStart, launchCount: 1, warmupCount: 1, targetCount: 5, id: "FastAndDirtyJob")]
    public class JsonBenchmark
    {
        private readonly Product _product;

        private readonly string json = "{\"Name\":\"Apple\",\"Expiry\":\"2008-12-27T16:00:00Z\",\"Sizes\":[\"Small\"]}";

        private readonly string jsonDataContractJsonSerializer =
            "{\"Expiry\":\"\\/Date(1230393600000+0800)\\/\",\"Name\":\"Apple\",\"Sizes\":[\"Small\"]}";

        public JsonBenchmark()
        {
            _product = new Product
            {
                Name = "Apple",
                Expiry = new DateTime(2008, 12, 28),
                Sizes = new[] {"Small"}
            };
        }

        [Benchmark]
        public string NewtonsoftSerialize()
        {
            return JsonConvert.SerializeObject(_product);
        }

        [Benchmark]
        public Product NewtonsoftDeserialize()
        {
            return JsonConvert.DeserializeObject<Product>(json);
        }

        [Benchmark]
        public string SimpleJsonSerialize()
        {
            return SimpleJson.SerializeObject(_product);
        }

        [Benchmark]
        public Product SimpleJsonDeserialize()
        {
            return SimpleJson.DeserializeObject<Product>(json);
        }

        [Benchmark]
        public object ServiceStackSerialize()
        {
            //return ServiceStack.DynamicJson.Serialize(_product);
            return ServiceStack.Text.JsonSerializer.SerializeToString(_product);
        }

        [Benchmark]
        public object ServiceStackDeserialize()
        {
            //return ServiceStack.DynamicJson.Deserialize(json);
            //Product p = ServiceStack.Text.JsonSerializer.DeserializeFromString<Product>(json);
            return ServiceStack.Text.JsonSerializer.DeserializeFromString<Product>(json);
        }

        [Benchmark]
        public object DataContractJsonSerializerSerialize()
        {
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Product));
            ser.WriteObject(ms, _product);
            byte[] jsonBytes = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(jsonBytes, 0, jsonBytes.Length);
        }

        [Benchmark]
        public object DataContractJsonSerializerDeserialize()
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonDataContractJsonSerializer));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(_product.GetType());
            var deserializedUser = ser.ReadObject(ms) as Product;
            ms.Close();
            return deserializedUser;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<JsonBenchmark>();
        }
    }
}