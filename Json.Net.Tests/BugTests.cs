using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Json.Net.Tests
{
    [TestClass]
    public class BugTests
    {
        class Bug001
        {   
            public string[] list;

            public string msg;
        }


        [TestMethod]
        public void TestMethod1()
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
        }
    }
}
