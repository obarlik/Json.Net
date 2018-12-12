# Json.Net
A minimalistic JSON handler library. 
Framework : .NET Standard 2.0


## Usage instructions
Define a POCO class, with just field definitions.

``` cs
class Pet
{
  public int id;
  public string name;
}
```

### Serializing an object:
``` cs
var petJson = JsonNet.Serialize(originalPet, true);
```

petJson's value:

``` javascript
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
```

### Deserializing from JSON string:
``` cs
var restoredPet = JsonNet.Deserialize<Pet>(petJson);
```            

### Custom type converters
You can define and use custom type converters to control serialization/deserialization.
``` cs
var dateConverter = 
  new JsonConverter<DateTime>(
    dt => dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
    s => DateTime.ParseExact(s, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture));
  
var petJson = JsonNet.Serialize(originalPet, true, dateConverter);
```

petJson's value:
``` javascript
{
	"id" : 1,
	"name" : "gucci",
	"birth" : "2018-12-12T14:13:46",
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
```

## Reference:

### Name space
``` cs
using Json.Net;
```

### Methods
***
``` cs
string JsonNet.Serialize(object obj, bool indent = false, params IJsonConverter[] converters)
```

  #### Description
  Serializes an object to its JSON text representation.

  #### Parameters
  obj        : Object to be serialized  
  
  indent     : If true, formats output text. Default: false  
  
  converters : Custom type converters. Default: empty
  
***
``` cs
T JsonNet.Deserialize<T>(string json, params IJsonConverter[] converters)
```
  
  #### Description
  Deserializes an object from a JSON text.
  
  #### Parameters
  T : Deserialized object's type
  
  json : JSON text
  
  converters : Custom converters. Default: empty
  
***

### Converter interface
``` cs
public interface IJsonConverter
{
  Type GetConvertingType();
  string Serializer(object obj);
  object Deserializer(string txt);
}
```  
