using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Telerik.Sitefinity.Authentication;
using Telerik.Sitefinity.Authentication.Configuration;
using Telerik.Sitefinity.Authentication.Configuration.SecurityTokenService.ExternalProviders;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Security.Claims;

namespace WebApplication5.Custom
{
    public class CustomAuthenticationProvidersInitializer : AuthenticationProvidersInitializer
    {
        public override Dictionary<string, Action<IAppBuilder, string, AuthenticationProviderElement>> GetAdditionalIdentityProviders()
        {
            var providers = base.GetAdditionalIdentityProviders();
            //return providers;
            providers.Add("CustomSTS", (IAppBuilder app, string signInAsType, AuthenticationProviderElement providerConfig) =>
            {
                var clientId = providerConfig.GetParameter("clientId");
                var issuer = providerConfig.GetParameter("issuer").Trim('/');
                var redirectUri = providerConfig.GetParameter("redirectUri");
                var responseType = providerConfig.GetParameter("responseType");
                var scope = providerConfig.GetParameter("scope");
                var caption = providerConfig.GetParameter("caption");

                var localStsRelativePath = Config.Get<AuthenticationConfig>().SecurityTokenService.ServicePath.Trim('/');

                var options = new OpenIdConnectAuthenticationOptions()
                {
                    ClientId = clientId,
                    Authority = issuer + "/",
                    AuthenticationType = providerConfig.Name,
                    SignInAsAuthenticationType = signInAsType,
                    RedirectUri = redirectUri,
                    ResponseType = responseType,
                    Scope = scope,
                    Caption = caption,
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        SecurityTokenValidated = n => this.SecurityTokenValidatedInternal(n),
                        RedirectToIdentityProvider = n => this.RedirectToIdentityProvider(n)
                    }
                };

                app.UseOpenIdConnectAuthentication(options);
            });

            return providers;
        }

        private Task RedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> r)
        {
            r.ProtocolMessage.AcrValues = "ui_locale=en-us;tenant-code=DE";
            r.ProtocolMessage.UiLocales = "en-US";
            return Task.CompletedTask;
        }

        private Task SecurityTokenValidatedInternal(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            var identity = notification.AuthenticationTicket.Identity;

            var externalUserEmail = identity.FindFirst("email");
            if (externalUserEmail != null)
                identity.AddClaim(new Claim(SitefinityClaimTypes.ExternalUserEmail, externalUserEmail.Value));

            var externalUserId = identity.FindFirst("sub");
            if (externalUserId != null)
                identity.AddClaim(new Claim(SitefinityClaimTypes.ExternalUserId, externalUserId.Value));

            var externalUserFirstName = identity.FindFirst("given_name") != null ? identity.FindFirst("given_name").Value : string.Empty;
            var externalUserFamilyName = identity.FindFirst("family_name") != null ? identity.FindFirst("family_name").Value : string.Empty;
            var externalUserFullName = externalUserFirstName + " " + externalUserFamilyName;
            identity.AddClaim(new Claim(SitefinityClaimTypes.ExternalUserName, externalUserFullName));

            var externalUserNickName = identity.FindFirst("nickname") != null ? identity.FindFirst("nickname").Value : string.Empty;
            identity.AddClaim(new Claim(SitefinityClaimTypes.ExternalUserNickName, externalUserNickName));

            var externalUserPicture = identity.FindFirst("picture");
            if (externalUserPicture != null)
                identity.AddClaim(new Claim(SitefinityClaimTypes.ExternalUserPictureUrl, externalUserPicture.Value));

            return Task.FromResult(0);
        }
    }
}