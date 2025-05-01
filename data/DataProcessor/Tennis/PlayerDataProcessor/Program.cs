using System;
using System.Data.SQLite;
using Data.Models.Tennis;
using PlayerDataProcessor.Services;

namespace PlayerDataProcessor
{
    class Program
    {
        private const string _dataBasePath = "../../../DataBase/flashcards.db";
        private const string _sampleDataPath = "sampledata.csv";
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
