using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Models
{
    public class AllowedChannel
    {
        public AllowedChannel(ulong id)
        {
            ChannelId = id;
        }

        public AllowedChannel()
        { }
        public ulong ChannelId { get; set; }
        public bool Allowed { get; set; }
    }
}
