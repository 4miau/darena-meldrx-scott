//
//

//using System.ComponentModel.DataAnnotations;
//using Duende.IdentityServer.Models;
//using MeldRx.Sdk.Dtos;
//using MeldRx.Sdk.Enums;

//namespace MeldRx.Server.Configuration.Client
//{
//    public class ClientDto
//    {
//        public ClientDto()
//        {
//            IdentityResourceScopes = new List<string>();
//            ApiScopes = new List<string>();
//            AllowedScopes = new List<string>();
//            PostLogoutRedirectUris = new List<string>();
//            RedirectUris = new List<string>();
//            IdentityProviderRestrictions = new List<string>();
//            AllowedCorsOrigins = new List<string>();
//            AllowedGrantTypes = new List<string>();
//            AllowedIdentityTokenSigningAlgorithms = new List<string>();
//            Claims = new List<ClientClaimDto>();
//            ClientSecrets = new List<ClientSecretDto>();
//            Properties = new List<ClientPropertyDto>();
//        }
//        /// <summary>
//        /// Gets or sets the list of identity resource scopes allowed for this client
//        /// </summary>
//        public List<string> IdentityResourceScopes { get; set; }
//        public ClientType ClientType { get; set; }

//        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
//        public int AccessTokenLifetime { get; set; } = 3600;

//        public int? ConsentLifetime { get; set; }

//        public AccessTokenType AccessTokenType { get; set; }
//        public List<SelectItemDto> AccessTokenTypes { get; set; }

//        public bool AllowAccessTokensViaBrowser { get; set; }
//        public bool AllowOfflineAccess { get; set; }
//        public bool AllowPlainTextPkce { get; set; }
//        public bool AllowRememberConsent { get; set; } = true;
//        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
//        public bool AlwaysSendClientClaims { get; set; }
//        public int AuthorizationCodeLifetime { get; set; } = 300;
//        /// <summary>
//        /// Gets or sets the list of api scopes allowed for this client
//        /// </summary>
//        public List<string> ApiScopes { get; set; }

//        public string FrontChannelLogoutUri { get; set; }
//        public bool FrontChannelLogoutSessionRequired { get; set; } = true;
//        public string BackChannelLogoutUri { get; set; }
//        public bool BackChannelLogoutSessionRequired { get; set; } = true;

//        [Required]
//        public string ClientId { get; set; }

//        [Required]
//        public string ClientName { get; set; }

//        public string ClientUri { get; set; }

//        public string Description { get; set; }

//        public bool Enabled { get; set; } = true;
//        public bool EnableLocalLogin { get; set; } = true;
//        public int Id { get; set; }
//        public int IdentityTokenLifetime { get; set; } = 300;
//        public bool IncludeJwtId { get; set; }
//        public string LogoUri { get; set; }

//        public string ClientClaimsPrefix { get; set; } = "client_";

//        public string PairWiseSubjectSalt { get; set; }

//        public string ProtocolType { get; set; } = "oidc";

//        public List<SelectItemDto> ProtocolTypes { get; set; }

//        public TokenExpiration RefreshTokenExpiration { get; set; } = TokenExpiration.Absolute;
//        public List<SelectItemDto> RefreshTokenExpirations { get; set; }

//        public TokenUsage RefreshTokenUsage { get; set; } = TokenUsage.OneTimeOnly;
//        public List<SelectItemDto> RefreshTokenUsages { get; set; }

//        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

//        public bool RequireClientSecret { get; set; } = true;
//        public bool RequireConsent { get; set; } = true;
//        public bool RequirePkce { get; set; }
//        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

//        public List<string> PostLogoutRedirectUris { get; set; }
//        public string PostLogoutRedirectUrisItems { get; set; }

//        public List<string> IdentityProviderRestrictions { get; set; }
//        public string IdentityProviderRestrictionsItems { get; set; }

//        public List<string> RedirectUris { get; set; }
//        public string RedirectUrisItems { get; set; }

//        public List<string> AllowedCorsOrigins { get; set; }
//        public string AllowedCorsOriginsItems { get; set; }

//        public List<string> AllowedGrantTypes { get; set; }
//        public string AllowedGrantTypesItems { get; set; }

//        public List<string> AllowedScopes { get; set; }
//        public string AllowedScopesItems { get; set; }

//        public DateTime? Updated { get; set; }
//        public DateTime? LastAccessed { get; set; }

//        public int? UserSsoLifetime { get; set; }
//        public string UserCodeType { get; set; }
//        public int DeviceCodeLifetime { get; set; } = 300;

//        public int? CibaLifetime { get; set; }
//        public int? PollingInterval { get; set; }

//        public bool NonEditable { get; set; }

//        public bool RequireRequestObject { get; set; }

//        public List<string> AllowedIdentityTokenSigningAlgorithms { get; set; }

//        public string AllowedIdentityTokenSigningAlgorithmsItems { get; set; }

//        public List<ClientClaimDto> Claims { get; set; }
//        public List<ClientSecretDto> ClientSecrets { get; set; }
//        public List<ClientPropertyDto> Properties { get; set; }
//        /// <summary>
//        /// Gets or sets the date the client was created
//        /// </summary>
//        public DateTime CreatedOn { get; set; }
//        /// <summary>
//        /// Gets or sets the grant type for this client
//        /// </summary>
//        public string AllowedGrantType { get; set; }
//        /// <summary>
//        /// Gets or sets a secret for this client. The value of the secret will be hidden to the caller
//        /// </summary>
//        public SecretDto Secret { get; set; }
//    }
//}