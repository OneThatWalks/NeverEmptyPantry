using System.ComponentModel.DataAnnotations;

namespace NeverEmptyPantry.Common.Models.Entity
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}