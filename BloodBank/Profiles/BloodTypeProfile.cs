
using AutoMapper;
using BloodBank.DTOs;
using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Profiles
{
    public class BloodTypeProfile : Profile
    {
        public BloodTypeProfile()
        {

            CreateMap<DonationCenter, DonationCenterReadDto>();

            // // Mapping for BloodTypeUpdateDto
            CreateMap<BloodTypeUpdateDto, BloodTypeInfo>()
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.Now));

            
            CreateMap<BloodTypeInfo, BloodTypeReadDto>();

            CreateMap<BloodTypeCreateDto, BloodTypeInfo>()
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow));

        }
    }
}
