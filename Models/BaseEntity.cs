using System;

namespace LoginRegEntity.Models
{
    public abstract class BaseEntity
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}