using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using LtiAdvantage;
using Newtonsoft.Json.Linq;

namespace EduManagementLab.IdentityServer
{
    public static class Config
    {
        private static JObject jsonObj = JObject.Parse(File.ReadAllText(@"C:\Users\V\source\repos\EduManagementLab_Latest\src\EduManagementLab.IdentityServer4\Data\PublicKey.json"));

        public static string publicKey = jsonObj.SelectToken("publicKey").ToString();
        public static IEnumerable<IdentityResource> IdentityResources => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource
            {
                Name = "role",
                UserClaims = new List<string> {"role"}
            }
        };
        public static IEnumerable<ApiScope> ApiScopes => new List<ApiScope>
        {
            //used to specify what actions authorized user can perform at the level of the API
             new ApiScope("eduManagementLabApi.read", "Read Access to EduManagementLab API"),
             new ApiScope("eduManagementLabApi.write", "Write Access to EduManagementLab API"),
        };
        public static IEnumerable<Client> Clients => new List<Client>
        {
            new Client
            {
                //OAuth2 
                ClientId = "eduManagementLabApi",
                ClientName = "ASP.NET Core EduManagementLab Api",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> {new Secret("TestEduApi".Sha256())},
                AllowedScopes = new List<string> {"eduManagementLabApi.read"},
                AllowedCorsOrigins = new List<string>
                {
                    "https://localhost:5002",
                    "https://localhost:5001",
                    "https://localhost:7187"
                }
            },
            new Client
            {
                //Tool Client
                ClientId = "IMSTool",
                ClientName = "EduLabTool",
                AllowedGrantTypes = GrantTypes.ImplicitAndClientCredentials,
                ClientSecrets = new List<Secret>
                {
                    new Secret
                    {
                        Type = Core.Validation.Constants.SecretTypes.PublicPemKey,
                        Value = publicKey
                    }
                },
                AllowedScopes = LtiScopes,
                RedirectUris = { "https://localhost:44308/Tool/0300e6d518c41de7" },
                RequireConsent = false,
            },
            new Client
            {
                //OpenID Connect IDS
                ClientId = "oidcEduWebApp",
                ClientName = "ASP.NET Core EduManagementLab Web",
                ClientSecrets =  {new Secret("TestEduApi".Sha256())},
                AllowedGrantTypes = GrantTypes.Code,
                AllowOfflineAccess = true,
                RedirectUris = {"https://localhost:5002/signin-oidc"},
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile
                }
            }
        };
        public static IEnumerable<ApiResource> ApiResources => new List<ApiResource>
        {
            //used to define the API that the identity server is protecting 
            new ApiResource
            {
                Name = "eduManagementLabApi",
                DisplayName = "EduManagementLab Api",
                Description = "Allow the application to access EduManagementLab Api on your behalf",
                Scopes = new List<string> { "eduManagementLabApi.read", "eduManagementLabApi.write"},
                ApiSecrets = new List<Secret> {new Secret("TestEduApi".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };
        public static ICollection<string> LtiScopes => new[]
{
            OidcConstants.StandardScopes.OpenId,
            Constant.LtiScopes.Ags.LineItem,
            Constant.LtiScopes.Ags.LineItemReadonly,
            Constant.LtiScopes.Ags.ResultReadonly,
            Constant.LtiScopes.Ags.Score,
            Constant.LtiScopes.Ags.ScoreReadonly,
            Constant.LtiScopes.Nrps.MembershipReadonly
        };
    }
}
