using System.Collections.Generic;

namespace NeverEmptyPantry.Common.Models
{
    public class ProductIndexViewModel
    {
        public IList<ProductSelect> ProductSelects { get; set; }
        public int ListId { get; set; }
    }
}