namespace Json.Net
{
    public class TitleToCamelCase : IPropertyNameTransform
    {
        public string Transform(string propertyName) =>
            char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
    }
}
