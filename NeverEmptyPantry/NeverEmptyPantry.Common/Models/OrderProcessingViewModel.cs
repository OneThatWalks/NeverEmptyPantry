using System.Collections.Generic;
using NeverEmptyPantry.Common.Models.List;

namespace NeverEmptyPantry.Common.Models
{
    public class OrderProcessingViewModel
    {
        public int ListId { get; set; }
        public IList<ProductVoteGroup> ProductVoteGroups { get; set; }
    }
}