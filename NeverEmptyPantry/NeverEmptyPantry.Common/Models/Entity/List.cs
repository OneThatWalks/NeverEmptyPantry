using System;
using NeverEmptyPantry.Common.Enum;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class List
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime AuditDateTime { get; set; }
        public OrderState OrderState { get; set; }
        public bool Active { get; set; }
    }
}