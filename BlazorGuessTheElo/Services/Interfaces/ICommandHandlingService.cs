using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services.Interfaces
{
    public interface ICommandHandlingService
    {
        public Task InitializeAsync();
        public Task MessageReceivedAsync(SocketMessage rawMessage);
        public Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result);
    }
}
