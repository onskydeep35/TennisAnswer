using System;
using System.Collections.Generic;
using System.IO;
using Data.Models.Tennis;
using System.Globalization;

namespace PlayerDataProcessor.Services
{
    /// <summary>
    /// Provides functionality to read and parse tennis player data from a CSV file.
    /// </summary>
    public static class PlayerProcessor
    {
        /// <summary>
        /// Reads a CSV file and converts each row into a <see cref="Player"/> object.
        /// </summary>
        /// <param name="path">The path to the CSV file.</param>
        /// <returns>A list of valid <see cref="Player"/> instances parsed from the file.</returns>
        public static List<Player> ReadFromCsv(string path)
        {
            var lines = File.ReadAllLines(path);
            var players = new List<Player>();

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');

                if (!int.TryParse(parts[0], out int playerId))
                    continue;

                DateTime? parsedDob = null;
                if (!string.IsNullOrWhiteSpace(parts[4]) &&
                    DateTime.TryParseExact(parts[4], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
                {
                    parsedDob = dob;
                }

                players.Add(new Player
                {
                    PlayerId = playerId,
                    FirstName = parts[1],
                    LastName = parts[2],
                    Hand = Constants.PreferableHandMapping.TryGetValue(parts[3], out var hand) ? hand : "Undefined",
                    Dob = parsedDob,
                    Ioc = parts[5],
                    Country = Constants.IocToCountry.TryGetValue(parts[5], out var country) ? country : "Undefined",
                    Height = string.IsNullOrWhiteSpace(parts[6]) ? null : int.Parse(parts[6]),
                    WikiDataId = parts[7]
                });
            }

            return players;
        }
    }

    /// <summary>
    /// Provides constant lookup dictionaries for mapping player attributes such as hand preference and IOC country codes.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Maps single-character hand codes to descriptive strings.
        /// </summary>
        public static readonly Dictionary<string, string> PreferableHandMapping = new()
        {
            { "L", "Left-Handed" },
            { "R", "Right-Handed" }
        };

