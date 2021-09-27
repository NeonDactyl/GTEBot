using BlazorGuessTheElo.Models;
using Neondactyl.PgnParser.Net;
using NeonDactyl.ChessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlazorGuessTheElo.Parser
{
    public static class EloSubmissionParser
    {
        private readonly static Regex ColorRegex = new Regex("(white|black)", RegexOptions.IgnoreCase);
        private readonly static Regex UrlRegex = new Regex(@"https?:\/\/(www\.)?chess\.com\/(\w+\/)+(\d+)(\?(\w+=\w+&?)*)?", RegexOptions.IgnoreCase);
        private readonly static Regex MetadataColorRegex = new Regex(@"\[(white|black)\s+(""|')\w+(""|')\]", RegexOptions.IgnoreCase);
        private readonly static Regex LeadingMoveNumberRegex = new Regex(@"^(\d+\.)");

        private readonly static Regex PieceMove = new Regex(@"^([KQBNR][a-h1-8]?x?[a-h][1-8](\+|#)?)$");
        private readonly static Regex CastlesMove = new Regex(@"^O-O(-O)?(\+|#)?$");
        private readonly static Regex PawnMove = new Regex(@"^[a-h][2-7](\+|#)?$");
        private readonly static Regex PawnTake = new Regex(@"^[a-h]x[a-h][2-7](\+|#)?$");
        private readonly static Regex PawnPromote = new Regex(@"^[a-h][18]=[QBNR](\+|#)?$");
        private readonly static Regex PawnTakePromote = new Regex(@"^[a-h]x[a-h][18]=[QBNR](\+|#)?$");


        public static EloSubmission Parse(string message)
        {
            string[] splitMessage = message.Split();
            List<string> moves = new List<string>();
            EloSubmission eloSubmission = new EloSubmission();
            int depth = 0;
            List<char> depthMatchers = new List<char>();

            foreach (string chunk in splitMessage)
            {
                if (string.IsNullOrWhiteSpace(chunk)) continue;

                string trimmedChunk = chunk.Trim();

                if (trimmedChunk.StartsWith('[') || trimmedChunk.StartsWith('(') || trimmedChunk.StartsWith('{'))
                {
                    depth++;
                    switch (trimmedChunk.First())
                    {
                        case ('['):
                            depthMatchers.Add(']');
                            break;
                        case ('('):
                            depthMatchers.Add(')');
                            break;
                        case ('{'):
                            depthMatchers.Add('}');
                            break;
                    }
                }

                if (depthMatchers.Count > 0 && trimmedChunk.EndsWith(depthMatchers.Last()))
                {
                    depth--;
                    depthMatchers.RemoveAt(depthMatchers.Count - 1);
                }

                Match colorMatch = ColorRegex.Match(chunk);
                if (colorMatch.Success)
                {
                    Match metadataColorMatch = MetadataColorRegex.Match(chunk);
                    if (!metadataColorMatch.Success)
                    {
                        eloSubmission.Color = colorMatch.Groups[0].Value == "white" ? ChessColor.White : ChessColor.Black;
                        continue;
                    }
                }

                Match urlMatch = UrlRegex.Match(chunk);
                if (urlMatch.Success)
                {
                    eloSubmission.GameUrl = trimmedChunk;
                    continue;
                }

                if (depth > 0) continue;
                if (LeadingMoveNumberRegex.Match(trimmedChunk).Success) trimmedChunk = LeadingMoveNumberRegex.Replace(trimmedChunk, "");
                if (IsSanNotation(trimmedChunk)) moves.Add(trimmedChunk);
            }

            string pgn = string.Join(' ', moves).Trim();

            var reader = new PgnParser();
            try
            {
                eloSubmission.Pgn = pgn;
                reader.ParseFromString(pgn);
                if (reader.CountGames() > 1) throw new Exception("More than one game in message.");
                Position gamePosition = Board.StartPosition;
                foreach (string move in reader.GetCurrentGame().GetMovesArray())
                {
                    try
                    {
                        gamePosition = gamePosition.MakeMove(move);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error reading pgn at move {move}.", ex);
                    }
                }
            }
            catch (Exception e)
            {
                eloSubmission.ErrorMessage = e.Message;
                eloSubmission.IsValid = false;
                return eloSubmission;
            }

            if (string.IsNullOrWhiteSpace(eloSubmission.GameUrl))
            {
                eloSubmission.ErrorMessage = "No game link found.";
                eloSubmission.IsValid = false;
                return eloSubmission;
            }

            if (string.IsNullOrWhiteSpace(eloSubmission.Pgn))
            {
                eloSubmission.ErrorMessage = "No valid PGN could be found.";
                eloSubmission.IsValid = false;
                return eloSubmission;
            }

            if (eloSubmission.Color == ChessColor.NotFound)
            {
                eloSubmission.ErrorMessage = "No color information could be found.";
                eloSubmission.IsValid = false;
                return eloSubmission;
            }


            eloSubmission.IsValid = true;

            return eloSubmission;
        }

        public static bool IsSanNotation(string input)
        {
            if (PieceMove.Match(input).Success) return true;
            if (CastlesMove.Match(input).Success) return true;
            if (PawnMove.Match(input).Success) return true;
            if (PawnTake.Match(input).Success) return true;
            if (PawnPromote.Match(input).Success) return true;
            if (PawnTakePromote.Match(input).Success) return true;
            return false;
        }
    }
}
