using System;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;
using Data.Models.Tennis;
using MatchDataProcessor.Services;

class Program
{
    private const string _databasePath = "../../../DataBase/flashcards.db";
    private const string _matchFilesDirectory = "Dataset";
    private const string _matchFilePrefix = "atp_matches";

    static void Main(string[] args)
    {
        var allCsvFiles = Directory.GetFiles(_matchFilesDirectory, $"{_matchFilePrefix}*.csv")
            .Where(fileName => !fileName.Contains("doubles"))
            .ToList();

        if (allCsvFiles.Count == 0)
        {
            Console.WriteLine("No match CSV files found.");
            return;
        }

        var allMatches = new List<Match>();

        foreach (var file in allCsvFiles)
        {
            Console.WriteLine($"Reading: {file}");
            var matches = MatchProcessor.ReadFromCsv(file);
            allMatches.AddRange(matches);
        }

        using var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
        connection.Open();

        MatchIngester.EnsureTableExists(connection);
        MatchIngester.IngestMatches(connection, allMatches);

        Console.WriteLine("✅ All match data ingested.");
    }
}
