using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.DTOs
{
     public class BloodTypeCreateDto
    {
        public string Id { get; set; }
        public string Area { get; set; }
        public string Type { get; set; }
        public string AvailabilityStatus { get; set; }
        public int StockLevel { get; set; }
        public DateTime LastUpdated { get; set; }
       
    }
}
