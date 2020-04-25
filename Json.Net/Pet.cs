using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Json.Net
{
    public enum Gender
    {
        None,
        Female,
        Male,
        Other
    }


    [DataContract]
    public class Pet
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public DateTime birth { get; set; }

        [DataMember]
        public bool alive { get; set; }

        [DataMember]
        public Gender gender { get; set; }

        [DataMember]
        public Dictionary<string, string> dictType { get; set; }

        [DataMember]
        public int[] intArray { get; set; }


        public static string OriginalPetJson =
            "{\"id\":1,\"name\":\"gucci\",\"birth\":\"08/30/2012 13:41:59\","
          + "\"alive\":true,\"gender\":1,\"dictType\":{\"Key1\":\"Value1\","
          + "\"Key2\":\"Value2\"},\"intArray\":[1,2,3]}";


        public static Pet OriginalPet = new Pet()
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
    }

    
}
