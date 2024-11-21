using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.DTOs
{
    // For reading data
    public class DonationCenterReadDto
    {
        public string Id { get; set; }
        public string Area { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostalCode { get; set; }
        public Dictionary<string, string> HoursOfOperation { get; set; }
        public List<BloodTypeInfo> BloodTypes { get; set; }

    }
}
