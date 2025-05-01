using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Data.Models.Tennis;

namespace MatchDataProcessor.Services
{
    public static class MatchIngester
    {
        /// <summary>
        /// Ensures the tennis_matches table exists in the SQLite database. 
        /// Creates the table if it doesn't already exist.
        /// </summary>
        /// <param name="connection">An open SQLite database connection.</param>
        public static void EnsureTableExists(SQLiteConnection connection)
        {
            string createTableSql = @"
                CREATE TABLE tennis_matches (
                    tourney_id TEXT,
                    tourney_name TEXT,
                    surface TEXT,
                    draw_size INTEGER,
                    tourney_level TEXT,
                    tourney_date DATE,
                    match_num INTEGER,
                    winner_id INTEGER,
                    winner_seed TEXT,
                    winner_entry TEXT,
                    winner_name TEXT,
                    winner_hand TEXT,
                    winner_ht INTEGER,
                    winner_ioc TEXT,
                    winner_age REAL,
                    loser_id INTEGER,
                    loser_seed TEXT,
                    loser_entry TEXT,
                    loser_name TEXT,
                    loser_hand TEXT,
                    loser_ht INTEGER,
                    loser_ioc TEXT,
                    loser_age REAL,
                    score TEXT,
                    best_of INTEGER,
                    round TEXT,
                    minutes INTEGER,
                    w_ace INTEGER,
                    w_df INTEGER,
                    w_svpt INTEGER,
                    w_1stIn INTEGER,
                    w_1stWon INTEGER,
                    w_2ndWon INTEGER,
                    w_SvGms INTEGER,
                    w_bpSaved INTEGER,
                    w_bpFaced INTEGER,
                    l_ace INTEGER,
                    l_df INTEGER,
                    l_svpt INTEGER,
                    l_1stIn INTEGER,
                    l_1stWon INTEGER,
                    l_2ndWon INTEGER,
                    l_SvGms INTEGER,
                    l_bpSaved INTEGER,
                    l_bpFaced INTEGER,
                    winner_rank INTEGER,
                    winner_rank_points INTEGER,
                    loser_rank INTEGER,
                    loser_rank_points INTEGER
                );
            ";

            using (var createCmd = new SQLiteCommand(createTableSql, connection))
            {
                createCmd.ExecuteNonQuery();
                Console.WriteLine("Created fresh tennis_matches table.");
            }
        }

