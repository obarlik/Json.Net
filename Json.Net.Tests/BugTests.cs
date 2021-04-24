using NUnit.Framework;
using System;
using System.Collections;
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
        // Issue 8
        public void TestBug004()
        {
            var dict_test = new Dictionary<int, string>{
                {1, "hello" }
            };
            
            var serialized_json = JsonNet.Serialize(dict_test);
            
            var dict_new = JsonNet.Deserialize<Dictionary<int, string>>(serialized_json);//throws exception

            Assert.AreEqual(dict_test, dict_new);
        }


        class Bug005
        {
            public Guid Id { get; set; }
        }

        [Test]
        public void TestBug005()
        {
            var bug005 = new Bug005 { Id = Guid.NewGuid() };

            var serialized_json = JsonNet.Serialize(bug005);

            var newBug005 = JsonNet.Deserialize<Bug005>(serialized_json);//throws exception

            Assert.AreEqual(bug005.Id, newBug005.Id);
        }

        class Bug006
        {
            public IEnumerable data { get; set; }
            public IEnumerable<string> data2 { get; set; }
        }

        [Test]
        public void TestBug006()
        {
            var bug006 = new Bug006
            {
                data = new[] { 1, 2 },
                data2 = new[] { "a", "b" }
            };

            var serialized_json = JsonNet.Serialize(bug006);

            var newBug006 = JsonNet.Deserialize<Bug006>(serialized_json);//throws exception

            Assert.True(true);
        }


        [Flags]
        enum Bug007Status
        {
            V1 = 1,
            V2 = 2,
            V3 = 4
        }

        class Bug007
        {
            public bool? vBool { get; set; }
            public Bug007Status? status { get; set; }
        }

        [Test]
        public void TestBug007()
        {
            var bug007 = new Bug007
            {
                status = Bug007Status.V1 | Bug007Status.V2,
                vBool = true
            };

            var serialized_json = JsonNet.Serialize(bug007);

            var newBug007 = JsonNet.Deserialize<Bug007>(serialized_json);//throws exception

            Assert.AreEqual(bug007.vBool, newBug007.vBool);
            Assert.AreEqual(bug007.status, newBug007.status);
        }


        public class ApiResult
        {
            public object Data { get; set; }
            public string Code { get; set; }

            public bool OK => Code == "OK";
        }

        [Test]
        public void TestIEnumerableDeserialization()
        {
            var json = "{\"Data\":{\"Data\":[\"a\",1,true],\"totalCount\":-1,\"groupCount\":-1,\"summary\":\"summary text\"},\"Code\":\"OK\",\"OK\":true}";
            var result = JsonNet.Deserialize<ApiResult>(json);

            var newJson = JsonNet.Serialize(result);

            Assert.AreEqual(json, newJson);
        }

    }
}
