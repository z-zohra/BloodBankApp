using BloodBank.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Models
{
    public class DonationCenter
    {
        public string Id { get; set; }
        public string Area { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostalCode { get; set; }
        public List<BloodTypeInfo> BloodTypes { get; set; }
        public Dictionary<string, string> HoursOfOperation { get; set; }
       // public List<BloodTypeCreateDto> BloodTypes { get; set; }
    }

   

}
