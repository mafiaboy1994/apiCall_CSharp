using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


class program
{
    static async Task Main(string[] args)
    {
        

        Console.Write("Enter ApplicationId (ClientId): ");
        string clientId = Console.ReadLine();
        Console.Write("Enter client Secret: ");
        string clientSecret = Console.ReadLine();
        Console.Write("Enter Tenant ID: ");
        string tenantId = Console.ReadLine();
        Console.Write("Enter subscription ID: ");
        string subscriptionId = Console.ReadLine();


        string token = await GetAccessToken(tenantId, clientId, clientSecret );
        await ListResources(subscriptionId, token);

    }

    static async Task<string> GetAccessToken(string tenantId, string clientId, string clientSecret)
    {
        using (var client = new HttpClient())
        {
            var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            var body = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", "https://management.azure.com/.default")
            });

            var response = await client.PostAsync(url, body);
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            return json["access_token"].ToString();

        }

    }

    static async Task ListResources(string subscriptionId, string token)
    {
        using (var client = new HttpClient())
        {
            var url = $"https://management.azure.com/subscriptions/{subscriptionId}/resources?api-version=2021-04-01";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);


            foreach(var resource in json["value"])
            {
                Console.WriteLine($"Resource: {resource["name"]}, Type: {resource["type"]}, Location: {resource["location"]}");
            }


        }
    }

}