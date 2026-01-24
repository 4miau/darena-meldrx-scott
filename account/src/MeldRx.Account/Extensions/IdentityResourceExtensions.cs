using IdentityResource = Duende.IdentityServer.EntityFramework.Entities.IdentityResource;

namespace DarenaSolutions.Iam.Server.Extensions
{
    /// <summary>
    /// Class that contains extensions to the <see cref="IdentityResource"/> class
    /// </summary>
    public static class IdentityResourceExtensions
    {
        /// <summary>
        /// Maps an identity resource into its DTO representation
        /// </summary>
        /// <param name="self">The source identity resource</param>
        /// <returns>The DTO representation of the source identity resource</returns>
        public static ScopeResourceDto Map(this IdentityResource self)
        {
            var dto = new ScopeResourceDto
            {
                Id = self.Id,
                Required = self.Required,
                Emphasize = self.Emphasize,
                Enabled = self.Enabled,
                Name = self.Name,
                DisplayName = self.DisplayName,
                Description = self.Description,
                ShowInDiscoveryDocument = self.ShowInDiscoveryDocument
            };

            if (self.UserClaims != null)
            {
                foreach (var claim in self.UserClaims)
                {
                    dto.UserClaims.Add(claim.Type);
                }
            }

            return dto;
        }
    }
}
