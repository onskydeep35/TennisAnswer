using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Data.Models.Tennis;

namespace MatchDataProcessor.Services
{
    public static class MatchProcessor
    {
        public static List<Match> ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            var matches = new List<Match>();

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length != 49) continue;

                try
                {
                    DateTime? parsedDate = null;
                    if (!string.IsNullOrWhiteSpace(parts[5]) &&
                    DateTime.TryParseExact(parts[5], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
                    {
                        parsedDate = dob;
                    }

                    matches.Add(new Match
                    {
                        TourneyId = parts[0],
                        TourneyName = parts[1],
                        Surface = parts[2],
                        DrawSize = TryInt(parts[3]),
                        TourneyLevel = parts[4],
                        TourneyDate = parsedDate,
                        MatchNum = int.Parse(parts[6]),
                        WinnerId = int.Parse(parts[7]),
                        WinnerSeed = parts[8],
                        WinnerEntry = parts[9],
                        WinnerName = parts[10],
                        WinnerHand = parts[11],
                        WinnerHeight = TryInt(parts[12]),
                        WinnerIoc = parts[13],
                        WinnerAge = TryDouble(parts[14]),
                        LoserId = int.Parse(parts[15]),
                        LoserSeed = parts[16],
                        LoserEntry = parts[17],
                        LoserName = parts[18],
                        LoserHand = parts[19],
                        LoserHeight = TryInt(parts[20]),
                        LoserIoc = parts[21],
                        LoserAge = TryDouble(parts[22]),
                        Score = parts[23],
                        BestOf = int.Parse(parts[24]),
                        Round = parts[25],
                        Minutes = TryInt(parts[26]),
                        WAce = TryInt(parts[27]),
                        WDoubleFault = TryInt(parts[28]),
                        WServePoints = TryInt(parts[29]),
                        WFirstIn = TryInt(parts[30]),
                        WFirstWon = TryInt(parts[31]),
                        WSecondWon = TryInt(parts[32]),
                        WServiceGames = TryInt(parts[33]),
                        WBpSaved = TryInt(parts[34]),
                        WBpFaced = TryInt(parts[35]),
                        LAce = TryInt(parts[36]),
                        LDoubleFault = TryInt(parts[37]),
                        LServePoints = TryInt(parts[38]),
                        LFirstIn = TryInt(parts[39]),
                        LFirstWon = TryInt(parts[40]),
                        LSecondWon = TryInt(parts[41]),
                        LServiceGames = TryInt(parts[42]),
                        LBpSaved = TryInt(parts[43]),
                        LBpFaced = TryInt(parts[44]),
                        WinnerRank = TryInt(parts[45]),
                        WinnerRankPoints = TryInt(parts[46]),
                        LoserRank = TryInt(parts[47]),
                        LoserRankPoints = TryInt(parts[48])
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to parse line {i + 1} in file {path}: {ex.Message}");
                }
            }

            return matches;
        }

        private static int? TryInt(string value)
            => int.TryParse(value, out var result) ? result : null;

        private static double? TryDouble(string value)
            => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : null;
    }
} 
