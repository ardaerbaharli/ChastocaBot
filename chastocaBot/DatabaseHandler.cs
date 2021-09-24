using System;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace chastocaBot
{
    class DatabaseHandler
    {
        private static string connectionString;
        public static void CreateTables()
        {
            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string dbDirectory = Path.Combine(appDataPath, "Chastoca");
                dbDirectory = Path.Combine(dbDirectory, "Twitch");
                string dbPath = Path.Combine(dbDirectory, "chastocaBotTwitch.db");
                connectionString = string.Format("Data Source={0}", dbPath);
                if (!File.Exists(dbPath))
                {
                    Directory.CreateDirectory(dbDirectory);
                    SQLiteConnection.CreateFile(dbPath);

                    SQLiteConnection con = new SQLiteConnection(connectionString);
                    con.Open();

                    string sql;
                    SQLiteCommand cmd;

                    sql = @"CREATE TABLE Commands(command TEXT NOT NULL, answer TEXT NOT NULL)";
                    cmd = new SQLiteCommand(con);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    sql = @"CREATE TABLE Channels(channelName TEXT NOT NULL, status TEXT NOT NULL)";
                    cmd = new SQLiteCommand(con);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        public static string GetAnswer(string command)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                SQLiteDataReader reader;
                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Commands WHERE command = '" + command + "'";
                reader = cmd.ExecuteReader();
                string answer = "";
                while (reader.Read())
                {
                    answer = reader[1].ToString().TrimEnd();
                }

                con.Close();
                return answer;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return new string("ERROR!");
            }
        }
        public static bool AddCommand(string command, string answer)
        {
            if (!DoesExistInCommands(command))
            {
                int isSuccessful;
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"INSERT INTO Commands (command,answer) VALUES ('" + command + "','" + answer + "');";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                     LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }

        public static bool ChangeCommand(string command, string updatedCommand, string answer)
        {
            int isSuccessful;
            if (DoesExistInCommands(command))
            {
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"UPDATE Commands  SET command='" + updatedCommand + "',answer='" + answer + "' WHERE command ='" + command + "'"; // hadi inş
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                       LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool DeleteCommand(string komut)
        {
            if (DoesExistInCommands(komut))
            {
                int isSuccessful;
                SQLiteConnection con = new SQLiteConnection(connectionString);
                try
                {
                    con.Open();
                    SQLiteCommand cmd;
                    cmd = con.CreateCommand();
                    cmd.CommandText = @"DELETE from Commands WHERE command ='" + komut + "'";
                    isSuccessful = cmd.ExecuteNonQuery();
                    con.Close();
                    if (isSuccessful == 0)
                        return false;
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    con.Close();
                     LogHandler.ReportCrash(ex);
                    return false;
                }
            }
            else
                return false;
        }
        public static bool DoesExistInCommands(string command)
        {
            SQLiteConnection con = new SQLiteConnection(connectionString);
            try
            {
                con.Open();
                SQLiteCommand cmd;
                cmd = con.CreateCommand();
                cmd.CommandText = @"SELECT EXISTS(SELECT * FROM Commands WHERE command='" + command + "');";
                int commandExist = int.Parse(cmd.ExecuteScalar().ToString());
                con.Close();
                if (commandExist > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                con.Close();
                LogHandler.ReportCrash(ex);
                return false;
            }
        }
    }
}