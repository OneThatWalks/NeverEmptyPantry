using System;
using NeverEmptyPantry.Common.Enum;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListProductMap
    {
        public int Id { get; set; }
        public Entity.Product Product { get; set; }
        public Entity.List List { get; set; }
        public DateTime AddedDateTime { get; set; }
        public ListProductState ListProductState { get; set; }
        public DateTime AuditDateTime { get; set; }
        public bool Active { get; set; }

        public static ListProductMap FromProductAndList(Entity.Product product, Entity.List list) => new ListProductMap
        {
            Product = product,
            AddedDateTime = DateTime.UtcNow,
            AuditDateTime = DateTime.UtcNow,
            List = list,
            ListProductState = ListProductState.ITEM_ADDED
        };
        
    }
}