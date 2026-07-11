using StocksAssignment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.Domain.Entities
{
    public class Stock
    {
        public int Id { get; set; }

        public string MakeName { get; set; } = string.Empty;
        public int MakeId { get; set; }

        public string ModelName { get; set; } = string.Empty;
        public int ModelId { get; set; }

        public int CityId { get; set; }

        public FuelType FuelType { get; set; }

        public int RegistrationYear { get; set; }

        public int KilometersDriven { get; set; }

        public decimal Price { get; set; }
    }
}
