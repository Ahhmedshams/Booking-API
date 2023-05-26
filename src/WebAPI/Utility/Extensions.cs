namespace WebAPI.Utility
{
    public static class Extensions
    {
        static public ResourceMetadata ToResourceMetadata(this ResourceAttribute resourceAttribute, int ResourceTypeId)
        {
            ResourceMetadata ResMetaData = new ResourceMetadata()
            {
                ResourceTypeId = ResourceTypeId,
                AttributeName = resourceAttribute.AttributeName,
                AttributeType = resourceAttribute.AttributeType
            };
           
            return ResMetaData;
        }
    }
}
