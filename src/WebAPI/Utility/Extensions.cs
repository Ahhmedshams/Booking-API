using Domain.Entities;
using Domain.Enums;
using Stripe.Terminal;
using static Infrastructure.Utility.Permissions;

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
        public static IEnumerable<ClientBookingWithDetails> ToClientBookingWithDetails(this IEnumerable<ClientBooking> clientBooking)
        {
            var result = new List<ClientBookingWithDetails>();
            foreach (var booking in clientBooking)
            {
                var entity = booking.ToClientBookingWithDetails();
                result.Add(entity);
            }
            return result;
        }
        public static ClientBookingWithDetails ToClientBookingWithDetails(this ClientBooking clientBooking)
        {

            var result = new ClientBookingWithDetails()
            {
                Id = clientBooking.Id,
                Date = clientBooking.Date,
                StartTime = clientBooking.StartTime,
                EndTime = clientBooking.EndTime,
                Location = clientBooking.Location,
                Status = clientBooking.Status,
                ServiceName = clientBooking.Service.Name,
                UserId = clientBooking.UserId,
                BookingItems = clientBooking.BookingItems.ToBookingItemWIthDetails(),
                ServiceImages = clientBooking.Service.Images,

                // PaymentMethodName = clientBooking?.paymentTransaction.PaymentMethod.Name,
                // PaymentStatus = clientBooking?.paymentTransaction.Status,
                TotalCost = clientBooking.TotalCost
            };


            return result;
        }

        public static IEnumerable<ClientBookingDTO> ToClientBooking(this IEnumerable<ClientBooking> clientBooking)
        {
            var result = new List<ClientBookingDTO>();
            foreach (var booking in clientBooking)
            {
                var entity = booking.ToClientBooking();
                result.Add(entity);
            }
            return result;
        }
        public static ClientBookingDTO ToClientBooking(this ClientBooking clientBooking)
        {

            var result = new ClientBookingDTO()
            {
                Id = clientBooking.Id,
                Date = clientBooking.Date,
                StartTime = clientBooking.StartTime,
                EndTime = clientBooking.EndTime,
                Location = clientBooking.Location,
                Status = clientBooking.Status,
                ServiceName = clientBooking.Service.Name,
                TotalCost = clientBooking.TotalCost
            };


            return result;
        }

        public static IEnumerable<ScheduleItemGetDTO> ToScheduleItem(this IEnumerable<ScheduleItem> scheduleItems) 
        {
            var result = new List<ScheduleItemGetDTO>();
            foreach(var scheduleItem in scheduleItems)
            {
                var entity =scheduleItem.ToScheduleItem();
                result.Add(entity);
            }
            return result;
        }

        public static ScheduleItemGetDTO ToScheduleItem(this  ScheduleItem scheduleItem)
        {
            var result = new ScheduleItemGetDTO()
            {
                ScheduleId = scheduleItem.ScheduleId,
                Day = scheduleItem.Day,
                StartTime = scheduleItem.StartTime,
                EndTime = scheduleItem.EndTime,
                Available = scheduleItem.Available,
                Shift = scheduleItem.Shift,
                Name = scheduleItem.Schedule.Resource.Name,
                ImageUrls = scheduleItem.Schedule.Resource.Images
            };
            return result;
        }

        public static IEnumerable<BookingItemWIthDetails> ToBookingItemWIthDetails(this IEnumerable<BookingItem> bookingItems)
        {
            List<BookingItemWIthDetails> result = new();
            foreach (var bookingItem in bookingItems)
            {
                if (bookingItem.Resource.ResourceType.Shown == true)
                {
                    var entity = new BookingItemWIthDetails()
                    {
                        Price = bookingItem.Price,
                        ResourceId = bookingItem.ResourceId,
                        ResourceName = bookingItem.Resource.Name,
                        ResourceImages = bookingItem.Resource.Images,
                    };
                    result.Add(entity);
                }
                
            }

            return result;


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
