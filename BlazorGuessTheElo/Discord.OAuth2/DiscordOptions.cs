using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorGuessTheEloDiscord.OAuth2
{
    public class DiscordOptions : OAuthOptions
    {
        public string UserGuildsEndpoint;
        /// <summary> Initializes a new <see cref="DiscordOptions"/>. </summary>
        public DiscordOptions()
        {
            CallbackPath = new PathString("/signin-discord");
            AuthorizationEndpoint = DiscordDefaults.AuthorizationEndpoint;
            TokenEndpoint = DiscordDefaults.TokenEndpoint;
            UserInformationEndpoint = DiscordDefaults.UserInformationEndpoint;
            UserGuildsEndpoint = DiscordDefaults.UserGuildsEndpoint;
            CorrelationCookie = new Microsoft.AspNetCore.Http.CookieBuilder
            {
                HttpOnly = false,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always,
                Expiration = TimeSpan.FromMinutes(10)
            };  
            Scope.Add("identify");
            Scope.Add("guilds");

            ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id", ClaimValueTypes.UInteger64);
            ClaimActions.MapJsonKey(ClaimTypes.Name, "username", ClaimValueTypes.String);
            ClaimActions.MapJsonKey(ClaimTypes.Email, "email", ClaimValueTypes.Email);
            ClaimActions.MapJsonKey("urn:discord:discriminator", "discriminator", ClaimValueTypes.UInteger32);
            ClaimActions.MapJsonKey("urn:discord:avatar", "avatar", ClaimValueTypes.String);
            ClaimActions.MapJsonKey("urn:discord:verified", "verified", ClaimValueTypes.Boolean);
        }

        /// <summary> Gets or sets the Discord-assigned appId. </summary>
        public string AppId { get => ClientId; set => ClientId = value; }
        /// <summary> Gets or sets the Discord-assigned app secret. </summary>
        public string AppSecret { get => ClientSecret; set => ClientSecret = value; }
    }
}
