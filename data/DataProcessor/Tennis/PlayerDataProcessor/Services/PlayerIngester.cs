using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Data.Models.Tennis;

namespace PlayerDataProcessor.Services
{
    /// <summary>
    /// Provides functionality to create and populate the tennis_players table in an SQLite database.
    /// </summary>
    public static class PlayerIngester
    {
        /// <summary>
        /// Ensures the tennis_players table exists in the SQLite database. 
        /// Creates the table if it doesn't already exist.
        /// </summary>
        /// <param name="connection">An open SQLite database connection.</param>
        public static void EnsureTableExists(SQLiteConnection connection)
        {
            string createTableSql = @"
                CREATE TABLE tennis_players (
                    player_id INTEGER PRIMARY KEY,
                    first_name TEXT NOT NULL,
                    last_name TEXT NOT NULL,
                    hand TEXT,
                    date_of_birth DATE,
                    ioc TEXT,
                    country TEXT,
                    height INTEGER,
                    wikidata_id TEXT
                );
            ";

            using (var createCmd = new SQLiteCommand(createTableSql, connection))
            {
                createCmd.ExecuteNonQuery();
                Console.WriteLine("Created fresh tennis_players table.");
            }
        }

        /// <summary>
        /// Inserts a list of <see cref="Player"/> records into the tennis_players table.
        /// Duplicate entries (based on primary key) are ignored.
        /// </summary>
        /// <param name="connection">An open SQLite database connection.</param>
        /// <param name="players">A list of <see cref="Player"/> objects to be inserted.</param>
        public static void IngestPlayers(SQLiteConnection connection, List<Player> players)
        {
            using var transaction = connection.BeginTransaction();
            using var command = new SQLiteCommand(connection);

            command.CommandText = @"
                INSERT OR IGNORE INTO tennis_players 
                (player_id, first_name, last_name, hand, date_of_birth, ioc, country, height, wikidata_id) 
                VALUES (@player_id, @first_name, @last_name, @hand, @date_of_birth, @ioc, @country, @height, @wikidata_id);
            ";

            command.Parameters.Add(new SQLiteParameter("@player_id"));
            command.Parameters.Add(new SQLiteParameter("@first_name"));
            command.Parameters.Add(new SQLiteParameter("@last_name"));
            command.Parameters.Add(new SQLiteParameter("@hand"));
            command.Parameters.Add(new SQLiteParameter("@date_of_birth"));
            command.Parameters.Add(new SQLiteParameter("@ioc"));
            command.Parameters.Add(new SQLiteParameter("@country"));
            command.Parameters.Add(new SQLiteParameter("@height"));
            command.Parameters.Add(new SQLiteParameter("@wikidata_id"));

            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];

                command.Parameters["@player_id"].Value = player.PlayerId;
                command.Parameters["@first_name"].Value = player.FirstName;
                command.Parameters["@last_name"].Value = player.LastName;
                command.Parameters["@hand"].Value = player.Hand;
                command.Parameters["@date_of_birth"].Value = player.Dob;
                command.Parameters["@ioc"].Value = player.Ioc;
                command.Parameters["@country"].Value = player.Country;
                command.Parameters["@height"].Value = player.Height.HasValue ? (object)player.Height.Value : DBNull.Value;
                command.Parameters["@wikidata_id"].Value = player.WikiDataId;

                command.ExecuteNonQuery();

                if ((i + 1) % 100 == 0)
                    Console.WriteLine($"Inserted {i + 1} players...");
            }

            transaction.Commit();
            Console.WriteLine("All player data inserted successfully.");
        }
    }
}
