﻿using Sieve.Attributes;

namespace Domain.Entities
{
    public enum ServiceStatus
    {
        Active,
        Inactive,
        PendingApproval
    }
    public class Service : BaseEntity
    {
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
        public string Description { get; set; }
        public ServiceStatus Status { get; set; }
        public ICollection<ServiceMetadata> Metadata { get; set; } = new HashSet<ServiceMetadata>();
        public ICollection<ServiceImage> Images { get; set; }
    }
}
