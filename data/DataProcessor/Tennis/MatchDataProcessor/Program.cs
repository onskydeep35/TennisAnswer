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
    private const string _singlesMatchesFilePath = "Dataset/merged.csv";

    static void Main(string[] args)
    {
        var allMatches = new List<Match>();

        Console.WriteLine($"Reading: {_singlesMatchesFilePath}");
        var matches = MatchProcessor.ReadFromCsv(_singlesMatchesFilePath);
        allMatches.AddRange(matches);

        using var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;");
        connection.Open();

        MatchIngester.EnsureTableExists(connection);
        MatchIngester.IngestMatches(connection, allMatches);

        Console.WriteLine("✅ All match data ingested.");
    }
}
