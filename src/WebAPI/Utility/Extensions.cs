namespace WebAPI.Utility
{
    public static class Extensions
    {
        public static  ResourceMetadata ToResourceMetadata(this ResourceAttribute resourceAttribute, int ResourceTypeId)
        {
            ResourceMetadata ResMetaData = new ResourceMetadata()
            {
                ResourceTypeId = ResourceTypeId,
                AttributeName = resourceAttribute.AttributeName,
                AttributeType = resourceAttribute.AttributeType
            };
           
            return ResMetaData;
        }

        public static IEnumerable<ResourceAttribute> CheckIsValidAttribute(this IEnumerable<ResourceAttribute> attributes)
        {
            var uniqueAttributes = attributes.Distinct(new AttributeNameComparer());

            return uniqueAttributes;
        }



    }

    
    public class AttributeNameComparer : IEqualityComparer<ResourceAttribute>
    {
        public bool Equals(ResourceAttribute x, ResourceAttribute y)
        {
            return x.AttributeName == y.AttributeName;
        }

        public int GetHashCode(ResourceAttribute obj)
        {
            return obj.AttributeName.GetHashCode();
        }
    }
}
