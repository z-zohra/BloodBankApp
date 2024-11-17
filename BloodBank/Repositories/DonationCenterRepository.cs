using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BloodBank.Interfaces;
using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Repositories
{
    public class DonationCenterRepository : IDonationCenterRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;

        public DonationCenterRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        public async Task<IEnumerable<DonationCenter>> GetAllAsync()
        {
            var scanRequest = new ScanRequest
            {
                TableName = "Blood_Donation_Centers_ON"
            };

            var result = await _dynamoDbClient.ScanAsync(scanRequest);

            return result.Items.Select(item => new DonationCenter
            {
                Id = item["Id"].S,
                Area = item["Area"].S,
                AddressLine1 = item["Address_Line_1"].S,
                AddressLine2 = item["Address_Line_2"].S,
                PostalCode = item["Postal_Code"].S,
                HoursOfOperation = item["Hours_of_Operation"].M.ToDictionary(
                    x => x.Key,
                    x => x.Value.S),
                BloodTypes = item.Where(kv => kv.Key.StartsWith("Blood_Type"))
                                 .GroupBy(kv => kv.Key.Split('_')[2]) // Extract blood type
                                 .ToDictionary(
                                     g => g.Key,
                                     g => new BloodTypeInfo
                                     {
                                         AvailabilityStatus = g.First(k => k.Key.EndsWith("Availability_Status")).Value.S,
                                         StockLevel = int.Parse(g.First(k => k.Key.EndsWith("Stock_Level")).Value.N),
                                         LastUpdated = DateTime.Parse(g.First(k => k.Key.EndsWith("Last_Updated")).Value.S)
                                     })
            });
        }
        public async Task<DonationCenter> GetByIdAsync(string id, string area)
        {
            var request = new GetItemRequest
            {
                TableName = "Blood_Donation_Centers_ON",
                Key = new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue { S = id } },
                    { "Area", new AttributeValue { S = area } }
                }
            };

            var result = await _dynamoDbClient.GetItemAsync(request);
            if (result.Item == null || !result.Item.Any())
            {
                throw new KeyNotFoundException($"No item found with Id: {id} and Area: {area}");
            }

            return new DonationCenter
            {
                Id = result.Item["Id"].S,
                Area = result.Item["Area"].S,
                AddressLine1 = result.Item["Address_Line_1"].S,
                AddressLine2 = result.Item["Address_Line_2"].S,
                PostalCode = result.Item["Postal_Code"].S,
                HoursOfOperation = result.Item["Hours_of_Operation"].M.ToDictionary(
                    x => x.Key,
                    x => x.Value.S),
                BloodTypes = result.Item.Where(kv => kv.Key.StartsWith("Blood_Type"))
                                 .GroupBy(kv => kv.Key.Split('_')[2]) // Extract blood type
                                 .ToDictionary(
                                     g => g.Key,
                                     g => new BloodTypeInfo
                                     {
                                         AvailabilityStatus = g.First(k => k.Key.EndsWith("Availability_Status")).Value.S,
                                         StockLevel = int.Parse(g.First(k => k.Key.EndsWith("Stock_Level")).Value.N),
                                         LastUpdated = DateTime.Parse(g.First(k => k.Key.EndsWith("Last_Updated")).Value.S)
                                     })
                // Map other properties as needed
            };
        }



        // Other methods like GetByIdAsync, AddAsync, etc.
    }

}
