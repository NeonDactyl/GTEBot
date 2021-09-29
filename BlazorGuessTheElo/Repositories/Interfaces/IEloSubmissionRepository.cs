using BlazorGuessTheElo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Repositories.Interfaces
{
    public interface IEloSubmissionRepository
    {
        public void SetAllActiveToInactiveByChannelId(ulong channelId);
        public List<EloSubmission> GetActiveEloSubmissionsByChannelId(ulong? channelId);
        public void AddEntry(EloSubmission submission);
        public void AddEntries(IList<EloSubmission> submissions);
        public bool UserHasEntriesInLastMinute(ulong discordUserId);
    }
}