        /// <summary>
        /// Inserts a list of <see cref="Match"/> records into the tennis_matches table.
        /// Duplicate entries (based on primary key) are ignored.
        /// </summary>
        /// <param name="connection">An open SQLite database connection.</param>
        /// <param name="matches">A list of <see cref="Match"/> objects to be inserted.</param>
        public static void IngestMatches(SQLiteConnection connection, List<Match> matches)
        {
            using var transaction = connection.BeginTransaction();
            using var command = new SQLiteCommand(connection);

            command.CommandText = @"
                INSERT INTO tennis_matches VALUES (
                    @tourney_id, @tourney_name, @surface, @draw_size, @tourney_level, @tourney_date, @match_num,
                    @winner_id, @winner_seed, @winner_entry, @winner_name, @winner_hand, @winner_ht, @winner_ioc, @winner_age,
                    @loser_id, @loser_seed, @loser_entry, @loser_name, @loser_hand, @loser_ht, @loser_ioc, @loser_age,
                    @score, @best_of, @round, @minutes,
                    @w_ace, @w_df, @w_svpt, @w_1stIn, @w_1stWon, @w_2ndWon, @w_SvGms, @w_bpSaved, @w_bpFaced,
                    @l_ace, @l_df, @l_svpt, @l_1stIn, @l_1stWon, @l_2ndWon, @l_SvGms, @l_bpSaved, @l_bpFaced,
                    @winner_rank, @winner_rank_points, @loser_rank, @loser_rank_points
                );
            ";

            string[] paramNames = new[] {
                "@tourney_id", "@tourney_name", "@surface", "@draw_size", "@tourney_level", "@tourney_date", "@match_num",
                "@winner_id", "@winner_seed", "@winner_entry", "@winner_name", "@winner_hand", "@winner_ht", "@winner_ioc", "@winner_age",
                "@loser_id", "@loser_seed", "@loser_entry", "@loser_name", "@loser_hand", "@loser_ht", "@loser_ioc", "@loser_age",
                "@score", "@best_of", "@round", "@minutes",
                "@w_ace", "@w_df", "@w_svpt", "@w_1stIn", "@w_1stWon", "@w_2ndWon", "@w_SvGms", "@w_bpSaved", "@w_bpFaced",
                "@l_ace", "@l_df", "@l_svpt", "@l_1stIn", "@l_1stWon", "@l_2ndWon", "@l_SvGms", "@l_bpSaved", "@l_bpFaced",
                "@winner_rank", "@winner_rank_points", "@loser_rank", "@loser_rank_points"
            };

            foreach (var name in paramNames)
            {
                command.Parameters.Add(new SQLiteParameter(name));
            }

            foreach (var match in matches)
            {
                command.Parameters["@tourney_id"].Value = match.TourneyId;
                command.Parameters["@tourney_name"].Value = match.TourneyName;
                command.Parameters["@surface"].Value = match.Surface;
                command.Parameters["@draw_size"].Value = (object?)match.DrawSize ?? DBNull.Value;
                command.Parameters["@tourney_level"].Value = match.TourneyLevel;
                command.Parameters["@tourney_date"].Value = match.TourneyDate;
                command.Parameters["@match_num"].Value = match.MatchNum;

                command.Parameters["@winner_id"].Value = match.WinnerId;
                command.Parameters["@winner_seed"].Value = match.WinnerSeed;
                command.Parameters["@winner_entry"].Value = match.WinnerEntry;
                command.Parameters["@winner_name"].Value = match.WinnerName;
                command.Parameters["@winner_hand"].Value = match.WinnerHand;
                command.Parameters["@winner_ht"].Value = (object?)match.WinnerHeight ?? DBNull.Value;
                command.Parameters["@winner_ioc"].Value = match.WinnerIoc;
                command.Parameters["@winner_age"].Value = (object?)match.WinnerAge ?? DBNull.Value;

                command.Parameters["@loser_id"].Value = match.LoserId;
                command.Parameters["@loser_seed"].Value = match.LoserSeed;
                command.Parameters["@loser_entry"].Value = match.LoserEntry;
                command.Parameters["@loser_name"].Value = match.LoserName;
                command.Parameters["@loser_hand"].Value = match.LoserHand;
                command.Parameters["@loser_ht"].Value = (object?)match.LoserHeight ?? DBNull.Value;
                command.Parameters["@loser_ioc"].Value = match.LoserIoc;
                command.Parameters["@loser_age"].Value = (object?)match.LoserAge ?? DBNull.Value;

                command.Parameters["@score"].Value = match.Score;
                command.Parameters["@best_of"].Value = match.BestOf;
                command.Parameters["@round"].Value = match.Round;
                command.Parameters["@minutes"].Value = (object?)match.Minutes ?? DBNull.Value;

                command.Parameters["@w_ace"].Value = (object?)match.WAce ?? DBNull.Value;
                command.Parameters["@w_df"].Value = (object?)match.WDoubleFault ?? DBNull.Value;
                command.Parameters["@w_svpt"].Value = (object?)match.WServePoints ?? DBNull.Value;
                command.Parameters["@w_1stIn"].Value = (object?)match.WFirstIn ?? DBNull.Value;
                command.Parameters["@w_1stWon"].Value = (object?)match.WFirstWon ?? DBNull.Value;
                command.Parameters["@w_2ndWon"].Value = (object?)match.WSecondWon ?? DBNull.Value;
                command.Parameters["@w_SvGms"].Value = (object?)match.WServiceGames ?? DBNull.Value;
                command.Parameters["@w_bpSaved"].Value = (object?)match.WBpSaved ?? DBNull.Value;
                command.Parameters["@w_bpFaced"].Value = (object?)match.WBpFaced ?? DBNull.Value;

                command.Parameters["@l_ace"].Value = (object?)match.LAce ?? DBNull.Value;
                command.Parameters["@l_df"].Value = (object?)match.LDoubleFault ?? DBNull.Value;
                command.Parameters["@l_svpt"].Value = (object?)match.LServePoints ?? DBNull.Value;
                command.Parameters["@l_1stIn"].Value = (object?)match.LFirstIn ?? DBNull.Value;
                command.Parameters["@l_1stWon"].Value = (object?)match.LFirstWon ?? DBNull.Value;
                command.Parameters["@l_2ndWon"].Value = (object?)match.LSecondWon ?? DBNull.Value;
                command.Parameters["@l_SvGms"].Value = (object?)match.LServiceGames ?? DBNull.Value;
                command.Parameters["@l_bpSaved"].Value = (object?)match.LBpSaved ?? DBNull.Value;
                command.Parameters["@l_bpFaced"].Value = (object?)match.LBpFaced ?? DBNull.Value;

                command.Parameters["@winner_rank"].Value = (object?)match.WinnerRank ?? DBNull.Value;
                command.Parameters["@winner_rank_points"].Value = (object?)match.WinnerRankPoints ?? DBNull.Value;
                command.Parameters["@loser_rank"].Value = (object?)match.LoserRank ?? DBNull.Value;
                command.Parameters["@loser_rank_points"].Value = (object?)match.LoserRankPoints ?? DBNull.Value;

                command.ExecuteNonQuery();
            }

            transaction.Commit();
            Console.WriteLine("All match data inserted successfully.");
        }
    }
}
