using System;
using System.Data.SqlClient;

namespace chastocaBot
{
    class DatabaseHandler
    {
        private static SqlCommand sqlCommand;
        private static SqlConnection connection;
        private static SqlDataReader reader;
        private static readonly string connecString = Start.connectionString;

        public static string FindCommand(string message)
        {
            connection = new SqlConnection
            {
                ConnectionString = connecString
            };
            connection.Open();
            sqlCommand = new SqlCommand
            {
                Connection = connection,
                CommandText = "SELECT * FROM Commands WHERE command='" + message + "'"
            };
            reader = sqlCommand.ExecuteReader();
            string answer = "";
            while (reader.Read())
            {
                answer = reader[1].ToString();
            }
            connection.Close();
            return answer;
        }
        public static bool AddCommand(string command, string answer)
        {
            if (!DoesExistInCommands(command))
            {
                int isSuccessful;
                Console.WriteLine("Adding the command into database.");
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "INSERT INTO Commands (command,answer) VALUES ('" + command + "','" + answer + "')"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                    {
                        Console.WriteLine("Couldn't add the command into database.");
                        return false;
                    }
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.HelpLink);
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
                try
                {
                    Console.WriteLine("Changing the command " + command + " to " + updatedCommand + " and the answer is " + answer);
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,

                        CommandText = "UPDATE Commands  SET command='" + updatedCommand + "',answer='" + answer + "' WHERE command ='" + command + "'"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                    {
                        Console.WriteLine("Couldn't change the command in database.");
                        return false;
                    }
                    else
                        return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static bool DeleteCommand(string komut)
        {
            if (DoesExistInCommands(komut))
            {
                int isSuccessful;
                try
                {
                    connection = new SqlConnection
                    {
                        ConnectionString = connecString
                    };
                    sqlCommand = new SqlCommand
                    {
                        Connection = connection,
                        CommandText = "DELETE FROM Commands WHERE command='" + komut + "'"
                    };
                    connection.Open();
                    isSuccessful = sqlCommand.ExecuteNonQuery();
                    connection.Close();
                    if (isSuccessful == 0)
                    {
                        Console.WriteLine("Couldn't delete the command from database.");
                        return false;
                    }
                    else
                    {
                        Console.WriteLine("Command deleted from database successfully.");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.HelpLink);
                    return false;
                }
            }
            else
                return false;

        }
        public static bool DoesExistInCommands(string command)
        {
            connection = new SqlConnection
            {
                ConnectionString = connecString
            };
            using SqlCommand checkCommand = new SqlCommand("SELECT COUNT(*) FROM Commands WHERE (command = @command)", connection);
            checkCommand.Parameters.AddWithValue("@command", command);
            connection.Open();
            int commandExist = (int)checkCommand.ExecuteScalar();
            connection.Close();
            if (commandExist > 0)
            {
                Console.WriteLine("Command is in the database.");
                return true;
            }
            else
            {
                Console.WriteLine("Command is not in the database.");
                return false;
            }

        }
    }
}