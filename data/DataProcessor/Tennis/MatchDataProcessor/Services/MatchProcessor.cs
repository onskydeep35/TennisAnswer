using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Data.Models.Tennis;

namespace MatchDataProcessor.Services
{
    /// <summary>
    /// Provides functionality to read and parse tennis match data from CSV files.
    /// </summary>
    public static class MatchProcessor
    {
        /// <summary>
        /// Reads a CSV file and converts each row into a <see cref="Match"/> object.
        /// Skips lines with invalid column counts or values.
        /// </summary>
        /// <param name="path">Path to the CSV file containing match data.</param>
        /// <returns>A list of parsed <see cref="Match"/> objects.</returns>
        public static List<Match> ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            var matches = new List<Match>();

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length != 50) continue;

                try
                {
                    matches.Add(new Match
                    {
                        MatchId = parts[49],
                        TourneyId = parts[0],
                        TourneyName = parts[1],
                        Surface = parts[2],
                        DrawSize = TryInt(parts[3]),
                        TourneyLevel = parts[4],
                        TourneyDate = TryDate(parts[5]),
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

        /// <summary>
        /// Attempts to parse a date string in yyyyMMdd format.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>A nullable DateTime value.</returns>
        private static DateTime? TryDate(string value)
            => DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) ? date : null;

        /// <summary>
        /// Attempts to parse an integer string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>A nullable integer.</returns>
        private static int? TryInt(string value)
            => int.TryParse(value, out var result) ? result : null;

        /// <summary>
        /// Attempts to parse a floating-point string.
        /// </summary>
        /// <param name="value">The string to parse.</param>
        /// <returns>A nullable double.</returns>
        private static double? TryDouble(string value)
            => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : null;
    }
} 
