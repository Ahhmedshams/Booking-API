using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class PaymentTransactionRepository: CRUDRepository<PaymentTransaction>, IPayemntTransactionRepository
    {
        public PaymentTransactionRepository(ApplicationDbContext applicationDbContext): base(applicationDbContext) { }
    }
}
