using System;
using System.IO;
using System.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace CalendarProject
{
    public static class Database
    {
        // HeidiSQL info, lazy load from config
        private static string _connectionString;
        public static string CONNECTION_STRING
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    LoadConnectionString();
                }
                return _connectionString;
            }
        }

        private static void LoadConnectionString()
        {
            try
            {
                // Check if the config file exists
                string configPath = "config.ini";
                if (!File.Exists(configPath))
                {
                    // Use a default connection string for development
                    _connectionString = "Server=localhost;Port=3306;Database=calendar_db;Uid=root;Pwd=password;";
                    Debug.WriteLine("Warning: config.ini not found. Using default connection string.");
                    return;
                }

                // Read and parse the config.ini file
                string[] lines = File.ReadAllLines(configPath);
                string server = "", port = "", database = "", username = "", password = "";

                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (trimmedLine.StartsWith("Server=")) server = trimmedLine.Substring(7);
                    else if (trimmedLine.StartsWith("Port=")) port = trimmedLine.Substring(5);
                    else if (trimmedLine.StartsWith("Database=")) database = trimmedLine.Substring(9);
                    else if (trimmedLine.StartsWith("Username=")) username = trimmedLine.Substring(9);
                    else if (trimmedLine.StartsWith("Password=")) password = trimmedLine.Substring(9);
                }

                // Build the connection string
                _connectionString = $"Server={server};Port={port};Database={database};Uid={username};Pwd={password};";
                Debug.WriteLine("Connection string loaded from config.ini");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading connection string: {ex.Message}");
                
            }
        }

        private const string USERS_TABLE = "834_sp25_t6_users";
        private const string EVENTS_TABLE = "834_sp25_t6_events";
        private const string EVENTROSTER_TABLE = "834_sp25_t6_eventroster";

        // SQL queries for User
        //public static readonly string SQL_CREATE_USER_TABLE = $@"
        //    CREATE TABLE IF NOT EXISTS {USERS_TABLE} (
        //        username VARCHAR(50) PRIMARY KEY,
        //        password VARCHAR(100) NOT NULL,
        //        firstName VARCHAR(50) NOT NULL,
        //        lastName VARCHAR(50) NOT NULL,
        //        isManager BIT(1) NOT NULL DEFAULT 0
        //    );";

        //public static readonly string SQL_CREATE_EVENT_TABLE = $@"
        //    CREATE TABLE IF NOT EXISTS {EVENTS_TABLE} (
        //        eventId INT PRIMARY KEY AUTO_INCREMENT,
        //        owner VARCHAR(50) NOT NULL,
        //        name VARCHAR(100) NOT NULL,
        //        startDateTime DATETIME NOT NULL,
        //        endDateTime DATETIME NOT NULL,
        //        isMeeting BIT(1) NOT NULL DEFAULT 0,
        //        description TEXT,
        //        location VARCHAR(100),
        //        FOREIGN KEY (owner) REFERENCES {USERS_TABLE}(username)
        //    );";

        //public static readonly string SQL_CREATE_EVENTROSTER_TABLE = $@"
        //    CREATE TABLE IF NOT EXISTS {EVENTROSTER_TABLE} (
        //        eventId INT,
        //        username VARCHAR(50),
        //        PRIMARY KEY (eventId, username),
        //        FOREIGN KEY (eventId) REFERENCES {EVENTS_TABLE}(eventId) ON DELETE CASCADE,
        //        FOREIGN KEY (username) REFERENCES {USERS_TABLE}(username)
        //    );";

        // User CRUD
        public static readonly string SQL_INSERT_USER =
            $"INSERT INTO {USERS_TABLE} (username, password, firstName, lastName, isManager) VALUES (@username, @password, @firstName, @lastName, @isManager);";

        public static readonly string SQL_GET_ALL_USERS =
            $"SELECT * FROM {USERS_TABLE};";

        public static readonly string SQL_GET_USER_BY_USERNAME =
            $"SELECT * FROM {USERS_TABLE} WHERE username = @username;";

        public static readonly string SQL_UPDATE_USER =
            $"UPDATE {USERS_TABLE} SET firstName = @firstName, lastName = @lastName, password = @password, isManager = @isManager WHERE username = @username;";

        public static readonly string SQL_DELETE_USER =
            $"DELETE FROM {USERS_TABLE} WHERE username = @username;";

        // Event CRUD
        public static readonly string SQL_INSERT_EVENT =
            $"INSERT INTO {EVENTS_TABLE} (owner, name, startDateTime, endDateTime, isMeeting) " +
            $"VALUES (@owner, @name, @startDateTime, @endDateTime, @isMeeting);";

        public static readonly string SQL_GET_LAST_INSERT_ID =
            "SELECT LAST_INSERT_ID();";

        public static readonly string SQL_GET_EVENTS_BY_OWNER =
            $"SELECT * FROM {EVENTS_TABLE} WHERE owner = @owner;";

        public static readonly string SQL_GET_EVENTS_BY_DATE_RANGE =
            $@"SELECT * FROM {EVENTS_TABLE} WHERE owner = @owner 
               AND ((startDateTime BETWEEN @startDate AND @endDate) 
                    OR (endDateTime BETWEEN @startDate AND @endDate) 
                    OR (@startDate BETWEEN startDateTime AND endDateTime));";

        public static readonly string SQL_GET_EVENTS_BY_MONTH =
            $"SELECT * FROM {EVENTS_TABLE} WHERE owner = @owner AND MONTH(startDateTime) = @month AND YEAR(startDateTime) = @year;";

        public static readonly string SQL_UPDATE_EVENT =
            $@"UPDATE {EVENTS_TABLE} 
               SET name = @name, startDateTime = @startDateTime, 
                   endDateTime = @endDateTime, isMeeting = @isMeeting, location = @location 
               WHERE eventId = @eventId AND owner = @owner;";

        public static readonly string SQL_DELETE_EVENT =
            $"DELETE FROM {EVENTS_TABLE} WHERE eventId = @eventId AND owner = @owner;";

        // EventRoster
        public static readonly string SQL_INSERT_ROSTER_ENTRY =
            $"INSERT INTO {EVENTROSTER_TABLE} (eventId, username) VALUES (@eventId, @username);";

        public static readonly string SQL_GET_ROSTER_BY_EVENT =
            $"SELECT username FROM {EVENTROSTER_TABLE} WHERE eventId = @eventId;";

        public static readonly string SQL_GET_EVENTS_FOR_ATTENDEE =
            $@"SELECT e.* FROM {EVENTS_TABLE} e 
               JOIN {EVENTROSTER_TABLE} r ON e.eventId = r.eventId
               WHERE r.username = @username;";

        public static readonly string SQL_DELETE_ROSTER_ENTRIES =
            $"DELETE FROM {EVENTROSTER_TABLE} WHERE eventId = @eventId;";

        public static bool TestConnection()
        {
            using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Connection test failed: " + ex.Message);
                    return false;
                }
            }
        }

        public static void InitializeDatabase()
        {
            using (MySqlConnection conn = new MySqlConnection(CONNECTION_STRING))
            {
                try
                {
                    Debug.WriteLine("Connecting to MySQL...");
                    conn.Open();

                    CheckTablesExist(conn);

                    Debug.WriteLine("Database connection successful. Tables already exist.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error connecting to database: " + ex.ToString());
                    throw;
                }
            }
        }

        // unneeded since tables are alrady there and seeded
        //try
        //{
        //    Debug.WriteLine("Connecting to MySQL...");
        //    conn.Open();

        //    // Create tables
        //    ExecuteNonQuery(conn, SQL_CREATE_USER_TABLE);
        //    ExecuteNonQuery(conn, SQL_CREATE_EVENT_TABLE);
        //    ExecuteNonQuery(conn, SQL_CREATE_EVENTROSTER_TABLE);

        //    Debug.WriteLine("Database initialized successfully.");
        //}
        //catch (Exception ex)
        //{
        //    Debug.WriteLine("Error initializing database: " + ex.ToString());
        //    throw;
        //}
        //finally
        //{
        //    conn.Close();
        //}

        private static void CheckTablesExist(MySqlConnection conn)
        {
            // make sure the tables exist
            string checkTablesQuery = "SHOW TABLES LIKE '834_sp25_t6_%';";
            using (MySqlCommand cmd = new MySqlCommand(checkTablesQuery, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    List<string> tables = new List<string>();
                    while (reader.Read())
                    {
                        tables.Add(reader[0].ToString());
                    }

                    Console.WriteLine($"Found {tables.Count} tables:");
                    foreach (string table in tables)
                    {
                        Console.WriteLine($"- {table}");
                    }
                }
            }
        }
        //private static void ExecuteNonQuery(MySqlConnection conn, string sql, MySqlParameter[] parameters = null)
        //{
        //    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
        //    {
        //        if (parameters != null)
        //        {
        //            cmd.Parameters.AddRange(parameters);
        //        }
        //        cmd.ExecuteNonQuery();
        //    }
        //}
    }
}