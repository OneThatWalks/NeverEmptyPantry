using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces.Entity;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class ListProduct : BaseEntity, IMergeableEntity<ListProduct>
    {
        public Product Product { get; set; }
        public List List { get; set; }
        public ListProductState ListProductState { get; set; }

        public static ListProduct FromProductAndList(Product product, List list) => new ListProduct
        {
            Product = product,
            List = list,
            ListProductState = ListProductState.ITEM_ADDED
        };

        public void MergeProperties(ListProduct updatedEntity)
        {
            base.MergeProperties(updatedEntity);
            Product = updatedEntity.Product;
            List = updatedEntity.List;
            ListProductState = updatedEntity.ListProductState;
        }
    }
}