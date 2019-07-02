using System;
using System.Web;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.PublicControls;

namespace WebApplication5.Custom
{
    public class CustomLoginWidget : LoginWidget
    { 
        protected override void InitializeControls(GenericContainer container)
        {
            if (!SystemManager.IsDesignMode && ClaimsManager.GetCurrentUserId() == Guid.Empty)
            {
                var providerName = "CustomSTS";
                var widgetUrl = HttpContext.Current.Request.Url;
                string redirectUrl = RouteHelper.ResolveUrl(this.DestinationPageUrl, UrlResolveOptions.Absolute);

                var challengeProperties = ChallengeProperties.ForExternalUser(providerName, widgetUrl.AbsoluteUri);
                challengeProperties.RedirectUri = redirectUrl;

                this.Page.Request.GetOwinContext().Authentication.Challenge(challengeProperties, ClaimsManager.CurrentAuthenticationModule.STSAuthenticationType);
            }
            else
            {
                base.InitializeControls(container);
            }
        }
    }
}