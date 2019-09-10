using System.Collections.Generic;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Vote;

namespace NeverEmptyPantry.Common.Models
{
    public class OrderProcessingViewModel
    {
        public int ListId { get; set; }
        public IList<ProductVoteGroup> ProductVoteGroups { get; set; }
    }
}