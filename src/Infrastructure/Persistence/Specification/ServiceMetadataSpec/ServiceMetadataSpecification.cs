using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Specification.ServiceMetadataSpec
{
    public class ServiceMetadataSpecification: BaseSpecification<ServiceMetadata>
    {
        public ServiceMetadataSpecification(ServiceMetadataSpecParams specParams)
            :base(m => m.IsDeleted == false)
        {
            AddIncludes(s => s.ResourceType);
            ApplyPagging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

            if (specParams.ServiceId.HasValue)
            {
                AddSearchBy(m => m.ServiceId == specParams.ServiceId);
            }

            if(specParams.ResourceTypeId.HasValue)
            {
                AddSearchBy(m => m.ResourceTypeId == specParams.ResourceTypeId);
            }
        }
    }
}
