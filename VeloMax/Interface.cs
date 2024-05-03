using System;


namespace PB_MDD_A2

{
    public class Interface
    {
        public static void Home(MySqlConnection connection)
        {

            Window.DeactivateAllElements();
            Window.Clear();

            Title title = new Title("VeloMax");
            Window.AddElement(title);
            Window.Render(title);
            string[] options = ["Load from table", "See some stuff", "Modify a table", "Exit"];

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

            switch (response!.Status)
            {
                case Status.Selected:
                    {
                        switch (response!.Value)
                        {
                            case 0: // Load from table
                                var responseTable = ChooseTable(connection);
                                SeeTable(connection, responseTable);
                                Home(connection);
                                break;

                            case 1: // See some stuff
                                responseTable = ChooseTable(connection);
                                Insertion(connection, responseTable);
                                Home(connection);
                                break;

                            case 2:// Modify a table
                                ModifyTable(connection);
                                Home(connection);
                                break;

                            default:
                                break;
                        }
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
            connection.Close();
        }


        public static void ModifyTable(MySqlConnection connection)
        {
            string[] options = new string[] { "Insert into table", "Update from table", "Delete from table", "Go back" };
            ScrollingMenu menuModify = new ScrollingMenu(
                "Please choose an option among those below.",
                0,
                Placement.TopCenter,
                options
            );
            Window.AddElement(menuModify);
            Window.ActivateElement(menuModify);
            var response = menuModify.GetResponse();
            Window.DeactivateElement(menuModify);

            switch (response!.Status)
            {
                case Status.Selected:
                    {
                        switch (response!.Value)
                        {

                            case 0: // Insert into table
                                var responseTable = ChooseTable(connection);
                                Insertion(connection, responseTable);
                                break;

                            case 1:// Alter from table
                                responseTable = ChooseTable(connection);
                                Update(connection, responseTable);
                                break;
                            case 2:// Delete from table
                                responseTable = ChooseTable(connection);
                                Delete(connection, responseTable);
                                break;

                            default:
                                break;
                        }

                    }
                    Home(connection);
                    break;
                case Status.Escaped:
                    break;

                case Status.Deleted:
                    break;
                default:
                    break;
            }

        }

        public static void SeeTable(MySqlConnection connection, (string[], ConsoleAppVisuals.Models.InteractionEventArgs<int>) responseTable)
        {
            string tableName = responseTable.Item1[responseTable!.Item2.Value];
            var data = Select(connection, tableName);
            var headers = GetColumnsName(connection, tableName);

            TableView seeTable = new TableView(tableName, headers, data);
            Window.AddElement(seeTable);

            Window.Render(seeTable);

            Window.Freeze();
        }

        public static void Insertion(MySqlConnection connection, (string[], ConsoleAppVisuals.Models.InteractionEventArgs<int>) responseTable)
        {
            Prompt promptInsert;
            List<string> values = new List<string>();

            string tableName = responseTable.Item1[responseTable!.Item2.Value];
            var data = Select(connection, tableName);
            var headers = Helper.GetColumnsName(connection, tableName);

            TableView seeTable = new TableView($"Example for {tableName}", headers, new List<List<string>> { data[0].ToList() });
            Window.AddElement(seeTable);
            Window.Render(seeTable);
            foreach (var header in headers)
            {
                promptInsert = new Prompt($"Enter value for {header}: ");

                Window.AddElement(promptInsert);
                Window.ActivateElement(promptInsert);
                var responseInsert = promptInsert.GetResponse();
                if (responseInsert!.Status == Status.Escaped) {Window.DeactivateElement(promptInsert);Home(connection);}
                values.Add(responseInsert?.Value?.Replace(",", ".") ?? string.Empty);
            }
            try
            {
                Window.DeactivateElement(seeTable);
                InsertInto(connection, tableName, values);
                SeeTable(connection, responseTable);
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
                Insertion(connection, responseTable);
            }
        }

        public static void Update(MySqlConnection connection, (string[], ConsoleAppVisuals.Models.InteractionEventArgs<int>) responseTable)
        {
            Console.WriteLine("Update");

            string tableName = responseTable.Item1[responseTable!.Item2.Value];
            var data = Select(connection, tableName);
            var headers = Helper.GetColumnsName(connection, tableName);

            TableSelector selectAlterTable = new TableSelector(tableName, headers, data);
            Window.AddElement(selectAlterTable);
            Window.ActivateElement(selectAlterTable);

            var responseAlter = selectAlterTable.GetResponse();
            if (responseAlter!.Status == Status.Escaped) Home(connection);

            var responseColumn = ChooseColumn(connection, tableName);
            if (responseColumn!.Item2.Status == Status.Escaped) Home(connection);

            string columnName = responseColumn.Item1[responseColumn!.Item2.Value];
            Prompt promptUpdate = new Prompt($"Enter new value for {columnName}: ");
            Window.AddElement(promptUpdate);
            Window.ActivateElement(promptUpdate);
            var responseUpdate = promptUpdate.GetResponse();
            if (responseUpdate!.Status == Status.Escaped) Home(connection);
            try
            {
                UpdateRow(connection, tableName, data[responseAlter!.Value][0], columnName, responseUpdate!.Value);

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
            }
            finally
            {
                Update(connection, responseTable);
            }
        }

        public static void Delete(MySqlConnection connection, (string[], ConsoleAppVisuals.Models.InteractionEventArgs<int>) responseTable)
        {
            string tableName = responseTable.Item1[responseTable!.Item2.Value];
            var data = Select(connection, tableName);
            var headers = Helper.GetColumnsName(connection, tableName);

            TableSelector selectDeletionTable = new TableSelector(tableName, headers, data);
            Window.AddElement(selectDeletionTable);
            Window.ActivateElement(selectDeletionTable);

            var responseDelete = selectDeletionTable.GetResponse();
            if (responseDelete!.Status == Status.Escaped) Home(connection);
            if (Helper.ConfirmationMenu("Deleting this row will also delete its dependencies. Do you want to proceed?") == 2) DeleteFrom(connection, tableName, data[responseDelete!.Value][0]);

            SeeTable(connection, responseTable);
        }
    }
}
