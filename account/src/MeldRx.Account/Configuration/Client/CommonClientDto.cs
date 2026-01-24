//using System.ComponentModel.DataAnnotations;
//using Duende.IdentityServer.Models;
//using MeldRx.Sdk.Dtos;

//namespace MeldRx.Server.Configuration.Client
//{
//    /// <summary>
//    /// The model that contains the common client data used by the create and modify models
//    /// </summary>
//    public class CommonClientDto
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="CommonClientDto"/> class
//        /// </summary>
//        protected CommonClientDto()
//        {
//            RedirectUris = new List<string>();
//            PostLogoutRedirectUris = new List<string>();
//            IdentityResourceScopes = new List<string>();
//            ApiScopes = new List<string>();
//            AllowedCorsOrigins = new List<string>();
//            Properties = new List<PropertyDto>();
//        }

//        /// <summary>
//        /// Gets or sets a value indicating whether the client is enabled
//        /// </summary>
//        [Required]
//        public bool Enabled { get; set; } = true;

//        /// <summary>
//        /// Gets or sets a secret for this client
//        /// </summary>
//        public SecretDto Secret { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether a client secret is required at the token endpoint
//        /// </summary>
//        public bool RequireClientSecret { get; set; } = true;

//        /// <summary>
//        /// Gets or sets the client display name
//        /// </summary>
//        [Required]
//        [StringLength(200)]
//        public string ClientName { get; set; }

//        /// <summary>
//        /// Gets or sets the description of the client.
//        /// </summary>
//        [StringLength(1000)]
//        public string Description { get; set; }

//        /// <summary>
//        /// Gets or sets a URI that contains further information about client
//        /// </summary>
//        [StringLength(2000)]
//        public string ClientUri { get; set; }

//        /// <summary>
//        /// Gets or sets the URI to the client logo
//        /// </summary>
//        [StringLength(2000)]
//        public string LogoUri { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether a consent screen is required
//        /// </summary>
//        public bool RequireConsent { get; set; } = false;

//        /// <summary>
//        /// Gets or sets a value indicating whether a user can choose to store consent decisions
//        /// </summary>
//        public bool AllowRememberConsent { get; set; } = true;

//        /// <summary>
//        /// Gets or sets the grant type for this client
//        /// </summary>
//        [Required]
//        public string AllowedGrantType { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether a proof key is required for authorization code based token requests
//        /// </summary>
//        public bool RequirePkce { get; set; } = true;

//        /// <summary>
//        /// Gets or sets a value indicating whether a proof key can be sent using plain method
//        /// </summary>
//        public bool AllowPlainTextPkce { get; set; } = false;

//        /// <summary>
//        /// Gets or sets a value indicating whether the client must use a request object on authorize requests
//        /// </summary>
//        public bool RequireRequestObject { get; set; } = false;

//        /// <summary>
//        /// Gets or sets a value indicating whether whether access tokens are transmitted via the browser for this client
//        /// This can prevent accidental leakage of access tokens when multiple response types are allowed
//        /// </summary>
//        public bool AllowAccessTokensViaBrowser { get; set; } = false;

//        /// <summary>
//        /// Gets or sets the list of URIs to return tokens or authorization codes to
//        /// </summary>
//        public List<string> RedirectUris { get; set; }

//        /// <summary>
//        /// Gets or sets the list of allowed URIs to redirect to after logout
//        /// </summary>
//        public List<string> PostLogoutRedirectUris { get; set; }

//        /// <summary>
//        /// Gets or sets the logout URI at client for HTTP front-channel based logout
//        /// </summary>
//        [StringLength(2000)]
//        public string FrontChannelLogoutUri { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether the user's session id should be sent to the <see cref="FrontChannelLogoutUri"/>
//        /// </summary>
//        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

//        /// <summary>
//        /// Gets or sets the logout URI at client for HTTP back-channel based logout
//        /// </summary>
//        [StringLength(2000)]
//        public string BackChannelLogoutUri { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether the user's session id should be sent to the <see cref="BackChannelLogoutUri"/>
//        /// </summary>
//        public bool BackChannelLogoutSessionRequired { get; set; } = true;

//        /// <summary>
//        /// Gets or sets a value indicating whether offline access is allowed (refresh tokens)
//        /// </summary>
//        public bool AllowOfflineAccess { get; set; } = false;

//        /// <summary>
//        /// Gets or sets the list of identity resource scopes allowed for this client
//        /// </summary>
//        public List<string> IdentityResourceScopes { get; set; }

