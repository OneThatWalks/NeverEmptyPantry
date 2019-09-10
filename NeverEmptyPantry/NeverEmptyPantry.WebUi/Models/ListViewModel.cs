using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NeverEmptyPantry.Common.Enum;
using NeverEmptyPantry.Common.Models;
using NeverEmptyPantry.Common.Models.List;
using NeverEmptyPantry.Common.Models.Vote;

namespace NeverEmptyPantry.WebUi.Models
{
    public class ListViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDateTime { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDateTime { get; set; }
        public OrderState OrderState { get; set; }
        [DataType(DataType.Date)]
        public DateTime AuditDateTime { get; set; }
        public IEnumerable<ListProductDto> ListProducts { get; set; } = new List<ListProductDto>();
        public IEnumerable<UserProductVoteDto> UserProductVotes { get; set; } = new List<UserProductVoteDto>();
    }
}