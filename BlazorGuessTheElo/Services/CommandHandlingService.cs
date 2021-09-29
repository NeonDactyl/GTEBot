using BlazorGuessTheElo.Models;
using BlazorGuessTheElo.Parser;
using BlazorGuessTheElo.Repositories.Interfaces;
using BlazorGuessTheElo.Services.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services
{
    public class CommandHandlingService : ICommandHandlingService
    {
        private readonly CommandService commandService;
        private readonly DiscordSocketClient discordSocketClient;
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;
        private readonly IEloSubmissionService eloSubmissionService;
        private readonly IAllowedChannelsRepository allowedChannelsRepository;
        private readonly IDatabaseChangesService databaseChangesService;
        private readonly IMessageDeletionService messageDeletionService;
        List<ulong> ChannelIds;
        private readonly char CommandPrefix;

        public CommandHandlingService(CommandService commandService,
            DiscordSocketClient discordSocketClient,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory,
            IDatabaseChangesService databaseChangesService,
            IMessageDeletionService messageDeletionService)
        {
            this.commandService = commandService;
            this.discordSocketClient = discordSocketClient;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
            this.messageDeletionService = messageDeletionService;
            this.databaseChangesService = databaseChangesService;
            this.databaseChangesService.ChannelChangedAdded += Refresh;
            using (var scope = serviceScopeFactory.CreateScope())
            {
                this.eloSubmissionService = scope.ServiceProvider.GetService<IEloSubmissionService>();
                this.allowedChannelsRepository = scope.ServiceProvider.GetService<IAllowedChannelsRepository>();
            }
            CommandPrefix = this.configuration.GetValue<char>("Discord:CommandPrefix");

            ChannelIds = allowedChannelsRepository.GetAllowedChannels();

            commandService.CommandExecuted += CommandExecutedAsync;
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified) return;

            if (result.IsSuccess) return;

            await context.Channel.SendMessageAsync($"error: {result}");
        }

        public async Task InitializeAsync()
        {
            var a = Assembly.GetEntryAssembly();
            await commandService.AddModulesAsync(a, serviceProvider);
            this.discordSocketClient.MessageReceived += MessageReceivedAsync;
            this.databaseChangesService.ChannelChangedAdded += Refresh;
        }

        public void Refresh(ulong channelId)
        {
            ChannelIds = allowedChannelsRepository.GetAllowedChannels();
        }

        public Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            var start = DateTime.Now;
            if (!(rawMessage is SocketUserMessage message)) return Task.CompletedTask;
            if (!ChannelIds.Contains(message.Channel.Id)) return Task.CompletedTask;
            if (message.Source != MessageSource.User) return Task.CompletedTask;

            EloSubmission eloSubmission = EloSubmissionParser.Parse(message.Content);
            Console.WriteLine($"MessageReceivedAsync: Parsed in ${(DateTime.Now - start).TotalMilliseconds} ms");
            if (eloSubmission.IsValid)
            {
                eloSubmission.SourceDiscordUserId = message.Author.Id;
                eloSubmission.DiscordUserName = message.Author.Username;
                eloSubmission.SourceDiscordChannelId = rawMessage.Channel.Id;
                eloSubmission.IsActive = true;
                eloSubmission.SubmissionTime = DateTime.Now;
                if (eloSubmissionService.HandleSubmission(ref eloSubmission))
                    rawMessage.Author.SendMessageAsync($"Your submission for Guess The Elo has been recorded.\nGame link: <{eloSubmission.GameUrl}>");
                else
                    rawMessage.Author.SendMessageAsync($"Your submission was not recorded: \n\n{eloSubmission.ErrorMessage}\n\n {string.Join("\n> ", message.Content.Split('\n'))}");
                messageDeletionService.DeleteMessage(message);
                Console.WriteLine($"MessageReceivedAsync: Valid Parse: Returned in ${(DateTime.Now - start).TotalMilliseconds} ms");
                return Task.CompletedTask;
            }
            else
            {
                message.Author.SendMessageAsync($"Your submission was not recorded:\n\n{eloSubmission.ErrorMessage}\n\n> {string.Join("\n> ", message.Content.Split('\n'))}");
                messageDeletionService.DeleteMessage(message);
                Console.WriteLine($"MessageReceivedAsync: Invalid Parse: Returned in ${(DateTime.Now - start).TotalMilliseconds} ms");
                return Task.CompletedTask;
            }
        }
    }
}
