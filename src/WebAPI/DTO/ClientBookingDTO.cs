﻿using Sieve.Attributes;

namespace WebAPI.DTO
{
    public class ClientBookingDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Location { get; set; }
        public BookingStatus Status { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public decimal TotalCost { get; set; }
        public string UserEmail { get; set; }
		public string UserID { get; set; }
		public string ServiceName { get; set; }
    }


}
