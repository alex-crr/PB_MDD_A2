
namespace PB_MDD_A2
{
    class Program
    {
        static void Main(string[] args)
        {


            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=velomax;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT distinct marque from location natural join voiture;"; // exemple de requete bien-sur !

            //InsertInto(connection, "vendeur");
            DeleteFrom(connection, "vendeur");

            //Helper.GetColumns(connection, "vendeur");

            connection.Close();
        }

        /// <summary>
        /// This function asks the user (through the console) to enter a value for each column of the selected table in order to insert a new row.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="tableName"></param>
        /// //ajouter un check pour les variables null, plusvérifiier les types
        public static void InsertInto(MySqlConnection connection, string tableName)
        {
            MySqlParameter _table = new MySqlParameter("@table", MySqlDbType.VarChar);
            _table.Value = tableName;

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName} LIMIT 1;";
            MySqlDataReader reader = command.ExecuteReader();
            var values = new List<string>();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"Enter value for {reader.GetName(i)}: ");
                    string value = Console.ReadLine();
                    values.Add($"'{value}'");
                }
            }
            reader.Close();

            string query = $"INSERT INTO {tableName} VALUES ({string.Join(", ", values)});";

            command = connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();
        }

        public static void DeleteFrom(MySqlConnection connection, string tableName)
        {
            // Show all rows of the table
            MySqlCommand commandShow = connection.CreateCommand();
            commandShow.CommandText = $"SELECT * FROM {tableName};";
            MySqlDataReader reader = commandShow.ExecuteReader();
            while (reader.Read())
            {
                string currentRowAsString = "";
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string valueAsString = reader.GetValue(i).ToString();
                    currentRowAsString += valueAsString + ", ";
                }
                Console.WriteLine(currentRowAsString);
            }
            reader.Close();

            Console.Write("Enter the id of the row you want to delete: ");
            string idVal = Console.ReadLine();
            List<string> columns = Helper.GetColumns(connection, tableName);
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
    }
}