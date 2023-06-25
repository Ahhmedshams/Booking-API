using AutoMapper;
using Domain.Common;
using Domain.Enums;
using Domain.Identity;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.DTO
{
    public class ClientBookingWithDetails
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public BookingStatus Status { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string UserId { get; set; }
        public IEnumerable<BookingItemWIthDetails> BookingItems { get; set; }
        public ICollection<ServiceImage> ServiceImages { get; set; }
        public string PaymentMethodName { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal TotalCost { get; set; }

    }


    public class BookingItemWIthDetails
    {
        public decimal Price { get; set; }
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
    }

   



}
