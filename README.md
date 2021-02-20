# Json.Net & Json.Net.Core
A minimalistic JSON handler library. 

Json.Net (.NET Standard 2.0) : https://www.nuget.org/packages/Json.Net/

Json.Net.Core (.NET 5.0) :https://www.nuget.org/packages/Json.Net.Core/

## Usage instructions
Define a POCO class...

``` cs
class Pet
{
  public int id;
  public string name;
}
```
***
Serialization...
``` cs
var petJson = JsonNet.Serialize(pet);
```
***
Deserialization...
``` cs
var pet = JsonNet.Deserialize<Pet>(petJson);
```            
***
You can also define and use custom type converters to control serialization/deserialization.
``` cs
var dateConverter = 
  new JsonConverter<DateTime>(
    dt => dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture),
    s => DateTime.ParseExact(s, "yyyy'-'MM'-'dd'T'HH':'mm':'ss", CultureInfo.InvariantCulture));
  
var petJson = JsonNet.Serialize(pet, dateConverter);
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
void Serialize(object obj, Stream stream, params IJsonConverter[] converters)
```

  #### Description
  Serializes an object to a JSON text stream destination.

  #### Parameters
  obj : Object to be serialized  
  stream : JSON stream  
  converters : Custom type converters. Default: empty

***

``` cs
void Serialize(object obj, TextWriter writer, params IJsonConverter[] converters)
```

  #### Description
  Serializes an object to a JSON text writer destination.

  #### Parameters
  obj : Object to be serialized   
  writer : JSON text writer  
  converters : Custom type converters. Default: empty
                

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
T Deserialize<T>(Stream stream, params IJsonConverter[] converters)
```
  
  #### Description
  Deserializes an object from a JSON text stream source.
  
  #### Parameters
  T : Deserialized object's type    
  stream : JSON stream    
  converters : Custom converters. Default: empty
  
***

``` cs
T Deserialize<T>(TextReader reader, params IJsonConverter[] converters)
```

  #### Description
  Deserializes an object from a JSON text reader source.
  
  #### Parameters
  T : Deserialized object's type    
  reader : JSON text reader    
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
