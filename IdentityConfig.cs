using System.Collections.Generic;
using IdentityServer4.Models;

namespace PodNoms.Api
{
    public static class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>{ new Client
            {
                ClientId = "podnoms_api_client",
                ClientName = "PodNoms API Client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowAccessTokensViaBrowser = true,
                AllowedCorsOrigins = new string[]{
                    "*",
                    "http://localhost:4200",
                    "http://dev.podnoms.com:4200",
                } ,

                ClientSecrets =
                {
                    new Secret("eba94d71-3ce7-4592-9435-37f786c0e79e".Sha256())
                },
                
                AllowedScopes =
                {
                    StandardScopes.OpenId.Name,
                    StandardScopes.Profile.Name,
                    StandardScopes.OfflineAccess.Name,
                    "podnoms_podcast_api"
                }
            }};
        }

        public static IEnumerable<Scope> GetScopes()
        {
            return new List<Scope>
            {
                new Scope
                {
                    Name = "podnoms_podcast_api",
                    Description = "PodNoms Podcast API"
                }
            };
        }
    }
}