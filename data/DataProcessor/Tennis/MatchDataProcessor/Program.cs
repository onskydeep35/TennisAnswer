using System;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;
using Data.Models.Tennis;
using MatchDataProcessor.Services;

/// <summary>
/// Entry point for the Tennis Match Data Processor application.
/// Aggregates multiple ATP match CSV files and ingests them into an SQLite database.
/// </summary>
class Program
{
    /// <summary>
    /// Relative path to the SQLite database file.
    /// </summary>
    private const string _databasePath = "../../../DataBase/flashcards.db";

    /// <summary>
    /// Directory containing all ATP match CSV files.
    /// </summary>
    private const string _matchFilesDirectory = "Dataset";

    /// <summary>
    /// Prefix used to filter ATP match files.
    /// </summary>
    private const string _matchFilePrefix = "atp_matches";

    /// <summary>
    /// Main method to ingest all ATP match data files (excluding doubles) into the database.
    /// </summary>
    /// <param name="args">Command-line arguments (not used).</param>
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
