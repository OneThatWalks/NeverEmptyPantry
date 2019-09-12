using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces.Entity;
using System;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class List : BaseEntity, IMergeableEntity<List>
    {
        public string Name { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public OrderState OrderState { get; set; }

        public void MergeProperties(List updatedEntity)
        {
            base.MergeProperties(updatedEntity);
            Name = updatedEntity.Name;
            StartDateTime = updatedEntity.StartDateTime;
            EndDateTime = updatedEntity.EndDateTime;
            OrderState = updatedEntity.OrderState;
        }
    }
}