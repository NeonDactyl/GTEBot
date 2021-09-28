using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services.Interfaces
{
    public interface IDatabaseChangesService
    {
        public void RefreshChannelAdded(ulong channelId);
        public event Action<ulong> ChannelChangedAdded;

        public void GameAdded(ulong channelId);
        public event Action<ulong> GameAddedAction;

        public void GamesMarkedInactive(ulong channelId);
        public event Action<ulong> GamesMarkedInactiveAction;

        public event Action<ulong> ChannelChangedRemoved;
        public void RefreshChannelRemoved(ulong channelId);
    }
}
