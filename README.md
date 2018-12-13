# Json.Net
A minimalistic JSON handler library. 

Framework : .NET Standard 2.0


## Usage instructions
Define a POCO class...

``` cs
class Pet
{
  public int id;
  public string name;
}
```

### Serializing
``` cs
var petJson = JsonNet.Serialize(originalPet);
```

### Deserializing
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
  
var petJson = JsonNet.Serialize(originalPet, dateConverter);
```

## Reference

### Name space
``` cs
using Json.Net;
```

### Methods
``` cs
string JsonNet.Serialize(object obj, params IJsonConverter[] converters)
```

  #### Description
  Serializes an object to its JSON text representation.

  #### Parameters
  obj        : Object to be serialized  
  
  converters : Custom type converters. Default: empty

***

``` cs
void SerializeToStream(object obj, Stream stream, params IJsonConverter[] converters)
```

  #### Description
  Serializes an object to a JSON text stream destination.

  #### Parameters
  obj        : Object to be serialized  
  
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

``` cs
T DeserializeFromStream<T>(Stream stream, params IJsonConverter[] converters)
```
  
  #### Description
  Deserializes an object from a JSON text stream source.
  
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
