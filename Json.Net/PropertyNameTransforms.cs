namespace Json.Net
{
    public static class PropertyNameTransforms
    {
        public static IPropertyNameTransform TitleToCamelCase { get; } = new TitleToCamelCase();
    }
}
