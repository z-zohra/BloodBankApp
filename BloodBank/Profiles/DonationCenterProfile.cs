using AutoMapper;
using BloodBank.DTOs;
using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BloodBank.Profiles
{
    public class DonationCenterProfile : Profile
    {
        public DonationCenterProfile()
        {
            CreateMap<DonationCenter, DonationCenterReadDto>();
            CreateMap<DonationCenterCreateDto, DonationCenter>();
        }
    }

}
