using System;
using NeverEmptyPantry.Common.Enum;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListProduct
    {
        public int Id { get; set; }
        public Product.Product Product { get; set; }
        public List List { get; set; }
        public DateTime AddedDateTime { get; set; }
        public ListProductState ListProductState { get; set; }
        public DateTime AuditDateTime { get; set; }

        public static ListProduct From(Product.Product product, List list) => new ListProduct
        {
            Product = product,
            AddedDateTime = DateTime.UtcNow,
            AuditDateTime = DateTime.UtcNow,
            List = list,
            ListProductState = ListProductState.ITEM_ADDED
        };
        
    }
}