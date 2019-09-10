using System;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListProductModel
    {
        public int Id { get; set; }
        public ProductModel Product { get; set; }
        public ListModel List { get; set; }
        public DateTime AddedDateTime { get; set; }
        public ListProductState ListProductState { get; set; }
        public DateTime AuditDateTime { get; set; }

        public static ListProductModel FromListProductMap(ListProductMap listProduct) => new ListProductModel
        {
            Id = listProduct.Id,
            Product = ProductModel.FromProduct(listProduct.Product),
            List = ListModel.FromList(listProduct.List),
            AddedDateTime = listProduct.AddedDateTime,
            ListProductState = listProduct.ListProductState,
            AuditDateTime = listProduct.AuditDateTime
        };
    }
}