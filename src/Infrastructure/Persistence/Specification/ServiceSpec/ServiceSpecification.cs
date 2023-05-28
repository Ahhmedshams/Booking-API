namespace Infrastructure.Persistence.Specification.ServiceSpec
{
    public class ServiceSpecification : BaseSpecification<Service>
    {
        public ServiceSpecification(ServiceSpecParams specParams) :
        base(s => s.IsDeleted == false)
        {
            ApplyPagging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "Name":
                        AddOrderBy(s => s.Name);
                        break;
                }
            }

            if(specParams.Name != null)
            {
                AddSearchBy(s => s.Name.ToLower() ==  specParams.Name.ToLower());
            }

            if (specParams.Id.HasValue)
            {
                AddSearchBy(s => s.Id ==  specParams.Id.Value);
            }

        }


    }
}
