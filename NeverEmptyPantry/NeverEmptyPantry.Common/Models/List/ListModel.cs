using System;
using NeverEmptyPantry.Common.Enum;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public OrderState OrderState { get; set; }
        public DateTime AuditDateTime { get; set; }

        public static ListModel FromList(Entity.List list) => new ListModel
        {
            Id = list.Id,
            Name = list.Name,
            StartDateTime = list.StartDateTime,
            EndDateTime = list.EndDateTime,
            OrderState = list.OrderState,
            AuditDateTime = list.AuditDateTime
        };
    }
}