using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using BloodBank.DTOs;
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

        // Initializing helper class
        public DonationCenterRepository(IAmazonDynamoDB dynamoDbClient)
        {
            _dynamoDbClient = dynamoDbClient;
        }

        // GETALL
        public async Task<IEnumerable<DonationCenter>> GetAllAsync()
        {
            var scanRequest = new ScanRequest
            {
                TableName = "Blood_Donation_Centers"
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
                BloodTypes = item["BloodTypes"].L.Select(b => new BloodTypeInfo 
                { 
                    Type = b.M["Type"].S, AvailabilityStatus = b.M["AvailabilityStatus"].S, 
                    StockLevel = int.Parse(b.M["StockLevel"].N), 
                    LastUpdated = DateTime.Parse(b.M["LastUpdated"].S),
                    Id = item["Id"].S,
                    Area = item["Area"].S
                }).ToList()
            });
        }

        // GET BY ID
        public async Task<DonationCenter> GetByIdAsync(string id)
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
            if (result.Item == null || !result.Item.Any())
            {
                
                throw new KeyNotFoundException($"No item found with Id: {id}");
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
                BloodTypes = result.Item["BloodTypes"].L.Select(b => new BloodTypeInfo
                {
                    Type = b.M["Type"].S,
                    AvailabilityStatus = b.M["AvailabilityStatus"].S,
                    StockLevel = int.Parse(b.M["StockLevel"].N),
                    LastUpdated = DateTime.Parse(b.M["LastUpdated"].S),
                    Id = result.Item["Id"].S,
                    Area = result.Item["Area"].S
                }).ToList()

            };
        }

        // POST
        public async Task AddAsync(DonationCenter donationCenter)
        {
            var item = new Dictionary<string, AttributeValue>
    {
        { "Id", new AttributeValue { S = donationCenter.Id } },
        { "Area", new AttributeValue { S = donationCenter.Area } },
        { "Address_Line_1", new AttributeValue { S = donationCenter.AddressLine1 } },
        { "Address_Line_2", new AttributeValue { S = donationCenter.AddressLine2 } },
        { "Postal_Code", new AttributeValue { S = donationCenter.PostalCode } },
        { "Hours_of_Operation", new AttributeValue
            {
                M = donationCenter.HoursOfOperation.ToDictionary(
                    x => x.Key,
                    x => new AttributeValue { S = x.Value }
                )
            }
        },
        { "BloodTypes", new AttributeValue
            {
                L = donationCenter.BloodTypes?.Select(bt => new AttributeValue
                {
                    M = new Dictionary<string, AttributeValue>
                    {
                        { "Type", new AttributeValue { S = bt.Type } },
                        { "AvailabilityStatus", new AttributeValue { S = bt.AvailabilityStatus } },
                        { "StockLevel", new AttributeValue { N = bt.StockLevel.ToString() } },
                        { "LastUpdated", new AttributeValue { S = bt.LastUpdated.ToString("yyyy-MM-dd") } }
                    }
                }).ToList() ?? new List<AttributeValue>() // Initialize as empty list if null
            }
        }
    };

            var request = new PutItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Item = item
            };

            await _dynamoDbClient.PutItemAsync(request);
        }

        // PUT 
        public async Task UpdateAsync(string id, DonationCenterUpdateDto updateDto)
        {
            // Ensure that the Id in the DTO matches the Id in the URL
            if (id != updateDto.Id)
            {
                throw new ArgumentException("Id in the URL does not match Id in the body.");
            }

            // Initialize the update expression and attribute values
            var updateExpression = new StringBuilder("SET ");
            var expressionAttributeValues = new Dictionary<string, AttributeValue>();

            // Update Area
            if (!string.IsNullOrEmpty(updateDto.Area))
            {
                updateExpression.Append("Area = :area, ");
                expressionAttributeValues[":area"] = new AttributeValue { S = updateDto.Area };
            }

            // Update Address fields
            if (!string.IsNullOrEmpty(updateDto.AddressLine1))
            {
                updateExpression.Append("Address_Line_1 = :addressLine1, ");
                expressionAttributeValues[":addressLine1"] = new AttributeValue { S = updateDto.AddressLine1 };
            }

            if (!string.IsNullOrEmpty(updateDto.AddressLine2))
            {
                updateExpression.Append("Address_Line_2 = :addressLine2, ");
                expressionAttributeValues[":addressLine2"] = new AttributeValue { S = updateDto.AddressLine2 };
            }

            if (!string.IsNullOrEmpty(updateDto.PostalCode))
            {
                updateExpression.Append("Postal_Code = :postalCode, ");
                expressionAttributeValues[":postalCode"] = new AttributeValue { S = updateDto.PostalCode };
            }

            // Update Hours of Operation
            if (updateDto.HoursOfOperation != null && updateDto.HoursOfOperation.Count > 0)
            {
                updateExpression.Append("Hours_of_Operation = :hoursOfOperation, ");
                expressionAttributeValues[":hoursOfOperation"] = new AttributeValue
                {
                    M = updateDto.HoursOfOperation.ToDictionary(
                        x => x.Key,
                        x => new AttributeValue { S = x.Value }
                    )
                };
            }

            // Update BloodTypes
            if (updateDto.BloodTypes != null && updateDto.BloodTypes.Count > 0)
            {
                updateExpression.Append("BloodTypes = :bloodTypes, ");
                expressionAttributeValues[":bloodTypes"] = new AttributeValue
                {
                    L = updateDto.BloodTypes.Select(bt => new AttributeValue
                    {
                        M = new Dictionary<string, AttributeValue>
                {
                    { "Type", new AttributeValue { S = bt.Type } },
                    { "AvailabilityStatus", new AttributeValue { S = bt.AvailabilityStatus } },
                    { "StockLevel", new AttributeValue { N = bt.StockLevel.ToString() } },
                    { "LastUpdated", new AttributeValue { S = bt.LastUpdated.ToString("yyyy-MM-dd") } }
                }
                    }).ToList()
                };
            }

            // If no fields to update, throw an exception
            if (expressionAttributeValues.Count == 0)
            {
                throw new ArgumentException("No valid fields to update.");
            }

            // Remove trailing comma and space from the update expression
            updateExpression.Length -= 2;

            // Build the update item request
            var updateRequest = new UpdateItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        },
                UpdateExpression = updateExpression.ToString(),
                ExpressionAttributeValues = expressionAttributeValues
            };

            try
            {
                // Execute the update request
                await _dynamoDbClient.UpdateItemAsync(updateRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating donation center with Id '{id}': {ex.Message}", ex);
            }
        }


        // PATCH 
        public async Task UpdateHoursAsync(string id, DonationCenterUpdateHoursDto updateHoursDto)
        {
            //  input validation
            if (updateHoursDto?.HoursOfOperation == null || !updateHoursDto.HoursOfOperation.Any())
            {
                throw new ArgumentException("Hours of Operation cannot be null or empty.");
            }

            //update expression and attribute values
            var updateExpression = "SET Hours_of_Operation = :hoursOfOperation";
            var expressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                [":hoursOfOperation"] = new AttributeValue
                {
                    M = updateHoursDto.HoursOfOperation.ToDictionary(
                        x => x.Key,
                        x => new AttributeValue { S = x.Value }
                    )
                }
            };

            // condition to ensure the Id exists
            var conditionExpression = "attribute_exists(Id)";

            // UpdateItem request
            var updateRequest = new UpdateItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        },
                UpdateExpression = updateExpression,
                ExpressionAttributeValues = expressionAttributeValues,
                ConditionExpression = conditionExpression // Ensures the item exists
            };

            try
            {
                // Execute the update request
                await _dynamoDbClient.UpdateItemAsync(updateRequest);
            }
            catch (ConditionalCheckFailedException)
            {
                throw new KeyNotFoundException($"Donation center with Id '{id}' does not exist.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating Hours of Operation for Id '{id}': {ex.Message}", ex);
            }
        }


        // DELETE
        public async Task DeleteAsync(string id)
        {
            // Create a DeleteItem request
            var request = new DeleteItemRequest
            {
                TableName = "Blood_Donation_Centers",
                Key = new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue { S = id } }
        }
            };

            // Execute the request
            try
            {
                await _dynamoDbClient.DeleteItemAsync(request);
            }
            catch (ResourceNotFoundException)
            {
                throw new KeyNotFoundException($"Donation center with Id '{id}' not found.");
            }
            catch (Exception ex)
            {
                // Log and rethrow if needed
                throw new Exception($"Error deleting donation center with Id '{id}': {ex.Message}", ex);
            }
        }



    }

}
