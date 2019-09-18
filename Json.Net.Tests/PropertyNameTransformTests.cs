using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Json.Net.Tests
{
    [TestClass]
    public class PropertyNameTransformTests
    {
        enum Gender
        {
            Female,
            Male
        }

        class Person
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        class Pet
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Birth { get; set; }
            public bool Alive { get; set; }
            public Gender Gender { get; set; }
            public Dictionary<string, string> DictType { get; set; }
            public int[] IntArray { get; set; }
            public Person Owner { get; set; }
        }

        const string originalPetJson =
            "{\"id\":1,\"name\":\"gucci\",\"birth\":\"08\\/30\\/2012 13:41:59\","
          + "\"alive\":true,\"gender\":1,\"dictType\":{\"Key1\":\"Value1\\nValue2\","
          + "\"Key2\":\"Value2\"},\"intArray\":[1,2,3],\"owner\":{\"id\":1,\"name\":\"chanel\"}}";

        Pet OriginalPet = new Pet()
        {
            Id = 1,
            Name = "gucci",
            Birth = new DateTime(2012, 8, 30, 13, 41, 59),
            Alive = true,
            Gender = Gender.Male,
            DictType = new Dictionary<string, string>()
                {
                    {"Key1", "Value1\nValue2"},
                    {"Key2", "Value2"},
                },
            IntArray = new[] { 1, 2, 3 },
            Owner = new Person { Id = 1, Name = "chanel" },
        };


        [TestMethod]
        public void SerializationTest()
        {
            var petJson = JsonNet.Serialize(OriginalPet, PropertyNameTransforms.TitleToCamelCase);

            Assert.AreEqual(originalPetJson, petJson);
        }


        [TestMethod]
        public void CamelCaseDeserializationTest()
        {
            var restoredPet = JsonNet.Deserialize<Pet>(originalPetJson, PropertyNameTransforms.TitleToCamelCase);

            Assert.AreEqual(restoredPet.Id,                 OriginalPet.Id);
            Assert.AreEqual(restoredPet.Name,               OriginalPet.Name);
            Assert.AreEqual(restoredPet.Gender,             OriginalPet.Gender);
            Assert.AreEqual(restoredPet.Alive,              OriginalPet.Alive);
            Assert.AreEqual(restoredPet.Birth.ToString(),   OriginalPet.Birth.ToString());
            Assert.AreEqual(restoredPet.DictType["Key1"],   OriginalPet.DictType["Key1"]);
            Assert.AreEqual(restoredPet.DictType["Key2"],   OriginalPet.DictType["Key2"]);
            Assert.AreEqual(restoredPet.IntArray[0],        OriginalPet.IntArray[0]);
            Assert.AreEqual(restoredPet.IntArray[1],        OriginalPet.IntArray[1]);
            Assert.AreEqual(restoredPet.Owner.Id,           OriginalPet.Owner.Id);
            Assert.AreEqual(restoredPet.Owner.Name,         OriginalPet.Owner.Name);
        }
    }
}
