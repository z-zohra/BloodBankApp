using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Interfaces
{
    public interface IBloodTypeRepository
    {
        Task<IEnumerable<BloodTypeInfo>> GetAllAsync(string id);
        Task<BloodTypeInfo> GetByIdAsync(string id, string bloodType);
        
        Task UpdateBloodTypeAsync(string id, BloodTypeInfo newBloodType);

        Task AddBloodTypeAsync(string id, BloodTypeInfo newBloodType);

        Task UpdateStockLevelAsync(string id, string bloodType, int newStockLevel);

        Task DeleteBloodTypeAsync(string id,  string bloodType);
    }
}
