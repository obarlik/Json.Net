# Json.Net
Minimalistic JSON handler


Usage instructions
--------------------------------------------
Define a POCO class, with just field definitions.

class Pet
{
  public int id;
  public string name;
}


Serializing an object:
--------------------------------------------
var petJson = Json.Serialize(originalPet);

petJson's value:

{
	"id" : 1,
	"name" : "gucci",
	"birth" : "12/12/2018 14:13:46",
	"alive" : true,
	"gender" : 1,
	"dictType" : {
		"Key1" : "Value1",
		"Key2" : "Value2"
	},
	"intArray" : [
		1,
		2,
		3
	]
}


Deserializing from JSON string:
--------------------------------------------
var restoredPet = Json.Deserialize<Pet>(petJson);


Output formatting
--------------------------------------------
You can set indentation on/off by passing

               

Custom type converters
--------------------------------------------
You can define and use custom type converters to control serialization/deserialization.

var dateConverter = new JsonConverter<DateTime>(
                    dt => dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
                    s => DateTime.ParseExact(s, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture));
  
var petJson = Json.Serialize(originalPet, false, dateConverter);

petJson's value:

{"id":1,"name":"gucci","birth":"2018-12-12T14:13:46","alive":true,"gender":1,"dictType":{"Key1":"Value1","Key2":"Value2"},"intArray":[1,2,3]}


Reference:
---------------------------------------------
using Json.Net;

  Function Method
  	string Json.Serialize(object obj, bool indent = false, params IJsonConverter[] converters)

  Description
  	Serializes an object to its JSON text representation.

  Parameters
	  obj        : Object to be serialized
	  indent     : If true, formats output text. Default: false
	  converters : Custom type converters. Default: empty
  

  Function Method
  	T Json.Deserialize<T>(string json, params IJsonConverter[] converters)
  
  Description
  	Deserializes an object from a JSON text.
  
  Parameters
  	T : Deserialized object's type
  	json : JSON text
  	converters : Custom converters. Default: empty
  
  // Converter interface
  public interface IJsonConverter
  {
      Type GetConvertingType();
      string Serializer(object obj);
      object Deserializer(string txt);
  }
  
