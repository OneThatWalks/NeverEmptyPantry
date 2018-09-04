using System.Collections.Generic;

namespace NeverEmptyPantry.WebUi.Models
{
    public class PantryViewModel
    {
        public IEnumerable<ListViewModel> ActiveLists { get; set; }
        public IEnumerable<ListViewModel> ListViewModels { get; set; }
    }
}