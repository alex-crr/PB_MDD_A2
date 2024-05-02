using YamlDotNet.Core;

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

            string[] options = new string[] { "Load from table", "Insert into table", "Update from table", "Delete from table", "Exit" };
            ScrollingMenu menuMain = new ScrollingMenu(
                "Please choose an option among those below.",
                0,
                Placement.TopCenter,
                options
            );
            Window.AddElement(menuMain);

        Main:
            Window.DeactivateAllElements();
            Window.Render(title);
            Window.ActivateElement(menuMain);

            var response = menuMain.GetResponse();
            Window.DeactivateElement(menuMain);

            switch (response!.Status)
            {
                case Status.Selected:
                    {
                        switch (response!.Value)
                        {
                            case 0: // Load from table
                                var responseTable = Helper.ChooseTable(connection);
                            SeeTable:
                                string tableName = responseTable.Item1[responseTable!.Item2.Value];
                                var data = Select(connection, tableName);
                                var headers = Helper.GetColumnsName(connection, tableName);

                                TableView seeTable = new TableView(tableName, headers, data);
                                Window.AddElement(seeTable);

                                Window.Render(seeTable);

                                Window.Freeze();
                                goto Main;

                            case 1: // Insert into table
                                responseTable = Helper.ChooseTable(connection);
                            Insertion:
                                Prompt promptInsert;
                                List<string> values = new List<string>();
                                tableName = responseTable.Item1[responseTable!.Item2.Value];
                                data = Select(connection, tableName);
                                headers = Helper.GetColumnsName(connection, tableName);

                                seeTable = new TableView($"Example for {tableName}", headers, new List<List<string>> { data[0].ToList() });
                                Window.AddElement(seeTable);
                                Window.Render(seeTable);
                                foreach (var header in headers)
                                {
                                    promptInsert = new Prompt($"Enter value for {header}: ");

                                    Window.AddElement(promptInsert);
                                    Window.ActivateElement(promptInsert);
                                    values.Add(promptInsert.GetResponse()?.Value?.Replace(",", ".") ?? string.Empty);
                                }
                                try
                                {
                                    Window.DeactivateElement(seeTable);
                                    InsertInto(connection, tableName, values);
                                    goto SeeTable;
                                }
                                catch (MySqlException)
                                {
                                    Dialog text = new Dialog(
                                        new List<string>()
                                        {
                                            "Insertion failed.",
                                        },
                                        null,
                                        "retry"
                                        );
                                    Window.AddElement(text);
                                    Window.ActivateElement(text);
                                    goto Insertion;
                                }

                            case 2:// Alter from table
                                responseTable = Helper.ChooseTable(connection);
                            Update:
                                tableName = responseTable.Item1[responseTable!.Item2.Value];
                                data = Select(connection, tableName);
                                headers = Helper.GetColumnsName(connection, tableName);

                                TableSelector selectAlterTable = new TableSelector(tableName, headers, data);
                                Window.AddElement(selectAlterTable);
                                Window.ActivateElement(selectAlterTable);

                                var responseAlter = selectAlterTable.GetResponse();

                                var responseColumn = Helper.ChooseColumn(connection, tableName);

                                string columnName = responseColumn.Item1[responseColumn!.Item2.Value];
                                Prompt promptUpdate = new Prompt($"Enter new value for {columnName}: ");
                                Window.AddElement(promptUpdate);
                                Window.ActivateElement(promptUpdate);
                                var responseUpdate = promptUpdate.GetResponse();
                                try
                                {
                                    Update(connection, tableName, data[responseAlter!.Value][0], columnName, responseUpdate!.Value);
                                    goto SeeTable;
                                }
                                catch (MySqlException)
                                {
                                    Dialog text = new Dialog(
                                        new List<string>()
                                        {
                                            "Update failed.",
                                        },
                                        null,
                                        "retry"
                                        );
                                    Window.AddElement(text);
                                    Window.ActivateElement(text);
                                    goto Update;
                                }
                            case 3:// Delete from table
                                responseTable = Helper.ChooseTable(connection);
                                tableName = responseTable.Item1[responseTable!.Item2.Value];
                                data = Select(connection, tableName);
                                headers = Helper.GetColumnsName(connection, tableName);

                                TableSelector selectDeletionTable = new TableSelector(tableName, headers, data);
                                Window.AddElement(selectDeletionTable);
                                Window.ActivateElement(selectDeletionTable);

                                var responseDelete = selectDeletionTable.GetResponse();
                                if (Helper.ConfirmationMenu("Deleting this row will also delete its dependencies. Do you want to proceed?") == 2) DeleteFrom(connection, tableName, data[responseDelete!.Value][0]);

                                goto SeeTable;

                            default:
                                goto finish;
                        }


                    }
                case Status.Escaped:
                    break;

                case Status.Deleted:
                    break;
                default:
                    break;
            }

        finish:
            Window.Close();
            connection.Close();
        }

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

        public static void Update(MySqlConnection connection, string tableName, string idVal, string column, string value)
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