using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;

namespace Json.Net.Tests
{
    public class BugTests
    {
        class Bug001
        {
            public string[] list;

            public string msg;
        }


        [Test]
        public void TestBug001()
        {
            var json = "{\"list\":[],\"msg\":\"OK\"}";

            var bug1 = new Bug001()
            {
                list = new string[0],
                msg = "OK"
            };

            var bug2 = JsonNet.Deserialize<Bug001>(json);

            Assert.IsTrue(bug2.list.Length == 0);
            Assert.AreEqual(bug1.msg, bug2.msg);
            Assert.Pass();
        }


        class Bug002
        {
            public TimeSpan interval;
        }


        [Test]
        public void TestBug002()
        {
            var bug002 = new Bug002 { interval = TimeSpan.FromMinutes(2) };

            var json = JsonNet.Serialize(bug002);

            var constructed = JsonNet.Deserialize<Bug002>(json);

            Assert.AreEqual(bug002.interval, constructed.interval);
            Assert.Pass();
        }

        class TestBug003Configuration
        {
            public enum Protocols { Udp, Tcp }

            public (IPAddress IpAddress, int Port, Protocols Protocol) NetworkSettings { get; private set; } = (IPAddress.Any, 10001, Protocols.Udp);
        }

        [Test]
        public void TestBug003()
        {
            var configuration = new TestBug003Configuration();

            var converter = new JsonConverter<System.Net.IPAddress>
            (
                ip => configuration.NetworkSettings.IpAddress.ToString(),
                s => System.Net.IPAddress.Parse(s)
            );

            var json = JsonNet.Serialize(new TestBug003Configuration(), converter);
            Assert.Pass();
        }





        [Test]
        public void TestBug004()
        {
            var dict_test = new Dictionary<int, string>();
            dict_test.Add(1, "hello");
            var serialized_json = JsonNet.Serialize(dict_test);
            Console.WriteLine(serialized_json);//Output: {"1","hello"}
            var dict_new = JsonNet.Deserialize<Dictionary<int, string>>(serialized_json);//throws exception

            Assert.AreEqual(dict_test, dict_new);
        }
    }
}
