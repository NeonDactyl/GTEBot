using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services.Interfaces
{
    public interface IDatabaseChangesService
    {
        public void RefreshChannel(ulong channelId);
        public event Action<ulong> ChannelChanged;

        public void GameAdded(ulong channelId);
        public event Action<ulong> GameAddedAction;

        public void GamesMarkedInactive(ulong channelId);
        public event Action<ulong> GamesMarkedInactiveAction;
    }
}
