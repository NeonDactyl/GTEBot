using BlazorGuessTheElo.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services
{
    public class DatabaseChangesService : IDatabaseChangesService
    {
        public event Action<ulong> ChannelChangedAdded;
        public event Action<ulong> ChannelChangedRemoved;
        public event Action<ulong> GameAddedAction;
        public event Action<ulong> GamesMarkedInactiveAction;

        public void GameAdded(ulong channelId)
        {
            GameAddedAction?.Invoke(channelId);
        }

        public void GamesMarkedInactive(ulong channelId)
        {
            GamesMarkedInactiveAction?.Invoke(channelId);
        }

        public void RefreshChannelAdded(ulong channelId)
        {
            Console.WriteLine("Invokeing RefreshChannelAdded");
            ChannelChangedAdded?.Invoke(channelId);
        }
        public void RefreshChannelRemoved(ulong channelId)
        {
            Console.WriteLine("Invokeing RefreshChannelRemoved");
            ChannelChangedAdded?.Invoke(channelId);
        }
    }
}