//        /// <summary>
//        /// Gets or sets the list of api scopes allowed for this client
//        /// </summary>
//        public List<string> ApiScopes { get; set; }

//        /// <summary>
//        /// Gets or sets a value indicating whether the user claims should always be added to the id token instead of requiring
//        /// the client to use the userinfo endpoint
//        /// </summary>
//        public bool AlwaysIncludeUserClaimsInIdToken { get; set; } = false;

//        /// <summary>
//        /// Gets or sets the lifetime of an identity token in seconds
//        /// </summary>
//        [Range(1, int.MaxValue, ErrorMessage = "The identity token lifetime must be greater than 0")]
//        public int IdentityTokenLifetime { get; set; } = 300;

//        /// <summary>
//        /// Gets or sets the lifetime of an access token in seconds
//        /// </summary>
//        [Range(1, int.MaxValue, ErrorMessage = "The access token lifetime must be greater than 0")]
//        public int AccessTokenLifetime { get; set; } = 3600;

//        /// <summary>
//        /// Gets or sets the lifetime of an authorization code in seconds
//        /// </summary>
//        [Range(120, 600, ErrorMessage = "The authorization code lifetime must be between 120 and 600 seconds (2 mins and 10 mins)")]
//        public int AuthorizationCodeLifetime { get; set; } = 300;

//        /// <summary>
//        /// Gets or sets the maximum lifetime of a refresh token in seconds
//        /// </summary>
//        [Range(1, int.MaxValue, ErrorMessage = "The absolute refresh token lifetime must be greater than 0")]
//        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

//        /// <summary>
//        /// Gets or sets the sliding lifetime of a refresh token in seconds
//        /// </summary>
//        [Range(1, int.MaxValue, ErrorMessage = "The sliding refresh token lifetime must be greater than 0")]
//        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

//        /// <summary>
//        /// Gets or sets the lifetime of a user consent in seconds. If <c>null</c>, then it means there is no expiration
//        /// and the lifetime is infinite
//        /// </summary>
//        public int? ConsentLifetime { get; set; } = null;

//        /// <summary>
//        /// Gets or sets the refresh token usage type
//        /// </summary>
//        public TokenUsage RefreshTokenUsage { get; set; } = TokenUsage.OneTimeOnly;

//        /// <summary>
//        /// Gets or sets a value indicating whether the access token (and its claims) should be updated on a refresh token
//        /// request
//        /// </summary>
//        public bool UpdateAccessTokenClaimsOnRefresh { get; set; } = false;

//        /// <summary>
//        /// Gets or sets the refresh token expiration type
//        /// </summary>
//        public TokenExpiration RefreshTokenExpiration { get; set; } = TokenExpiration.Absolute;

//        /// <summary>
//        /// Gets or sets a value indicating whether the access token is a reference token or a self contained JWT token
//        /// </summary>
//        public AccessTokenType AccessTokenType { get; set; } = AccessTokenType.Jwt;

//        /// <summary>
//        /// Gets or sets a value indicating whether the local login is allowed for this client
//        /// </summary>
//        public bool EnableLocalLogin { get; set; } = true;

//        /// <summary>
//        /// Gets or sets a value indicating whether JWT access tokens should include an identifier
//        /// </summary>
//        public bool IncludeJwtId { get; set; } = true;

//        /// <summary>
//        /// Gets or sets a value indicating whether client claims should be always included in the access tokens - or only
//        /// for client credentials flow
//        /// </summary>
//        public bool AlwaysSendClientClaims { get; set; } = false;

//        /// <summary>
//        /// Gets or sets a salt value used in pair-wise subjectId generation for users of this client.
//        /// </summary>
//        [StringLength(200)]
//        public string PairWiseSubjectSalt { get; set; }

//        /// <summary>
//        /// Gets or sets the maximum duration (in seconds) since the last time the user authenticated. If <c>null</c>, then
//        /// no limits are set and the duration is infinite
//        /// </summary>
//        public int? UserSsoLifetime { get; set; }

//        /// <summary>
//        /// Gets or sets the list of allowed CORS origins for JavaScript clients
//        /// </summary>
//        public List<string> AllowedCorsOrigins { get; set; }

//        /// <summary>
//        /// Gets or sets the list of custom properties for the client
//        /// </summary>
//        public List<PropertyDto> Properties { get; set; }
//    }
//}
