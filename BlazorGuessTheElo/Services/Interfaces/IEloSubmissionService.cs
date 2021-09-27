using BlazorGuessTheElo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Services.Interfaces
{
    public interface IEloSubmissionService
    {
        bool HandleSubmission(ref EloSubmission eloSubmission);
    }
}
