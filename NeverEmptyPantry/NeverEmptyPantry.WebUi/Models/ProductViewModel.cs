using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models.Entity;
using NeverEmptyPantry.Common.Models.Product;

namespace NeverEmptyPantry.WebUi.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Brand { get; set; }
        public int PackSize { get; set; }
        public string UnitSize { get; set; }
        [Required]
        public Category Category { get; set; }
        public SelectList Categories { get; set; }

        public string Image { get; set; }
        public DateTime AddedDateTime { get; set; }
        public bool Active { get; set; }
    }
}