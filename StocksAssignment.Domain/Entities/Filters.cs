using StocksAssignment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.Domain.Entities
{
    public class Filters
    {
        public List<FuelType> FuelTypes { get; set; } = [];
        public int? MinBudgetLakhs { get; set;  }
        public int? MaxBudgetLakhs { get; set; }
        public int? CityId { get; set; }
        public List<int> MakeIds { get; set; } = [];
        public SortColumn SortColumn { get; set; } = SortColumn.Price;
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
    }
}
