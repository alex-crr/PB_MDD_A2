using System;

namespace PB_MDD_A2
{
    public class Command
    {
        /// <summary>
        /// This function asks the user (through the console) to enter a value for each column of the selected table in order to insert a new row.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// //ajouter un check pour les variables null, plusvérifiier les types
        public static void InsertInto(MySqlConnection connection, string tableName, List<string> values)
        {
            MySqlCommand command = connection.CreateCommand();
            string query = $"INSERT INTO {tableName} VALUES ({string.Join(", ", values.Select(v => $"'{v}'"))});";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        public static void DeleteFrom(MySqlConnection connection, string tableName, string idVal)
        {
            List<string> columns = Helper.GetColumnsName(connection, tableName);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM {tableName} WHERE {columns[0]} = {idVal};";
            // Handle dependencies
            try
            {
                command.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1451) // 1451 is the error number for a foreign key constraint failure
                {
                    Console.WriteLine("This row has dependencies in other tables. Deleting those rows as well.");
                    MySqlCommand commandCascade = connection.CreateCommand();
                    commandCascade.CommandText = $"DELETE FROM {tableName} WHERE {columns[0]} = {idVal} CASCADE;";
                    commandCascade.ExecuteNonQuery();
                }
                else
                {
                    throw;
                }
            }
        }

        public static void UpdateRow(MySqlConnection connection, string tableName, string idVal, string column, string value)
        {
            List<string> columns = Helper.GetColumnsName(connection, tableName);
            MySqlCommand command = connection.CreateCommand();
            string query = $"UPDATE {tableName} SET {column} = '{value}' WHERE {columns[0]} = {idVal};";
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        public static List<List<string>> Select(MySqlConnection connection, string tableName)
        {
            MySqlCommand commandShow = connection.CreateCommand();
            commandShow.CommandText = $"SELECT * FROM {tableName};";
            MySqlDataReader reader = commandShow.ExecuteReader();

            List<List<string>> tableData = new List<List<string>>();
            while (reader.Read())
            {
                List<string> rowData = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string valueAsString = reader.GetValue(i).ToString();
                    rowData.Add(valueAsString);
                }
                tableData.Add(rowData);
            }
            reader.Close();
            return tableData;


        }

    }
}
