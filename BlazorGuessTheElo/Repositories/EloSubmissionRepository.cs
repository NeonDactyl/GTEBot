using BlazorGuessTheElo.DataContext;
using BlazorGuessTheElo.Models;
using BlazorGuessTheElo.Repositories.Interfaces;
using BlazorGuessTheElo.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Repositories
{
    public class EloSubmissionRepository : IEloSubmissionRepository
    {
        private EloSubmissionDatabaseContext databaseContext;
        private readonly IDatabaseChangesService changesService;
        public EloSubmissionRepository(EloSubmissionDatabaseContext databaseContext, IDatabaseChangesService changesService)
        {
            this.databaseContext = databaseContext;
            this.changesService = changesService;
        }

        public void AddEntry(EloSubmission submission)
        {
            databaseContext.EloSubmissions.Add(submission);
            databaseContext.SaveChanges();
        }

        public List<EloSubmission> GetActiveEloSubmissionsByChannelId(ulong? channelId)
        {
            return (channelId == null) ? new List<EloSubmission>() : databaseContext.EloSubmissions.AsQueryable().Where(e => e.IsActive && e.SourceDiscordChannelId == channelId).ToList();
        }

        public void SetAllActiveToInactiveByChannelId(ulong channelId)
        {
            var active = databaseContext.EloSubmissions.AsQueryable().Where(x => x.IsActive == true && x.SourceDiscordChannelId == channelId);
            foreach (var submission in active)
            {
                submission.IsActive = false;
            }
            databaseContext.UpdateRange(active);
            databaseContext.SaveChanges();
            changesService.GamesMarkedInactive(channelId);
        }

        public bool UserHasEntriesInLastMinute(ulong discordUserId)
        {
            return databaseContext.EloSubmissions.AsQueryable().Any(submission => submission.SourceDiscordUserId == discordUserId && submission.SubmissionTime > DateTime.Now.AddMinutes(-1));
        }
    }
}
