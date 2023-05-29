﻿
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public enum BookingStatus
    {
        Confirmed,
        Cancelled,
        Pending,
        Completed
    }
    public class ClientBooking: BaseEntity
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan Duration { get; set; }
        public string Location { get; set; }
        public BookingStatus Status { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
      
        public ICollection<BookingItem> BookingItems { get; set; } = new HashSet<BookingItem>();
    }
}
