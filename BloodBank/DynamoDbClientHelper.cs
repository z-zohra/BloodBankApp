using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BloodBank
{
    public static class DynamoDbClientHelper
    {
        public static IAmazonDynamoDB CreateClient(IConfiguration configuration)
        {

            var awsOptions = configuration.GetSection("AWS");
            var accessKey = awsOptions["Id"];
            var Key = awsOptions["Key"];
            var region = awsOptions["Region"];

            var credentials = new BasicAWSCredentials(accessKey, Key);
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
            };

            return new AmazonDynamoDBClient(credentials, config);
        }
    }
}
