using System;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListProductDto
    {
        public int Id { get; set; }
        public ProductDto Product { get; set; }
        public ListDto List { get; set; }
        public DateTime AddedDateTime { get; set; }
        public ListProductState ListProductState { get; set; }
        public DateTime AuditDateTime { get; set; }

        public static ListProductDto From(ListProduct listProduct) => new ListProductDto
        {
            Id = listProduct.Id,
            Product = ProductDto.From(listProduct.Product),
            List = ListDto.From(listProduct.List),
            AddedDateTime = listProduct.AddedDateTime,
            ListProductState = listProduct.ListProductState,
            AuditDateTime = listProduct.AuditDateTime
        };
    }
}