using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace BenchMark
{

    [DataContract]
    public class Employee
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Dept { get; set; }

    }
    class Program
    {
        
        class BenchSet
        {
            public string Name;

            public Func<List<Employee>, string> Serialize;
            public double SerializeTime;
            
            public Func<string, List<Employee>> Deserialize;
            public double DeserializeTime;
        }


        static DataContractJsonSerializer MsSerializer = new DataContractJsonSerializer(typeof(List<Employee>));
        static MemoryStream MsSerializerBuffer = new MemoryStream();


        static BenchSet[] Benchmarks =
            new BenchSet[]
            {                
                new BenchSet {
                    Name = "Jil",
                    Serialize = emp => Jil.JSON.Serialize(emp),
                    Deserialize = json => Jil.JSON.Deserialize<List<Employee>>(json)
                },

                new BenchSet
                {
                    Name = "Microsoft",

                    Serialize = emp => {
                        var ms = new MemoryStream();
                        MsSerializer.WriteObject(ms, emp);
                        return Encoding.Default.GetString(ms.ToArray());
                    },

                    Deserialize= json =>
                    {
                        var ms = new MemoryStream(Encoding.Default.GetBytes(json));
                        return (List<Employee>)MsSerializer.ReadObject(ms);
                    }
                },
 
                new BenchSet {
                    Name = "Newtonsoft.Json",
                    Serialize = emp => Newtonsoft.Json.JsonConvert.SerializeObject(emp),
                    Deserialize = json => Newtonsoft.Json.JsonConvert.DeserializeObject<List<Employee>>(json)
                },
                
                new BenchSet {
                    Name = "Json.Net",
                    Serialize = emp => Json.Net.JsonNet.Serialize(emp),
                    Deserialize = json => Json.Net.JsonNet.Deserialize<List<Employee>>(json)
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
                List<Employee> liemp = new List<Employee>();
                Employee emp = new Employee();

                liemp.Add(new Employee { id = 2, Name = "Debendra", Email = "debendra256@gmail.com", Dept = "IT" });
                liemp.Add(new Employee { id = 3, Name = "Manoj", Email = "ManojMass@gmail.com", Dept = "Sales" });
                liemp.Add(new Employee { id = 6, Name = "Kumar", Email = "Kumar256@gmail.com", Dept = "IT" });

                var jsonData1 = "";

                var times = MeasureAction(() =>
                {
                    jsonData1 = bench.Serialize(liemp);
                })
                .Take(iterCount)
                .ToArray();

                bench.SerializeTime = times.Min(t => t.TotalMilliseconds) * 1000;

                List<Employee> employeeDeserialized = null;

                times = MeasureAction(() =>
                {
                    employeeDeserialized = bench.Deserialize(jsonData1);
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

            foreach(var bench in results)
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
                var start = DateTime.UtcNow;
                action();
                yield return DateTime.UtcNow - start;
            }
        }
    }
}