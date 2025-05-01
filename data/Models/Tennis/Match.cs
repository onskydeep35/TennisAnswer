using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Tennis
{
    [Table("tennis_matches")]
    public class Match
    {
        [Column("tourney_id")]
        public string TourneyId { get; set; }

        [Column("tourney_name")]
        public string TourneyName { get; set; }

        [Column("surface")]
        public string Surface { get; set; }

        [Column("draw_size")]
        public int? DrawSize { get; set; }

        [Column("tourney_level")]
        public string TourneyLevel { get; set; }

        [Column("tourney_date")]
        public DateTime? TourneyDate { get; set; }

        [Column("match_num")]
        public int MatchNum { get; set; }

        [Column("winner_id")]
        public int WinnerId { get; set; }

        [Column("winner_seed")]
        public string WinnerSeed { get; set; }

        [Column("winner_entry")]
        public string WinnerEntry { get; set; }

        [Column("winner_name")]
        public string WinnerName { get; set; }

        [Column("winner_hand")]
        public string WinnerHand { get; set; }

        [Column("winner_ht")]
        public int? WinnerHeight { get; set; }

        [Column("winner_ioc")]
        public string WinnerIoc { get; set; }

        [Column("winner_age")]
        public double? WinnerAge { get; set; }

        [Column("loser_id")]
        public int LoserId { get; set; }

        [Column("loser_seed")]
        public string LoserSeed { get; set; }

        [Column("loser_entry")]
        public string LoserEntry { get; set; }

        [Column("loser_name")]
        public string LoserName { get; set; }

        [Column("loser_hand")]
        public string LoserHand { get; set; }

        [Column("loser_ht")]
        public int? LoserHeight { get; set; }

        [Column("loser_ioc")]
        public string LoserIoc { get; set; }

        [Column("loser_age")]
        public double? LoserAge { get; set; }

        [Column("score")]
        public string Score { get; set; }

        [Column("best_of")]
        public int BestOf { get; set; }

        [Column("round")]
        public string Round { get; set; }

        [Column("minutes")]
        public int? Minutes { get; set; }

        [Column("w_ace")]
        public int? WAce { get; set; }

        [Column("w_df")]
        public int? WDoubleFault { get; set; }

        [Column("w_svpt")]
        public int? WServePoints { get; set; }

        [Column("w_1stIn")]
        public int? WFirstIn { get; set; }

        [Column("w_1stWon")]
        public int? WFirstWon { get; set; }

        [Column("w_2ndWon")]
        public int? WSecondWon { get; set; }

        [Column("w_SvGms")]
        public int? WServiceGames { get; set; }

        [Column("w_bpSaved")]
        public int? WBpSaved { get; set; }

        [Column("w_bpFaced")]
        public int? WBpFaced { get; set; }

        [Column("l_ace")]
        public int? LAce { get; set; }

        [Column("l_df")]
        public int? LDoubleFault { get; set; }

        [Column("l_svpt")]
        public int? LServePoints { get; set; }

        [Column("l_1stIn")]
        public int? LFirstIn { get; set; }

        [Column("l_1stWon")]
        public int? LFirstWon { get; set; }

        [Column("l_2ndWon")]
        public int? LSecondWon { get; set; }

        [Column("l_SvGms")]
        public int? LServiceGames { get; set; }

        [Column("l_bpSaved")]
        public int? LBpSaved { get; set; }

        [Column("l_bpFaced")]
        public int? LBpFaced { get; set; }

        [Column("winner_rank")]
        public int? WinnerRank { get; set; }

        [Column("winner_rank_points")]
        public int? WinnerRankPoints { get; set; }

        [Column("loser_rank")]
        public int? LoserRank { get; set; }

        [Column("loser_rank_points")]
        public int? LoserRankPoints { get; set; }
    }
}
