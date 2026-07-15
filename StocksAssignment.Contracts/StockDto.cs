using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksAssignment.Contracts
{
    public class StockDto
    {
        public int Id { get; set; }

        public int MakeId { get; set; }

        public string MakeName { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;

        public int RegistrationYear { get; set; }

        public string FuelType { get; set; } = string.Empty;

        public long Price { get; set; }

        public int KilometersDriven { get; set; }

        public int CityId { get; set; }

        public string CarName { get; set; } = string.Empty;

        public string FormattedPrice { get; set; } = string.Empty;

        public bool IsValueForMoney { get; set; }

        public List<string> ImageUrls { get; set; } = new();
    }
}
