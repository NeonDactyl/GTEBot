using BlazorGuessTheElo.Services.Interfaces;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services
{
    public class MessageDeletionService : IMessageDeletionService
    {
        private List<SocketMessage> buffer;
        private readonly DiscordSocketClient discordSocketClient;
        private Timer bufferTimer;
        public MessageDeletionService(DiscordSocketClient discordSocketClient)
        {
            this.discordSocketClient = discordSocketClient;
            this.buffer = new List<SocketMessage>();
            bufferTimer = new Timer(this.ClientDeleteMessages, null, 250, 250);
        }
        public void DeleteMessage(SocketMessage message)
        {
            buffer.Add(message);
        }

        private void ClientDeleteMessages(object stateInfo)
        {
            if (buffer.Count == 0) return;
            Console.WriteLine("Deleting a message");
            var m = buffer.First();
            m.DeleteAsync();
            buffer.Remove(m);
        }
    }
}
