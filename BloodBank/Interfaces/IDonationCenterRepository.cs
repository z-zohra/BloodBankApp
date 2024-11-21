using BloodBank.DTOs;
using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Interfaces
{
    public interface IDonationCenterRepository
    {
        Task<IEnumerable<DonationCenter>> GetAllAsync();
        Task<DonationCenter> GetByIdAsync(string id);
        Task AddAsync(DonationCenter center);
        Task UpdateAsync(string id, DonationCenterUpdateDto updateDto);
        Task UpdateHoursAsync(string id, DonationCenterUpdateHoursDto updateHoursDto);
        Task DeleteAsync(string id);

       
    }

}
