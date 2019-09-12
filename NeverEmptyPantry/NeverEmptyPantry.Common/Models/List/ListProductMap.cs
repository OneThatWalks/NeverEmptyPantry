using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Interfaces.Entity;
using NeverEmptyPantry.Common.Models.Entity;

namespace NeverEmptyPantry.Common.Models.List
{
    public class ListProductMap : BaseEntity, IMergeableEntity<ListProductMap>
    {
        public Entity.Product Product { get; set; }
        public Entity.List List { get; set; }
        public ListProductState ListProductState { get; set; }

        public static ListProductMap FromProductAndList(Entity.Product product, Entity.List list) => new ListProductMap
        {
            Product = product,
            List = list,
            ListProductState = ListProductState.ITEM_ADDED
        };

        public void MergeProperties(ListProductMap updatedEntity)
        {
            base.MergeProperties(updatedEntity);
            Product = updatedEntity.Product;
            List = updatedEntity.List;
            ListProductState = updatedEntity.ListProductState;
        }
    }
}