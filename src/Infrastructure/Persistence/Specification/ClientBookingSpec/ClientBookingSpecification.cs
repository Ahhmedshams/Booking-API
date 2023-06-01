namespace Infrastructure.Persistence.Specification.ClientBookingSpec
{
    public class ClientBookingSpecification : BaseSpecification<ClientBooking>
    {
        public ClientBookingSpecification(ClientBookingSpecParam specParams)
           : base(b => b.IsDeleted == false)
        {
            AddOrderBy(b => b.Date);

            ApplyPagging(specParams.PageSize *(specParams.PageIndex - 1) , specParams.PageSize);
            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "time":
                        AddOrderBy(b => b.StartTime);
                        break;
                    case "duration":
                        AddOrderBy(b => b.EndTime);
                        break;
                    default:
                        AddOrderBy(b => b.Date);
                        break;
                }

            }

            if (specParams.Id.HasValue)
            {
                AddSearchBy(b => b.Id == specParams.Id.Value);
            }

            if (!string.IsNullOrEmpty(specParams.Location))
            {
                AddSearchBy(b => b.Location == specParams.Location);
            }

            if(specParams.Date.HasValue) 
            {
                AddSearchBy(b => b.Date == specParams.Date.Value);
            }

            if(specParams.Time.HasValue)
            {
                AddSearchBy(b => b.StartTime == specParams.Time.Value);
            }

            if(specParams.ServiceId.HasValue)
            {
                AddSearchBy(b => b.ServiceId == specParams.ServiceId.Value);
            }

            //if(specParams.UserId != null)
            //{
            //    AddSearchBy(b => b.UserId == specParams.UserId);
            //}
            
        }
    }
}
