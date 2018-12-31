using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Json.Net;

namespace BenchMark
{
    class Program
    {

        class BenchSet
        {
            public string Name;

            public Func<List<Pet>, string> Serialize;
            public double SerializeTime;

            public Func<string, List<Pet>> Deserialize;
            public double DeserializeTime;
        }


        static DataContractJsonSerializer MsSerializer = new DataContractJsonSerializer(typeof(List<Pet>));
        static MemoryStream MsSerializerBuffer = new MemoryStream();


        static BenchSet[] Benchmarks =
            new BenchSet[]
            {
                new BenchSet {
                    Name = "Jil",
                    Serialize = pet => Jil.JSON.Serialize(pet),
                    Deserialize = json => Jil.JSON.Deserialize<List<Pet>>(json)
                },

                new BenchSet
                {
                    Name = "Microsoft",

                    Serialize = pet => {
                        var ms = new MemoryStream();
                        MsSerializer.WriteObject(ms, pet);
                        return Encoding.Default.GetString(ms.ToArray());
                    },

                    Deserialize= json =>
                    {
                        var ms = new MemoryStream(Encoding.Default.GetBytes(json));
                        return (List<Pet>)MsSerializer.ReadObject(ms);
                    }
                },

                new BenchSet {
                    Name = "Newtonsoft.Json",
                    Serialize = emp => Newtonsoft.Json.JsonConvert.SerializeObject(emp),
                    Deserialize = json => Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pet>>(json)
                },

                new BenchSet {
                    Name = "Json.Net",
                    Serialize = emp => JsonNet.Serialize(emp),
                    Deserialize = json => JsonNet.Deserialize<List<Pet>>(json)
                },

            };


        static void Main(string[] args)
        {
            var iterCount = 10000;

            if (args.Count() > 0)
                int.TryParse(args[0], out iterCount);

            Console.WriteLine(
                "\r\n" +
                "Benchmarking {0} iterations...",
                iterCount);


            var results = Benchmarks
            .Select(bench =>
            {
                var pet = new Pet();

                var pets = new List<Pet>(){
                    new Pet {
                        id = 2,
                        name = "Bella",
                        alive = false,
                        birth = new DateTime(2010, 2, 5),
                        dictType = new Dictionary<string, string> {
                            { "City", "New York" },
                        },
                        gender = Gender.Female,
                        intArray = new [] {
                            3, 5, 7
                        }
                    },
                    new Pet {
                        id = 3,
                        name = "Lucy",
                        alive = true,
                        birth = new DateTime(2018, 8, 10),
                        dictType = new Dictionary<string, string> {
                            { "City", "Paris" },
                        },
                        gender = Gender.Female,
                        intArray = new [] {
                            2, 4, 6
                        }
                    },

                };


                var listJson = "";

                var times = MeasureAction(() =>
                {
                    listJson = bench.Serialize(pets);
                })
                .Take(iterCount)
                .ToArray();

                bench.SerializeTime = times.Min(t => t.TotalMilliseconds) * 1000;

                List<Pet> restoredList = null;

                times = MeasureAction(() =>
                {
                    restoredList = bench.Deserialize(listJson);
                })
                .Take(iterCount)
                .ToArray();

                bench.DeserializeTime = times.Min(t => t.TotalMilliseconds) * 1000;

                return bench;
            })
            .OrderBy(b => Math.Sqrt(Math.Pow(b.SerializeTime, 2) + Math.Pow(b.DeserializeTime, 2)))
            .ToArray();

            Console.Write(
                "\r\n" +
                "{0,-20} {1,20}   {2,20}  ", "Library", "Serialization", "Deserialization");

            foreach (var bench in results)
            {
                Console.Write(
                    "\r\n" +
                    "{0,20} ", bench.Name);

                Console.Write(
                    "{0,20}µs ",
                    bench.SerializeTime.ToString("0.00"));

                Console.Write("{0,20}µs", bench.DeserializeTime.ToString("0.00"));
            }

            Console.WriteLine();
            Console.ReadLine();
        }


        static IEnumerable<TimeSpan> MeasureAction(Action action)
        {
            while (true)
            {
                var watch = DateTime.UtcNow;
                action();
                yield return DateTime.UtcNow - watch;
            }
        }
    }
}