using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Repositories.Interfaces
{
    public interface IAllowedChannelsRepository
    {
        public List<ulong> GetAllowedChannels();
        public void AddAllowedChannel(ulong channelId);
        public void RemoveAllowedChannel(ulong channelId);
    }
}
