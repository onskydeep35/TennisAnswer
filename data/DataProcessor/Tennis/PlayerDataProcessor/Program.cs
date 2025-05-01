using System;
using System.Data.SQLite;
using System.IO;
using Data.Models.Tennis;
using PlayerDataProcessor.Services;

namespace PlayerDataProcessor
{
    /// <summary>
    /// Entry point for the Tennis Player Data Processor application.
    /// Reads tennis player data from a CSV file and ingests it into an SQLite database.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Relative path to the SQLite database file.
        /// </summary>
        private const string _dataBasePath = "../../../DataBase/flashcards.db";

        /// <summary>
        /// Default path to the sample CSV data file.
        /// </summary>
        private const string _sampleDataPath = "sampledata.csv";

        /// <summary>
        /// Main execution method. Parses command-line arguments, reads CSV data, and ingests it into the database.
        /// </summary>
        /// <param name="args">Optional command-line arguments. The first argument may specify the CSV file path.</param>
        static void Main(string[] args)
        {
            string csvPath = args.Length > 0 ? args[0] : _sampleDataPath;

            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"CSV file not found: {csvPath}");
                return;
            }

            var players = PlayerProcessor.ReadFromCsv(csvPath);

            using var connection = new SQLiteConnection($"Data Source={_dataBasePath};Version=3;");
            connection.Open();

            PlayerIngester.EnsureTableExists(connection);
            PlayerIngester.IngestPlayers(connection, players);
        }
    }
}
