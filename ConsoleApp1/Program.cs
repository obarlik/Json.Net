using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Json.Net;


namespace BenchMark
{

    public class Employee
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Dept { get; set; }

    }
    class Program
    {



        static void Main(string[] args)
        {
            List<Employee> liemp = new List<Employee>();
            Employee emp = new Employee();


            liemp.Add(new Employee { id = 2, Name = "Debendra", Email = "debendra256@gmail.com", Dept = "IT" });
            liemp.Add(new Employee { id = 3, Name = "Manoj", Email = "ManojMass@gmail.com", Dept = "Sales" });
            liemp.Add(new Employee { id = 6, Name = "Kumar", Email = "Kumar256@gmail.com", Dept = "IT" });

            var jsonData1 = "";

            var time = MeasureAction(() =>
            {
                jsonData1 = SerializeEmployee(liemp);
            });


            Console.WriteLine("............Json.Net Serialization........");
            Console.WriteLine("Average serialization time : {0}ms", time.TotalMilliseconds);
            Console.WriteLine(jsonData1);

            List<Employee> employeeDeserialized = null;

            time = MeasureAction(() =>
            {
                employeeDeserialized = DeserializeEmployee(jsonData1);
            });

            Console.WriteLine("............Json.Net Deserialization........");
            Console.WriteLine("Average deserialization time : {0}ms", time.TotalMilliseconds);
            foreach (var data in employeeDeserialized)
            {
                Console.WriteLine("Id=" + data.id);
                Console.WriteLine("Name=" + data.Name);
                Console.WriteLine("Id=" + data.Email);
                Console.WriteLine("Id=" + data.Dept);
            }


            Console.ReadLine();

        }

        private static string SerializeEmployee(List<Employee> liemployee)
        {
            return JsonNet.Serialize(liemployee);
        }

        private static List<Employee> DeserializeEmployee(string liemployee)
        {
            return JsonNet.Deserialize<List<Employee>>(liemployee);
        }


        static TimeSpan MeasureAction(Action action, int iter = 1000000)
        {
            var start = DateTime.UtcNow;

            for (var i = 0; i < iter; i++)
                action();

            return (DateTime.UtcNow - start) / iter;
        }
    }
}