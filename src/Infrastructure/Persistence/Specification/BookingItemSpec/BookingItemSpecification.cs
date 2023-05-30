namespace Infrastructure.Persistence.Specification.BookingItemSpec
{
    public class BookingItemSpecification : BaseSpecification<BookingItem>
    {
        public BookingItemSpecification(BookingItemSpecParams specParams)
            :base(b => b.IsDeleted == false)
        {
            ApplyPagging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

            if (specParams.BookId.HasValue)
            {
                AddSearchBy(b => b.BookingId == specParams.BookId);
            }
        
            if(specParams.ResourceId.HasValue)
            {
                AddSearchBy(b => b.ResourceId == specParams.ResourceId);
            }

            if(specParams.Price.HasValue)
            {
                AddSearchBy(b => b.Price == specParams.Price);
            }
        
        }
    }
}
