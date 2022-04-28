using EduManagementLab.Core.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace EduManagementLab.Core.Validation
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds support for client authentication using JWT bearer assertions signed
        /// with client private key stored in PEM format rather than X509Certificate2 format.
        /// </summary>
        /// <remarks>
        /// See <see cref="IdentityServerBuilderExtensionsAdditional.AddJwtBearerClientAuthentication"/>
        /// for X509Certificate2 version.
        /// </remarks>
        public static IIdentityServerBuilder AddLtiJwtBearerClientAuthentication(this IIdentityServerBuilder builder)
        {
            builder.Services.AddLogging();
            builder.AddSecretValidator<PrivatePemKeyJwtSecretValidator>();

            return builder;
        }
    }
}
