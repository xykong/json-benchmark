using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Running;
using Newtonsoft.Json;

namespace json_benchmark
{
    public class Product
    {
        public string Name;
        public DateTime Expiry;
        public string[] Sizes;
    }

    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class JsonBenchmark
    {
        private readonly Product _product;

        private readonly string json =
            @"{
    'Name': 'Apple',
    'Expiry': '2008-12-28T00:00:00',
    'Sizes': [
    'Small'
        ]
}";

        public JsonBenchmark()
        {
            _product = new Product
            {
                Name = "Apple",
                Expiry = new DateTime(2008, 12, 28),
                Sizes = new[] { "Small" }
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
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<JsonBenchmark>();
        }
    }
}