        /// <summary>
        /// Maps IOC (Internation Olympic Commitee) country codes to full country names.
        /// </summary>
        public static readonly Dictionary<string, string> IocToCountry = new()
        {
            { "AFG", "Afghanistan" },
            { "ALB", "Albania" },
            { "ALG", "Algeria" },
            { "AND", "Andorra" },
            { "ANG", "Angola" },
            { "ANT", "Antigua and Barbuda" },
            { "ARG", "Argentina" },
            { "ARM", "Armenia" },
            { "ARU", "Aruba" },
            { "ASA", "American Samoa" },
            { "AUS", "Australia" },
            { "AUT", "Austria" },
            { "AZE", "Azerbaijan" },
            { "BAH", "Bahamas" },
            { "BAN", "Bangladesh" },
            { "BAR", "Barbados" },
            { "BDI", "Burundi" },
            { "BEL", "Belgium" },
            { "BEN", "Benin" },
            { "BER", "Bermuda" },
            { "BHU", "Bhutan" },
            { "BIH", "Bosnia and Herzegovina" },
            { "BIZ", "Belize" },
            { "BLR", "Belarus" },
            { "BOL", "Bolivia" },
            { "BOT", "Botswana" },
            { "BRA", "Brazil" },
            { "BRN", "Bahrain" },
            { "BRU", "Brunei" },
            { "BUL", "Bulgaria" },
            { "BUR", "Burkina Faso" },
            { "CAF", "Central African Republic" },
            { "CAM", "Cambodia" },
            { "CAN", "Canada" },
            { "CAY", "Cayman Islands" },
            { "CGO", "Congo" },
            { "CHA", "Chad" },
            { "CHI", "Chile" },
            { "CHN", "China" },
            { "CIV", "Côte d'Ivoire" },
            { "CMR", "Cameroon" },
            { "COD", "Democratic Republic of the Congo" },
            { "COK", "Cook Islands" },
            { "COL", "Colombia" },
            { "COM", "Comoros" },
            { "CPV", "Cape Verde" },
            { "CRC", "Costa Rica" },
            { "CRO", "Croatia" },
            { "CUB", "Cuba" },
            { "CYP", "Cyprus" },
            { "CZE", "Czech Republic" },
            { "DEN", "Denmark" },
            { "DJI", "Djibouti" },
            { "DMA", "Dominica" },
            { "DOM", "Dominican Republic" },
            { "ECU", "Ecuador" },
            { "EGY", "Egypt" },
            { "ERI", "Eritrea" },
            { "ESA", "El Salvador" },
            { "ESP", "Spain" },
            { "EST", "Estonia" },
            { "ETH", "Ethiopia" },
            { "FIJ", "Fiji" },
            { "FIN", "Finland" },
            { "FRA", "France" },
            { "FSM", "Micronesia" },
            { "GAB", "Gabon" },
            { "GAM", "Gambia" },
            { "GBR", "United Kingdom" },
            { "GBS", "Guinea-Bissau" },
            { "GEO", "Georgia" },
            { "GEQ", "Equatorial Guinea" },
            { "GER", "Germany" },
            { "GHA", "Ghana" },
            { "GRE", "Greece" },
            { "GRN", "Grenada" },
            { "GUA", "Guatemala" },
            { "GUI", "Guinea" },
            { "GUM", "Guam" },
            { "GUY", "Guyana" },
            { "HAI", "Haiti" },
            { "HKG", "Hong Kong" },
            { "HON", "Honduras" },
            { "HUN", "Hungary" },
            { "INA", "Indonesia" },
            { "IND", "India" },
            { "IRI", "Iran" },
            { "IRL", "Ireland" },
            { "IRQ", "Iraq" },
            { "ISL", "Iceland" },
            { "ISR", "Israel" },
            { "ISV", "Virgin Islands" },
            { "ITA", "Italy" },
            { "IVB", "British Virgin Islands" },
            { "JAM", "Jamaica" },
            { "JOR", "Jordan" },
            { "JPN", "Japan" },
            { "KAZ", "Kazakhstan" },
            { "KEN", "Kenya" },
            { "KGZ", "Kyrgyzstan" },
            { "KIR", "Kiribati" },
            { "KOR", "South Korea" },
            { "KOS", "Kosovo" },
            { "KSA", "Saudi Arabia" },
            { "KUW", "Kuwait" },
            { "LAO", "Laos" },
            { "LAT", "Latvia" },
            { "LBA", "Libya" },
            { "LBN", "Lebanon" },
            { "LBR", "Liberia" },
            { "LCA", "Saint Lucia" },
            { "LES", "Lesotho" },
            { "LIB", "Lebanon" },
            { "LIE", "Liechtenstein" },
            { "LTU", "Lithuania" },
            { "LUX", "Luxembourg" },
            { "MAD", "Madagascar" },
            { "MAR", "Morocco" },
            { "MAS", "Malaysia" },
            { "MAW", "Malawi" },
            { "MDA", "Moldova" },
            { "MDV", "Maldives" },
            { "MEX", "Mexico" },
            { "MGL", "Mongolia" },
            { "MKD", "North Macedonia" },
            { "MLT", "Malta" },
            { "MNE", "Montenegro" },
            { "MON", "Monaco" },
            { "MOZ", "Mozambique" },
            { "MRI", "Mauritius" },
            { "MTN", "Mauritania" },
            { "MYA", "Myanmar" },
            { "NAM", "Namibia" },
            { "NCA", "Nicaragua" },
            { "NED", "Netherlands" },
            { "NEP", "Nepal" },
            { "NGR", "Nigeria" },
            { "NIG", "Niger" },
            { "NOR", "Norway" },
            { "NRU", "Nauru" },
            { "NZL", "New Zealand" },
            { "OMA", "Oman" },
            { "PAK", "Pakistan" },
            { "PAN", "Panama" },
            { "PAR", "Paraguay" },
            { "PER", "Peru" },
            { "PHI", "Philippines" },
            { "PLE", "Palestine" },
            { "PLW", "Palau" },
            { "PNG", "Papua New Guinea" },
            { "POL", "Poland" },
            { "POR", "Portugal" },
            { "PRK", "North Korea" },
            { "PUR", "Puerto Rico" },
            { "QAT", "Qatar" },
            { "ROU", "Romania" },
            { "RSA", "South Africa" },
            { "RUS", "Russia" },
            { "RWA", "Rwanda" },
            { "SAM", "Samoa" },
            { "SEN", "Senegal" },
            { "SEY", "Seychelles" },
            { "SGP", "Singapore" },
            { "SKN", "Saint Kitts and Nevis" },
            { "SLE", "Sierra Leone" },
            { "SLO", "Slovenia" },
            { "SMR", "San Marino" },
            { "SOL", "Solomon Islands" },
            { "SOM", "Somalia" },
            { "SRB", "Serbia" },
            { "SRI", "Sri Lanka" },
            { "SSD", "South Sudan" },
            { "STP", "São Tomé and Príncipe" },
            { "SUD", "Sudan" },
            { "SUI", "Switzerland" },
            { "SUR", "Suriname" },
            { "SVK", "Slovakia" },
            { "SWE", "Sweden" },
            { "SWZ", "Eswatini" },
            { "SYR", "Syria" },
            { "TAN", "Tanzania" },
            { "TGA", "Tonga" },
            { "THA", "Thailand" },
            { "TJK", "Tajikistan" },
            { "TKM", "Turkmenistan" },
            { "TLS", "Timor-Leste" },
            { "TOG", "Togo" },
            { "TPE", "Chinese Taipei" },
            { "TTO", "Trinidad and Tobago" },
            { "TUN", "Tunisia" },
            { "TUR", "Turkey" },
            { "TUV", "Tuvalu" },
            { "UAE", "United Arab Emirates" },
            { "UGA", "Uganda" },
            { "UKR", "Ukraine" },
            { "URU", "Uruguay" },
            { "USA", "United States" },
            { "UZB", "Uzbekistan" },
            { "VAN", "Vanuatu" },
            { "VEN", "Venezuela" },
            { "VIE", "Vietnam" },
            { "VIN", "Saint Vincent and the Grenadines" },
            { "YEM", "Yemen" },
            { "ZAM", "Zambia" },
            { "ZIM", "Zimbabwe" },
            { "EUN", "Unified Team" },       // Used in 1992
            { "ROC", "Russian Olympic Committee" }, // 2020 designation due to doping sanctions
            { "IOA", "Independent Olympic Athletes" }, // For banned or stateless competitors
            { "REF", "Refugee Olympic Team" } // Refugees competing under the Olympic flag
        };
    }
}
