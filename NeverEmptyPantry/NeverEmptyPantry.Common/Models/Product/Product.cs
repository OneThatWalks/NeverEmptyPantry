using System;
using NeverEmptyPantry.Common.Enum;

namespace NeverEmptyPantry.Common.Models.Product
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public int PackSize { get; set; }
        public string UnitSize { get; set; }
        public Category Category { get; set; }
        public string Image { get; set; }
        public DateTime AddedDateTime { get; set; }
        public bool Active { get; set; }
    }
}
