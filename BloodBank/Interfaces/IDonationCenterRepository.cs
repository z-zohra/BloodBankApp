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
        Task<DonationCenter> GetByIdAsync(string id, string area);
        //Task AddAsync(DonationCenter center);
        //Task UpdateAsync(string id, DonationCenter updatedCenter);
        //Task DeleteAsync(string id);
    }

}
