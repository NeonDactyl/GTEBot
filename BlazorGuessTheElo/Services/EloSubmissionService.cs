using BlazorGuessTheElo.Models;
using BlazorGuessTheElo.Repositories.Interfaces;
using BlazorGuessTheElo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services
{
    public class EloSubmissionService : IEloSubmissionService
    {
        public IEloSubmissionRepository eloSubmissionRepository;
        public EloSubmissionService(IServiceScopeFactory serviceScopeFactory)
        {
            this.eloSubmissionRepository = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IEloSubmissionRepository>();
        }
        public bool HandleSubmission(ref EloSubmission eloSubmission)
        {

            //if (eloSubmissionRepository.UserHasEntriesInLastMinute(eloSubmission.SourceDiscordUserId))
            //{
            //    eloSubmission.IsValid = false;
            //    eloSubmission.ErrorMessage = "There's already a game submitted recently from this account.";
            //    return false;
            //}
            eloSubmissionRepository.AddEntry(eloSubmission);
            return true;
        }
    }
}
