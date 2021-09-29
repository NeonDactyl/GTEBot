using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services.Interfaces
{
    public interface IMessageDeletionService
    {
        public void DeleteMessage(SocketMessage message);
    }
}
