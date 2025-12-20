using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace PetStuff.IdentityServer.Configuration
{
    public static class IdentityServerConfiguration
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("catalog.api", "Catalog API"),
                new ApiScope("basket.api", "Basket API"),
                new ApiScope("order.api", "Order API"),
                new ApiScope("inventory.api", "Inventory API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // Swagger/API Testing Client
                new Client
                {
                    ClientId = "swagger",
                    ClientName = "Swagger Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("swagger_secret".Sha256()) },
                    AllowedScopes = { 
                        "catalog.api",
                        "basket.api",
                        "order.api",
                        "inventory.api"
                    }
                },
                // Resource Owner Password Client (for user login)
                new Client
                {
                    ClientId = "petstuff.client",
                    ClientName = "PetStuff Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("petstuff_secret".Sha256()) },
                    AllowedScopes = { 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "catalog.api",
                        "basket.api",
                        "order.api",
                        "inventory.api"
                    },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 3600 // 1 hour
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("catalog.api", "Catalog API")
                {
                    Scopes = { "catalog.api" }
                },
                new ApiResource("basket.api", "Basket API")
                {
                    Scopes = { "basket.api" }
                },
                new ApiResource("order.api", "Order API")
                {
                    Scopes = { "order.api" }
                },
                new ApiResource("inventory.api", "Inventory API")
                {
                    Scopes = { "inventory.api" }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
    }
}
