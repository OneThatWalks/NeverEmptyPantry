using NeverEmptyPantry.Common.Interfaces;
using NeverEmptyPantry.Common.Interfaces.Entity;
using NeverEmptyPantry.Common.Interfaces.Repository;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class Product : BaseEntity, IMergeableEntity<Product>
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public int PackSize { get; set; }
        public string UnitSize { get; set; }
        public string Image { get; set; }
        public Category Category { get; set; }

        public void MergeProperties(Product updatedEntity)
        {
            base.MergeProperties(updatedEntity);
            Name = updatedEntity.Name;
            Brand = updatedEntity.Brand;
            PackSize = updatedEntity.PackSize;
            UnitSize = updatedEntity.UnitSize;
            Image = updatedEntity.Image;
            Category = updatedEntity.Category;
        }
    }
}
