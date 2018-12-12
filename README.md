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
var petJson = Json.Net.Json.Serialize(originalPet);


Deserializing from JSON string:
--------------------------------------------
var restoredPet = Json.Net.Json.Deserialize<Pet>(petJson);
