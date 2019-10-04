using System.ComponentModel.DataAnnotations;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class Product : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Brand { get; set; }

        public int PackSize { get; set; }

        public string UnitSize { get; set; }

        public string Image { get; set; }

        public Category Category { get; set; }

        public override void MergeProperties<T>(T updatedEntity)
        {
            base.MergeProperties(updatedEntity);


            if (updatedEntity is Product product)
            {
                Name = product.Name;
                Brand = product.Brand;
                PackSize = product.PackSize;
                UnitSize = product.UnitSize;
                Image = product.Image;
                Category = product.Category;
            }
        }
    }
}
