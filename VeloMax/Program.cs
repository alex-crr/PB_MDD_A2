namespace PB_MDD_A2
{
    class Program
    {
        static void Main(string[] args)
        {

            Window.Open();

            Title title = new Title("VeloMax");
            Window.AddElement(title);
            Window.Render(title);

        Login:

            Prompt promptID = new Prompt("Enter your login ID: ");
            Window.AddElement(promptID);
            Window.ActivateElement(promptID);
            var ID = promptID.GetResponse();

            Prompt promptPassword = new Prompt("Enter your password: ");
            Window.AddElement(promptPassword);
            Window.ActivateElement(promptPassword);
            var password = promptPassword.GetResponse();

            MySqlConnection connection;

            // Bien vérifier, via Workbench par exemple, que ces paramètres de connexion sont valides !!!
            try
            {
                string connectionString = $"SERVER=localhost;PORT=3306;DATABASE=velomax;UID={ID?.Value};PASSWORD={password?.Value};";
                connection = new MySqlConnection(connectionString);
                connection.Open();

            }
            catch (MySqlException ex)
            {
                Dialog text = new Dialog(
                    new List<string>()
                    {
                        "Login failed.",
                    },
                    null,
                    "retry"
                    );
                Window.AddElement(text);
                Window.ActivateElement(text);
                goto Login;
            }

            string[] options = new string[] { "Load from table", "Insert into table", "Delete from table", "Exit" };
            ScrollingMenu menuMain = new ScrollingMenu(
                "Please choose an option among those below.",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menuMain);
            Window.ActivateElement(menuMain);
            
            var response = menuMain.GetResponse();
            Window.DeactivateElement(menuMain);

            string[] tables = Helper.GetTablesName(connection).ToArray();
            ScrollingMenu menuTables = new ScrollingMenu(
                "Please choose a table among those below.",
                0,
                Placement.TopCenter,
                tables
            );
            Window.AddElement(menuTables);
            Window.ActivateElement(menuTables);
            
            var responseTable = menuTables.GetResponse();
            Window.DeactivateElement(menuTables);

            switch (response!.Status)
            {
                case Status.Selected:
                    switch (response?.Value)
                    {
                        case 0:
                            string tableName = tables[responseTable!.Value];
                            var data = Select(connection, tableName);
                            var headers = Helper.GetColumnsName(connection, tableName);

                            TableView students = new TableView(tableName, headers, data);
                            Window.AddElement(students);
                            Window.Render(students);

                            Window.Freeze();
                            break;
                    }
                    break;
                case Status.Escaped:
                    break;
                case Status.Deleted:
                    break;
                default:
                    break;
            }


            Window.Close();

            //InsertInto(connection, "vendeur");
            //DeleteFrom(connection, "vendeur");

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

        public static void AfficheTable(MySqlConnection connection, string tableName)
        {
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

            Console.Write("Enter the id of the row you want to see: ");
            string idVal = Console.ReadLine();
            List<string> columns = Helper.GetColumnsName(connection, tableName);
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName} WHERE {columns[0]} = @idVal;";
            command.Parameters.AddWithValue("@idVal", idVal);
            MySqlDataReader readerTuple = command.ExecuteReader();
            if (readerTuple.Read())
            {
                string currentTupleAsString = "";
                for (int i = 0; i < readerTuple.FieldCount; i++)
                {
                    string valueAsString = readerTuple.GetValue(i).ToString();
                    currentTupleAsString += valueAsString + ", ";
                }
                Console.WriteLine(currentTupleAsString);
            }
            readerTuple.Close();
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