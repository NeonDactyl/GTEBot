﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheEloDiscord.OAuth2
{
    public static class DiscordAuthenticationOptionsExtensions
    {
        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder)
            => builder.AddDiscord(DiscordDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder, Action<DiscordOptions> configureOptions)
            => builder.AddDiscord(DiscordDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder, string authenticationScheme, Action<DiscordOptions> configureOptions)
            => builder.AddDiscord(authenticationScheme, DiscordDefaults.DisplayName, configureOptions);

        public static AuthenticationBuilder AddDiscord(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<DiscordOptions> configureOptions)
            => builder.AddOAuth<DiscordOptions, DiscordHandler>(authenticationScheme, displayName, configureOptions);
    }
}
