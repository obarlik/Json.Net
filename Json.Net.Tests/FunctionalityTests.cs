using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Json.Net.Tests
{
    [TestClass]
    public class FunctionalityTests
    {
        enum Gender
        {
            Female,
            Male
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
            "{\"id\":1,\"name\":\"gucci\",\"birth\":\"08/30/2012 13:41:59\","
          + "\"alive\":true,\"gender\":1,\"dictType\":{\"Key1\":\"Value1\","
          + "\"Key2\":\"Value2\"},\"intArray\":[1,2,3]}";

        Pet OriginalPet = new Pet()
        {
            id = 1,
            name = "gucci",
            birth = new DateTime(2012, 8, 30, 13, 41, 59),
            alive = true,
            gender = Gender.Male,
            dictType = new Dictionary<string, string>()
                {
                    {"Key1", "Value1"},
                    {"Key2", "Value2"},
                },
            intArray = new[] { 1, 2, 3 }
        };


        [TestMethod]
        public void SerializationTest()
        {
            var petJson = JsonNet.Serialize(OriginalPet);

            Assert.AreEqual(petJson, OriginalPetJson);
        }


        [TestMethod]
        public void DeserializationTest()
        {
            var restoredPet = JsonNet.Deserialize<Pet>(OriginalPetJson);
            
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
        }


        JsonConverter<DateTime>  DateConverter = 
            new JsonConverter<DateTime>(
                dt => dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
                s => DateTime.ParseExact(s, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture));


        [TestMethod]
        public void ConverterTest()
        {
            var petJson = JsonNet.Serialize(OriginalPet, DateConverter);

            var restoredPet = JsonNet.Deserialize<Pet>(petJson);

            Debug.Assert(restoredPet.birth.ToString() == OriginalPet.birth.ToString());
        }


        public class Employee
        {
            public int id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Dept { get; set; }

        }

        [TestMethod]
        public void AnotherTest()
        {
            var json = "[{\"id\":2,\"Name\":\"Debendra\",\"Email\":\"debendra256@gmail.com\"," +
                "\"Dept\":\"IT\"},{\"id\":3,\"Name\":\"Manoj\",\"Email\":\"ManojMass@gmail.com\"," +
                "\"Dept\":\"Sales\"},{\"id\":6,\"Name\":\"Kumar\",\"Email\":\"Kumar256@gmail.com\",\"Dept\":\"IT\"}]";

            var empList = JsonNet.Deserialize<Employee[]>(json);
        }
    }
}
