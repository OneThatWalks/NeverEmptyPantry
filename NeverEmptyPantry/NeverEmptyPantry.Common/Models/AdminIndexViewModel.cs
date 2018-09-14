using System.Collections.Generic;
using NeverEmptyPantry.Common.Models.Account;

namespace NeverEmptyPantry.Common.Models
{
    public class AdminIndexViewModel
    {
        public string ActiveLists { get; set; }
        public string ProcessedLists { get; set; }
        public string DeliveredItems { get; set; }
        public IList<ProfileDto> Contributors { get; set; }
    }
}