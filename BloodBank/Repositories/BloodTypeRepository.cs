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
    public class BloodTypeRepository: IBloodTypeRepository
    {
        private readonly IAmazonDynamoDB _dynamoDbClient;

        // Initializing helper class
        public BloodTypeRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        // GET ALL
        public async Task<IEnumerable<BloodTypeInfo>> GetAllAsync(string id)
        {
            var request = new GetItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } },
           
        }
            };

            var result = await _dynamoDbClient.GetItemAsync(request);

            if (result.Item == null || !result.Item.Any()) return null;
            // Retrieve Area
            var area = result.Item.ContainsKey("Area") ? result.Item["Area"].S : null;

            var bloodTypesAttribute = result.Item["BloodTypes"].L; // List of maps
            return bloodTypesAttribute.Select(bt => new BloodTypeInfo
            {
                Type = bt.M["Type"].S,
                AvailabilityStatus = bt.M["AvailabilityStatus"].S,
                StockLevel = int.Parse(bt.M["StockLevel"].N),
                LastUpdated = DateTime.Parse(bt.M["LastUpdated"].S),
                Id = id,
                Area = area
                
            });
        }

        // GET BY ID
        public async Task<BloodTypeInfo> GetByIdAsync(string id, string bloodType)
        {
            var request = new GetItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } },
         
        }
            };

            var result = await _dynamoDbClient.GetItemAsync(request);

            if (result.Item == null || !result.Item.Any()) return null;
            // Retrieve Area
            var area = result.Item.ContainsKey("Area") ? result.Item["Area"].S : null;

            var bloodTypesAttribute = result.Item["BloodTypes"].L; // List of maps
            var bloodTypeMap = bloodTypesAttribute.FirstOrDefault(bt => bt.M["Type"].S == bloodType);

            if (bloodTypeMap == null) return null;

            return new BloodTypeInfo
            {
                Type = bloodTypeMap.M["Type"].S,
                AvailabilityStatus = bloodTypeMap.M["AvailabilityStatus"].S,
                StockLevel = int.Parse(bloodTypeMap.M["StockLevel"].N),
                LastUpdated = DateTime.Parse(bloodTypeMap.M["LastUpdated"].S),
                Id = id,
                Area = area
               

            };
        }

        // PUT
        public async Task UpdateBloodTypeAsync(string id, BloodTypeInfo newBloodType)
        {
            // Step 1: Fetch the existing item from DynamoDB
            var getItemRequest = new GetItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        }
            };

            var getItemResult = await _dynamoDbClient.GetItemAsync(getItemRequest);

            if (getItemResult.Item == null || !getItemResult.Item.ContainsKey("BloodTypes"))
            {
                throw new KeyNotFoundException("BloodTypes not found for the specified ID.");
            }

            // Step 2: Get the existing BloodTypes list
            var bloodTypesList = getItemResult.Item["BloodTypes"].L;

            // Step 3: Check if the blood type to update already exists
            bool bloodTypeExists = false;
            foreach (var bloodType in bloodTypesList)
            {
                var bloodTypeMap = bloodType.M;
                if (bloodTypeMap.ContainsKey("Type") && bloodTypeMap["Type"].S == newBloodType.Type)
                {
                    // Blood type exists, update it
                    bloodTypeExists = true;

                    // Update the fields you need (AvailabilityStatus, StockLevel, LastUpdated)
                    bloodTypeMap["AvailabilityStatus"] = new AttributeValue { S = newBloodType.AvailabilityStatus };
                    bloodTypeMap["StockLevel"] = new AttributeValue { N = newBloodType.StockLevel.ToString() };
                    bloodTypeMap["LastUpdated"] = new AttributeValue { S = newBloodType.LastUpdated.ToString("yyyy-MM-dd") };
                    break;
                }
            }

            // Step 4: If the blood type does not exist, append it to the list
            if (!bloodTypeExists)
            {
                bloodTypesList.Add(new AttributeValue
                {
                    M = new Dictionary<string, AttributeValue>
            {
                { "Type", new AttributeValue { S = newBloodType.Type } },
                { "AvailabilityStatus", new AttributeValue { S = newBloodType.AvailabilityStatus } },
                { "StockLevel", new AttributeValue { N = newBloodType.StockLevel.ToString() } },
                { "LastUpdated", new AttributeValue { S = newBloodType.LastUpdated.ToString("yyyy-MM-dd") } }
            }
                });
            }

            // Step 5: Update the item in DynamoDB with the modified BloodTypes list
            var updateRequest = new UpdateItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        },
                UpdateExpression = "SET BloodTypes = :bloodTypes",  
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":bloodTypes", new AttributeValue { L = bloodTypesList } }
        }
            };

            try
            {
                await _dynamoDbClient.UpdateItemAsync(updateRequest);
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine("DynamoDB exception: " + ex.Message);
                throw;
            }
        }


        // POST
        public async Task AddBloodTypeAsync(string id, BloodTypeInfo newBloodType)
        {
            // Step 1: Fetch the existing item from DynamoDB
            var getItemRequest = new GetItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        }
            };

            var getItemResult = await _dynamoDbClient.GetItemAsync(getItemRequest);

            if (getItemResult.Item == null || !getItemResult.Item.ContainsKey("BloodTypes"))
            {
                throw new KeyNotFoundException("BloodTypes not found for the specified ID.");
            }

            // Step 2: Get the existing BloodTypes list
            var bloodTypesList = getItemResult.Item["BloodTypes"].L;

            // Step 3: Check if the blood type to update already exists
            bool bloodTypeExists = false;
            foreach (var bloodType in bloodTypesList)
            {
                var bloodTypeMap = bloodType.M;
                if (bloodTypeMap.ContainsKey("Type") && bloodTypeMap["Type"].S == newBloodType.Type)
                {
                    // Blood type exists, update it
                    bloodTypeExists = true;

                    bloodTypeMap["AvailabilityStatus"] = new AttributeValue { S = newBloodType.AvailabilityStatus };
                    bloodTypeMap["StockLevel"] = new AttributeValue { N = newBloodType.StockLevel.ToString() };
                    bloodTypeMap["LastUpdated"] = new AttributeValue { S = newBloodType.LastUpdated.ToString("yyyy-MM-dd") };
                    break;
                }
            }

            // Step 4: If the blood type does not exist, append it to the list
            if (!bloodTypeExists)
            {
                bloodTypesList.Add(new AttributeValue
                {
                    M = new Dictionary<string, AttributeValue>
            {
                { "Type", new AttributeValue { S = newBloodType.Type } },
                { "AvailabilityStatus", new AttributeValue { S = newBloodType.AvailabilityStatus } },
                { "StockLevel", new AttributeValue { N = newBloodType.StockLevel.ToString() } },
                { "LastUpdated", new AttributeValue { S = newBloodType.LastUpdated.ToString("yyyy-MM-dd") } }
            }
                });
            }

            // Step 5: Update the item in DynamoDB with the modified BloodTypes list
            var updateRequest = new UpdateItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        },
                UpdateExpression = "SET BloodTypes = :bloodTypes",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":bloodTypes", new AttributeValue { L = bloodTypesList } }
        }
            };

            try
            {
                await _dynamoDbClient.UpdateItemAsync(updateRequest);
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine("DynamoDB exception: " + ex.Message);
                throw;
            }
        }


        // PATCH
        public async Task UpdateStockLevelAsync(string id, string bloodType, int newStockLevel)
        {
            // Step 1: Retrieve the current state of the donation center from the database
            var getItemRequest = new GetItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        }
            };

            var getItemResult = await _dynamoDbClient.GetItemAsync(getItemRequest);
            if (getItemResult.Item == null || !getItemResult.Item.ContainsKey("BloodTypes"))
            {
                throw new KeyNotFoundException($"Blood donation center with id {id} or BloodTypes not found.");
            }

            // Step 2: Deserialize the BloodTypes list from the DynamoDB item
            var bloodTypesList = getItemResult.Item["BloodTypes"].L;
            var bloodTypeInfo = bloodTypesList
                .FirstOrDefault(b => b.M["Type"].S == bloodType);

            if (bloodTypeInfo == null)
            {
                throw new KeyNotFoundException($"Blood type {bloodType} not found in donation center {id}.");
            }

            // Step 3: Update the StockLevel for the specified blood type
            bloodTypeInfo.M["StockLevel"].N = newStockLevel.ToString();
            bloodTypeInfo.M["LastUpdated"].S = DateTime.UtcNow.ToString("yyyy-MM-dd"); // Optionally update the LastUpdated timestamp

            // Step 4: Prepare the update expression and values for DynamoDB
            var updateExpression = "SET BloodTypes = :bloodTypes";
            var expressionAttributeValues = new Dictionary<string, AttributeValue>
    {
        { ":bloodTypes", new AttributeValue { L = bloodTypesList } }
    };

            // Step 5: Execute the update request to update the BloodTypes list in DynamoDB
            var updateRequest = new UpdateItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        },
                UpdateExpression = updateExpression,
                ExpressionAttributeValues = expressionAttributeValues
            };

            try
            {
                await _dynamoDbClient.UpdateItemAsync(updateRequest);
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine("DynamoDB exception: " + ex.Message);
                throw;
            }
        }


        // DELETE
        public async Task DeleteBloodTypeAsync(string id, string bloodType)
        {
            // Get the existing donation center record
            var getItemRequest = new GetItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        }
            };

            var getItemResult = await _dynamoDbClient.GetItemAsync(getItemRequest);

            if (getItemResult.Item == null || !getItemResult.Item.Any())
            {
                throw new KeyNotFoundException($"No donation center found with id: {id}");
            }

            // Extract the BloodTypes list
            var bloodTypes = getItemResult.Item["BloodTypes"].L;

            // Find and remove the blood type from the list
            var updatedBloodTypes = bloodTypes.Where(bt => bt.M["Type"].S != bloodType).ToList();

            // Prepare the update expression to remove the blood type
            var updateRequest = new UpdateItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        },
                UpdateExpression = "SET BloodTypes = :bloodTypes",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
        {
            { ":bloodTypes", new AttributeValue { L = updatedBloodTypes } }
        }
            };

            // Execute the update
            await _dynamoDbClient.UpdateItemAsync(updateRequest);
        }

    }


}
