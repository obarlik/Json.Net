using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Json.Net.Tests
{
    public class FunctionalityTests
    {
        enum Gender
        {
            None,
            Female,
            Male,
            Other
        }


        class Pet
        {
            public int id { get; set; }
            public string name { get; set; }
            public DateTime birth { get; set; }
            public bool alive { get; set; }
            public Gender gender { get; set; }
            public Dictionary<string, string> dictType { get; set; }
            public int[] intArray { get; set; }
        }


        string OriginalPetJson =
            "{\"id\":1,\"name\":\"gucci\",\"birth\":\"08\\/30\\/2012 13:41:59\","
          + "\"alive\":true,\"gender\":2,\"dictType\":{\"Key1\":\"Value1\\nValue2\","
          + "\"Key2\":\"Value2\"},\"intArray\":[1,2,3]}";

        string[] OriginalPetJsonFormatted =
            {
            "{",
            "  \"id\" : 1,",
            "  \"name\" : \"gucci\",",
            "  \"birth\" : \"08\\/30\\/2012 13:41:59\",",
            "  \"alive\" : true,",
            "  \"gender\" : 2,",
            "  \"dictType\" : {",
            "    \"Key1\" : \"Value1\\nValue2\",",
            "    \"Key2\" : \"Value2\"",
            "  },",
            "  \"intArray\" : [",
            "    1,",
            "    2,",
            "    3",
            "  ]",
            "}"
            };

        Pet OriginalPet = new Pet()
        {
            id = 1,
            name = "gucci",
            birth = new DateTime(2012, 8, 30, 13, 41, 59),
            alive = true,
            gender = Gender.Male,
            dictType = new Dictionary<string, string>()
                {
                    {"Key1", "Value1\nValue2"},
                    {"Key2", "Value2"},
                },
            intArray = new[] { 1, 2, 3 }
        };


        [Test]
        public void SerializationTest()
        {
            var petJson = JsonNet.Serialize(OriginalPet);

            Assert.AreEqual(petJson, OriginalPetJson);
            Assert.Pass();
        }


        [Test]
        public void DeserializationTest()
        {
            var restoredPet = JsonNet.Deserialize<Pet>(
                string.Join("\n", OriginalPetJsonFormatted));
            
            Assert.AreEqual(restoredPet.id, OriginalPet.id);
            Assert.AreEqual(restoredPet.name, OriginalPet.name);
            Assert.AreEqual(restoredPet.gender, OriginalPet.gender);
            Assert.AreEqual(restoredPet.alive, OriginalPet.alive);
            Assert.AreEqual(restoredPet.birth.ToString(), OriginalPet.birth.ToString());
            Assert.AreEqual(restoredPet.dictType["Key1"], OriginalPet.dictType["Key1"]);
            Assert.AreEqual(restoredPet.dictType["Key2"], OriginalPet.dictType["Key2"]);
            Assert.AreEqual(restoredPet.intArray[0], OriginalPet.intArray[0]);
            Assert.AreEqual(restoredPet.intArray[1], OriginalPet.intArray[1]);
            Assert.AreEqual(restoredPet.intArray[2], OriginalPet.intArray[2]);
            Assert.Pass();
        }


        JsonConverter<DateTime>  DateConverter = 
            new JsonConverter<DateTime>(
                dt => dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
                s => DateTime.ParseExact(s, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture));


        [Test]
        public void ConverterTest()
        {
            var petJson = JsonNet.Serialize(OriginalPet, DateConverter);

            var restoredPet = JsonNet.Deserialize<Pet>(petJson);

            Assert.AreEqual(restoredPet.birth.ToString(), OriginalPet.birth.ToString());
            Assert.Pass();
        }


        public class Employee
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Dept { get; set; }

        }


        [Test]
        public void ListTest()
        {
            var json = "[{\"id\":2,\"Name\":\"Debendra\",\"Email\":\"debendra256@gmail.com\"," +
                "\"Dept\":\"IT\"},{\"id\":3,\"Name\":\"Manoj\",\"Email\":\"ManojMass@gmail.com\"," +
                "\"Dept\":\"Sales\"},{\"id\":6,\"Name\":\"Kumar\",\"Email\":\"Kumar256@gmail.com\",\"Dept\":\"IT\"}]";

            var empList = JsonNet.Deserialize<Employee[]>(json);

            Assert.IsTrue(empList.Length == 3);
            Assert.AreEqual(empList.Last().Dept, "IT");
            Assert.Pass();
        }
        

        

        class TestClass005
        {
            public int Id { get; set; }

            public string Name { get; set; }

            [JsonNetIgnore]
            public string SecurityKey { get; set; }

            public string NickName { get; set; }
        }


        [Test]
        // Issue 9
        public void JsonNetIgnoreAttributeTest()
        {
            var test_object = new TestClass005
            {
                Id = 81,
                Name = "Tester 005",
                SecurityKey = "Top Secret",
                NickName = "Superman"
            };

            var json_text = JsonNet.Serialize(test_object);

            Assert.AreEqual(json_text, "{\"Id\":81,\"Name\":\"Tester 005\",\"NickName\":\"Superman\"}");

            json_text = "{\"Id\":81,\"Name\":\"Tester 005\",\"SecurityKey\":\"Top Secret\",\"NickName\":\"Superman\"}";

            var test_object2 = JsonNet.Deserialize<TestClass005>(json_text);

            Assert.AreEqual(test_object.Id, test_object2.Id);
            Assert.AreEqual(test_object.Name, test_object2.Name);
            Assert.AreNotEqual(test_object.SecurityKey, test_object2.SecurityKey);
            Assert.AreEqual(test_object.NickName, test_object2.NickName);
        }
    }
}
