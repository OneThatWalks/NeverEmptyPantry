using NeverEmptyPantry.Common.Models.Entity;
using System;

namespace NeverEmptyPantry.Common.Models.Product
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public int PackSize { get; set; }
        public string UnitSize { get; set; }
        public Category Category { get; set; }
        public string Image { get; set; }
        public DateTime AddedDateTime { get; set; }
        public bool Active { get; set; }

        public static ProductModel FromProduct(Entity.Product product)
        {
            return new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Brand = product.Brand,
                PackSize = product.PackSize,
                UnitSize = product.UnitSize,
                Category = product.Category,
                Image = product.Image,
                AddedDateTime = product.AddedDateTime,
                Active = product.Active
            };
        }
    }
}