using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Specification.ClientBookingSpec
{
    public class ClientBookingSpecification : BaseSpecification<ClientBooking>
    {
        //protected readonly UserManager<ApplicationUser> _userManager;
        

        //public ClientBookingSpecification(UserManager<ApplicationUser> userManager)
        //{
        //    _userManager = userManager;
        //}

        public ClientBookingSpecification(ClientBookingSpecParam specParams)
           : base(b => b.IsDeleted == false)
        {
            AddOrderBy(b => b.Date);
            AddIncludes(b => b.User);
            AddIncludes(b => b.Service);

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

            if(specParams.StartTime.HasValue)
            {
                AddSearchBy(b => b.StartTime == specParams.StartTime.Value);
            }

            if (specParams.EndTime.HasValue)
            {
                AddSearchBy(b => b.StartTime == specParams.EndTime.Value);
            }

            if (specParams.Status.HasValue)
            {
                AddSearchBy(b => b.Status == specParams.Status);
            }

            if(specParams.UserID != null)
            {
                AddSearchBy(b=> b.UserId == specParams.UserID);
            }

        }
       
    }
}
