using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services.Interfaces
{
    public interface IDiscordManagementService
    {
        public Task OpenChannelToRole(ulong channelId, ulong roleId);
        public Task CloseChannelToRole(ulong channelId, ulong roleId);
        public Task InitializeAsync();
        public LoginState GetLoginState();
        public Task<List<RestUserGuild>> RetrieveGuilds();
        public List<RestUserGuild> GetGuilds();
        public List<SocketTextChannel> GetChannelsByGuild(ulong guildId);
    }
}
