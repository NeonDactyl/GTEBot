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
        string token;
        public event Action<ulong> GuildChanged;

        public EloBotService(IConfiguration config, DiscordSocketClient client, ICommandHandlingService commandHandlingService)
        {
            configuration = config;
            this.socketClient = client;
            token = configuration.GetValue<string>("Discord:Token");
            this.commandHandlingService = commandHandlingService;
        }

        public void Dispose()
        {

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            socketClient.Log += LogAsync;
            await socketClient.LoginAsync(TokenType.Bot, token);
            await socketClient.StartAsync();
            await commandHandlingService.InitializeAsync();
            socketClient.JoinedGuild += GuildJoined;
        }

        public async Task GuildJoined(SocketGuild guild)
        {
            GuildChanged?.Invoke(guild.Id);
            foreach (SocketTextChannel channel in guild.TextChannels)
            {
                await channel.AddPermissionOverwriteAsync(socketClient.CurrentUser, new OverwritePermissions(viewChannel: PermValue.Deny));
            }
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
