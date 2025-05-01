# Tennis Match Data Processor

This is a .NET console application that processes historical ATP tennis match data from CSV files and ingests it into an SQLite database. It parses, validates, and transforms match records into a structured database table for further analysis or querying.

## Usage

To run the application:

```bash
dotnet build
dotnet run
```

This will:

- Read all files from the `Dataset/` directory with the prefix `atp_matches` (excluding doubles matches).
- Parse and ingest all valid match records into the main database.

The SQLite database will be written to the main location used by the application:

```text
../../../DataBase/flashcards.db
```

## Input CSV Format

Expected headers (49 columns):

```text
tourney_id,tourney_name,surface,draw_size,tourney_level,tourney_date,match_num,
winner_id,winner_seed,winner_entry,winner_name,winner_hand,winner_ht,winner_ioc,winner_age,
loser_id,loser_seed,loser_entry,loser_name,loser_hand,loser_ht,loser_ioc,loser_age,
score,best_of,round,minutes,
w_ace,w_df,w_svpt,w_1stIn,w_1stWon,w_2ndWon,w_SvGms,w_bpSaved,w_bpFaced,
l_ace,l_df,l_svpt,l_1stIn,l_1stWon,l_2ndWon,l_SvGms,l_bpSaved,l_bpFaced,
winner_rank,winner_rank_points,loser_rank,loser_rank_points
```

Example row:

```text
2018-03,Sydney,Hard,32,ATP,20180101,1,
105909,,WC,John Smith,R,188,USA,29.5,
101234,1,,Roger Doe,R,185,ESP,31.2,
7-6(5) 6-4,3,R32,94,
10,3,65,43,32,12,12,3,5,
8,2,50,30,18,10,10,2,3,
15,1050,30,800
```

## Output

The application inserts ATP match records into the `tennis_matches` table in the flashcards SQLite database. Duplicate rows are ignored based on primary key logic (if applied in future versions).

## Table Schema

- `tourney_id` (TEXT)  
- `tourney_name` (TEXT)  
- `surface` (TEXT)  
- `draw_size` (INTEGER)  
- `tourney_level` (TEXT)  
- `tourney_date` (DATE)  
- `match_num` (INTEGER)  
- `winner_id` (INTEGER)  
- `winner_seed` (TEXT)  
- `winner_entry` (TEXT)  
- `winner_name` (TEXT)  
- `winner_hand` (TEXT)  
- `winner_ht` (INTEGER)  
- `winner_ioc` (TEXT)  
- `winner_age` (REAL)  
- `loser_id` (INTEGER)  
- `loser_seed` (TEXT)  
- `loser_entry` (TEXT)  
- `loser_name` (TEXT)  
- `loser_hand` (TEXT)  
- `loser_ht` (INTEGER)  
- `loser_ioc` (TEXT)  
- `loser_age` (REAL)  
- `score` (TEXT)  
- `best_of` (INTEGER)  
- `round` (TEXT)  
- `minutes` (INTEGER)  
- `w_ace`, `w_df`, `w_svpt`, `w_1stIn`, `w_1stWon`, `w_2ndWon`, `w_SvGms`, `w_bpSaved`, `w_bpFaced` (INTEGER)  
- `l_ace`, `l_df`, `l_svpt`, `l_1stIn`, `l_1stWon`, `l_2ndWon`, `l_SvGms`, `l_bpSaved`, `l_bpFaced` (INTEGER)  
- `winner_rank`, `winner_rank_points`, `loser_rank`, `loser_rank_points` (INTEGER)

## Data Source

The historical ATP match data [can be downloaded from here](https://www.kaggle.com/datasets/guillemservera/tennis)

This dataset includes ATP-level match records from various tournaments around the world, including metadata such as surface, scores, serve stats, and player rankings.

