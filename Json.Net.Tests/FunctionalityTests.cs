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


        [TestMethod]
        public void BasicFunctionalityTest()
        {
            var originalPet = new Pet()
            {
                id = 1,
                name = "gucci",
                birth = DateTime.Now,
                alive = true,
                gender = Gender.Male,
                dictType = new Dictionary<string, string>()
                {
                    {"Key1", "Value1"},
                    {"Key2", "Value2"},
                },
                intArray = new[] { 1, 2, 3 }
            };

            var petJson = JsonNet.Serialize(originalPet);
            var restoredPet = JsonNet.Deserialize<Pet>(petJson);

            Debug.Assert(restoredPet.id == originalPet.id);
            Debug.Assert(restoredPet.name == originalPet.name);
            Debug.Assert(restoredPet.gender == originalPet.gender);
            Debug.Assert(restoredPet.alive == originalPet.alive);
            Debug.Assert(restoredPet.birth.ToString() == originalPet.birth.ToString());
            Debug.Assert(restoredPet.dictType["Key1"] == originalPet.dictType["Key1"]);
            Debug.Assert(restoredPet.dictType["Key2"] == originalPet.dictType["Key2"]);
            Debug.Assert(restoredPet.intArray[0] == originalPet.intArray[0]);
            Debug.Assert(restoredPet.intArray[1] == originalPet.intArray[1]);
            Debug.Assert(restoredPet.intArray[2] == originalPet.intArray[2]);

            var dateConverter = new JsonConverter<DateTime>(
                    dt => dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
                    s => DateTime.ParseExact(s, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture));

            petJson = JsonNet.Serialize(
                originalPet,
                dateConverter);

            restoredPet = JsonNet.Deserialize<Pet>(petJson);

            Debug.Assert(restoredPet.id == originalPet.id);
            Debug.Assert(restoredPet.name == originalPet.name);
            Debug.Assert(restoredPet.gender == originalPet.gender);
            Debug.Assert(restoredPet.alive == originalPet.alive);
            Debug.Assert(restoredPet.birth.ToString() == originalPet.birth.ToString());
            Debug.Assert(restoredPet.dictType["Key1"] == originalPet.dictType["Key1"]);
            Debug.Assert(restoredPet.dictType["Key2"] == originalPet.dictType["Key2"]);
            Debug.Assert(restoredPet.intArray[0] == originalPet.intArray[0]);
            Debug.Assert(restoredPet.intArray[1] == originalPet.intArray[1]);
            Debug.Assert(restoredPet.intArray[2] == originalPet.intArray[2]);
        }
    }
}
