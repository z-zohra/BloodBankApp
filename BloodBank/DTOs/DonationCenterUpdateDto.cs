using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.DTOs
{
   class DonationCenterUpdateDto
    {
        public string Id { get; }
        public string Area { get;}
        //public string AddressLine1 { get; set; }
        //public string AddressLine2 { get; set; }
        //public string PostalCode { get; set; }

        // Assuming this application will allow admins to only update the blood stock info and hours of operation in case there is a chnage because of some holiday 
        public Dictionary<string, string> HoursOfOperation { get; set; }
        public Dictionary<string, BloodTypeInfo> BloodTypes { get; set; }
    }
}
