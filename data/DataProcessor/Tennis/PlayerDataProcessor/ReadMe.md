# Tennis Player Data Processor

This is a .NET console application that processes historical ATP tennis player data from a CSV file and ingests it into an SQLite database. It validates, filters, and transforms player data to populate a clean and structured database table.

## Usage

To run the application with a custom CSV path:

> dotnet build
> dotnet run -- "path/to/your.csv"

To run the application with the default sample data (`sampleData.csv`):

> dotnet run

The SQLite database will be written to main database for the application:

`../../../DataBase/flashcards.db`

## Input CSV Format

Expected headers:

`player_id,name_first,name_last,hand,dob,ioc,height,wikidata_id`

Example row:

`100001,Gardnar,Mulloy,R,19131122,USA,185,Q54544`

- `dob` must be in `yyyyMMdd` format  
- `hand` is either `R` or `L`  
- `ioc` must be a valid IOC country code

## Output

The application creates or recreates a table named `tennis_players` inside the SQLite database. All valid rows from the CSV are inserted.

## Table Schema

- `player_id` (INTEGER, PRIMARY KEY)  
- `first_name` (TEXT)  
- `last_name` (TEXT)  
- `hand` (TEXT)  
- `date_of_birth` (DATE)  
- `ioc` (TEXT)  
- `country` (TEXT)  
- `height` (INTEGER)  
- `wikidata_id` (TEXT)


