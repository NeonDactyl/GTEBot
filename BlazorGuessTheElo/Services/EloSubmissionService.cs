using BlazorGuessTheElo.Models;
using BlazorGuessTheElo.Repositories.Interfaces;
using BlazorGuessTheElo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services
{
    public class EloSubmissionService : IEloSubmissionService
    {
        public IEloSubmissionRepository eloSubmissionRepository;
        private List<EloSubmission> buffer;
        Timer bufferTimer;
        public EloSubmissionService(IServiceScopeFactory serviceScopeFactory)
        {
            this.eloSubmissionRepository = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IEloSubmissionRepository>();
            this.buffer = new List<EloSubmission>();
            bufferTimer = new Timer(this.SaveEntries, null, 5000, 5000);
        }
        public bool HandleSubmission(ref EloSubmission eloSubmission)
        {
            buffer.Add(eloSubmission);
            return true;
        }

        private void SaveEntries(object stateInfo)
        {
            if (buffer.Count == 0) return;
            var start = DateTime.Now;
            eloSubmissionRepository.AddEntries(buffer);
            Console.WriteLine($"Saved submissions via timer: {buffer.Count()} entries\t {(DateTime.Now - start).TotalMilliseconds} ms");
            buffer.Clear();
        }
    }
}
