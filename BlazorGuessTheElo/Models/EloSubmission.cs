using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Models
{
    public enum ChessColor
    {
        White = 1,
        Black = 2,
        NotFound = 0
    }
    public class EloSubmission
    {
        public int Id { get; set; }
        public string GameUrl { get; set; }
        public ChessColor Color { get; set; }
        public string Pgn { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        public ulong SourceDiscordUserId { get; set; }
        public string DiscordUserName { get; set; }
        public ulong SourceDiscordChannelId { get; set; }
        public DateTime SubmissionTime { get; set; }
        public bool IsActive { get; set; }

        public EloSubmission()
        {
            Color = ChessColor.NotFound;
        }
    }
}