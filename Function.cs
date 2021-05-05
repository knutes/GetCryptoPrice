using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization;
using Amazon.DynamoDBv2.DocumentModel;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GetCoinPrice
{
    public class Coin
    {
        public string Symbol { get; set; }
        public double Price { get; set; }
    }
    public class Function
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private string tableName = "CryptoCoins";

        public async Task<Coin> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string symbol = "";
            Dictionary<string, string> dict = (Dictionary<string, string>)input.QueryStringParameters;
            dict.TryGetValue("symbol", out symbol);
            GetItemResponse res = await client.GetItemAsync(tableName, new Dictionary<string, AttributeValue>
            {
                {"symbol", new AttributeValue {S = symbol} }
            });
            Document doc = Document.FromAttributeMap(res.Item);
            Coin myItems = JsonConvert.DeserializeObject<Coin>(doc.ToJson());
            return myItems;
        }
    }
}
