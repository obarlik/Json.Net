namespace Json.Net
{
    public class SerializationOptions
    {
        public IJsonConverter[] Converters { get; set; }

        public IPropertyNameTransform PropertyNameTransform { get; set; } 
    }
}
