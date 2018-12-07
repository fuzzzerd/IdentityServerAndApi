using System;
using Xunit;
using IdentityModel.Client;
using IdentityServerHost;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace IntegrationTest
{
    public class TestIntegrationTest
    {
        [Fact]
        public async Task Test1()
        {
            // arrange
            var server = new TestServer(Program.CreateWebHostBuilder(new string[] { }));

            var handler = server.CreateHandler();
            Startup.Handler = handler;
            var client = new HttpClient(handler);
            client.BaseAddress = server.BaseAddress;

            // discover endpoints from metadata
            var disco = await client.GetDiscoveryDocumentAsync();
            if (disco.IsError) Assert.True(false);

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "spa",
                ClientSecret = "test1",
                Scope = "api1"
            });

            if (tokenResponse.IsError) Assert.True(false);

            client.SetBearerToken(tokenResponse.AccessToken);

            // act
            var response = await client.GetAsync("test");

            string responseString = null;
            if (response.Content != null)
            {
                responseString = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine(responseString);
            }

            response.EnsureSuccessStatusCode();

            // assert
            Assert.NotNull(responseString);
        }
    }
}