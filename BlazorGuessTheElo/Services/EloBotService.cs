using BlazorGuessTheElo.Services.Interfaces;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services
{
    public class EloBotService : IHostedService, IDisposable
    {
        DiscordSocketClient socketClient;
        IConfiguration configuration;
        ICommandHandlingService commandHandlingService;
        private IDatabaseChangesService databaseChangesService;
        string token;
        public event Action<ulong> GuildChanged;

        public EloBotService(IConfiguration config, DiscordSocketClient client, ICommandHandlingService commandHandlingService, IDatabaseChangesService databaseChangesService)
        {
            configuration = config;
            this.socketClient = client;
            token = configuration.GetValue<string>("Discord:Token");
            this.commandHandlingService = commandHandlingService;
            this.databaseChangesService = databaseChangesService;
            this.databaseChangesService.ChannelChangedAdded += AllowOnChannel;
            this.databaseChangesService.ChannelChangedRemoved += DenyOnChannel;
        }

        public void Dispose()
        {

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            socketClient.Log += LogAsync;
            await socketClient.LoginAsync(TokenType.Bot, token);
            await socketClient.StartAsync();
            await socketClient.SetStatusAsync(UserStatus.Offline);
            await commandHandlingService.InitializeAsync();
            socketClient.JoinedGuild += GuildJoined;
        }

        public async Task SetSlowMode(int seconds, ulong channelId)
        {
            var c = socketClient.Guilds.FirstOrDefault(x => x.TextChannels.Any(y => y.Id == channelId)).TextChannels.FirstOrDefault(x => x.Id == channelId);
            await c.ModifyAsync(x =>
            {
                x.SlowModeInterval = seconds;
            });
        }
        public async Task GuildJoined(SocketGuild guild)
        {
            GuildChanged?.Invoke(guild.Id);
            foreach (SocketTextChannel channel in guild.TextChannels)
            {
                await channel.AddPermissionOverwriteAsync(socketClient.CurrentUser, new OverwritePermissions(viewChannel: PermValue.Deny));
            }
        }

        public void AllowOnChannel(ulong channelId)
        {
            Console.WriteLine($"Allowing view access to channel {channelId}");
            var g = socketClient.Guilds.FirstOrDefault(x => x.Channels.Select(y => y.Id).Contains(channelId));
            var c = g.Channels.FirstOrDefault(x => x.Id == channelId);
            c.AddPermissionOverwriteAsync(socketClient.CurrentUser, new OverwritePermissions(viewChannel: PermValue.Allow));
        }

        public void DenyOnChannel(ulong channelId)
        {
            Console.WriteLine($"Denying view access to channel {channelId}");
            var g = socketClient.Guilds.FirstOrDefault(x => x.Channels.Select(y => y.Id).Contains(channelId));
            var c = g.Channels.FirstOrDefault(x => x.Id == channelId);
            c.AddPermissionOverwriteAsync(socketClient.CurrentUser, new OverwritePermissions(viewChannel: PermValue.Deny));
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await socketClient.LogoutAsync();
            await socketClient.StopAsync();
        }

        private Task LogAsync(LogMessage log)
        {
            try
            {
                Console.WriteLine(log.ToString());
            }
            catch
            {

            }
            return Task.CompletedTask;
        }
    }
}
