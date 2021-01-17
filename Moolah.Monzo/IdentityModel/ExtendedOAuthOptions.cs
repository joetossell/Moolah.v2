using Microsoft.AspNetCore.Authentication.OAuth;

namespace Moolah.Monzo.IdentityModel
{
    public class ExtendedOAuthOptions : OAuthOptions
    {
        public string RevocationEndpoint { get; set; }
    }
}
