using BlazorGuessTheElo.Services.Interfaces;
using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using BlazorGuessTheElo.Providers;

namespace BlazorGuessTheElo.Services
{
    public class DiscordManagementService : IDiscordManagementService
    {
        private readonly DiscordRestClient discordRestClient;
        private readonly DiscordSocketClient discordSocketClient;
        private readonly TokenProvider tokenProvider;
        private string userToken;

        public List<RestUserGuild> Guilds;

        public DiscordManagementService(DiscordRestClient client, TokenProvider tokenProvider, DiscordSocketClient discordSocketClient)
        {
            this.discordRestClient = client;
            this.discordSocketClient = discordSocketClient;
            this.tokenProvider = tokenProvider;
        }

        public async Task InitializeAsync()
        {
            userToken = tokenProvider.AccessToken;
            try
            {
                if (discordRestClient.LoginState == LoginState.LoggedOut)
                    await discordRestClient.LoginAsync(TokenType.Bearer, userToken);
                Guilds = await RetrieveGuilds();
            }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
        }

        public LoginState GetLoginState()
        {
            return discordRestClient.LoginState;
        }

        public async Task CloseChannelToRole(ulong channelId, ulong roleId)
        {
            var guild = discordSocketClient.Guilds.First(x => x.Channels.Select(y => y.Id).ToList().Contains(channelId));
            var chan = guild.Channels.First(x => x.Id == channelId);
            var role = guild.Roles.First(x => x.Id == roleId);
            await chan.AddPermissionOverwriteAsync(role, new OverwritePermissions(sendMessages: PermValue.Deny));
            //var guild = discordRestClient.GetGuildsAsync().Result
            //        .First(g =>
            //            g.GetChannelsAsync().Result.Any(c => c.Id == channelId));
            //var channel = guild.GetChannelAsync(channelId).Result;
            //var role = guild.GetRole(roleId);
            //await channel.AddPermissionOverwriteAsync(role, new OverwritePermissions(sendMessages: PermValue.Deny));

        }

        public async Task OpenChannelToRole(ulong channelId, ulong roleId)
        {
            var guild = discordSocketClient.Guilds.First(x => x.Channels.Select(y => y.Id).ToList().Contains(channelId));
            var chan = guild.Channels.First(x => x.Id == channelId);
            var role = guild.Roles.First(x => x.Id == roleId);
            await chan.AddPermissionOverwriteAsync(role, new OverwritePermissions(sendMessages: PermValue.Allow));
            //var guild = discordRestClient.GetGuildsAsync().Result
            //        .First(g =>
            //            g.GetChannelsAsync().Result.Any(c => c.Id == channelId));
            //var channel = guild.GetChannelAsync(channelId).Result;
            //var role = guild.GetRole(roleId);
            //await channel.AddPermissionOverwriteAsync(role, new OverwritePermissions(sendMessages: PermValue.Allow));
        }

        public async Task<List<RestUserGuild>> RetrieveGuilds()
        {
            try
            {
                if (discordRestClient.LoginState != LoginState.LoggedIn) return new List<RestUserGuild>();
                var guilds = (await discordRestClient.GetGuildSummariesAsync().ToListAsync()).SelectMany(x => x).ToList();
                return guilds.Where(x => x.Permissions.Administrator).ToList();
            }
            catch (Exception ex)
            {
                return new List<RestUserGuild>();
            }

            //var x = discordRestClient.


        }

        public List<RestUserGuild> GetGuilds()
        {
            return Guilds ?? new List<RestUserGuild>();
        }

        public List<SocketTextChannel> GetChannelsByGuild(ulong guildId)
        {
            var guild = discordSocketClient.Guilds.First(x => x.Id == guildId);
            var channels = discordSocketClient.Guilds.First(x => x.Id == guildId).TextChannels.ToList();
            return channels;
        }
    }
}
