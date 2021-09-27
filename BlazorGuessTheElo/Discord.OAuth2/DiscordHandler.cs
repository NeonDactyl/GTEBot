using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorGuessTheEloDiscord.OAuth2
{
    internal class DiscordHandler : OAuthHandler<DiscordOptions>
    {
        public DiscordHandler(IOptionsMonitor<DiscordOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            userInfoRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var userInfoResponse = await Backchannel.SendAsync(userInfoRequest, Context.RequestAborted);
            if (!userInfoResponse.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to retrieve Discord user information ({userInfoResponse.StatusCode}).");

            var userInfoPayload = JsonDocument.Parse(await userInfoResponse.Content.ReadAsStringAsync());

            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, userInfoPayload.RootElement);
            context.RunClaimActions();

            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
        }
    }
}
