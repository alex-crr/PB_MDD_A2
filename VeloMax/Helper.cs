using Microsoft.VisualBasic;

namespace PB_MDD_A2
{
    public class Helper
    {
        public static List<string> GetColumnsName(MySqlConnection connection, string tableName)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = 'velomax' ORDER BY ORDINAL_POSITION";
            MySqlDataReader reader = command.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    columns.Add(reader.GetString(i));
                }
            }
            reader.Close();
            return columns;
        }

        public static List<string> GetTablesName(MySqlConnection connection)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SHOW TABLES";
            MySqlDataReader reader = command.ExecuteReader();
            var tables = new List<string>();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    tables.Add(reader.GetString(i));
                }
            }
            reader.Close();
            return tables;
        }

        public static string GetId(MySqlConnection connection)
        {
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"select user from information_schema.processlist where id = connection_id();";
            MySqlDataReader reader = command.ExecuteReader();
            string id= "";
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    id = reader.GetString(i);
                }
            }
            reader.Close();
            return id;
        }

        public static int ConfirmationMenu(string message)
        {
            string[] options = new string[] { "No", "Definitely not", "Yes" };
            ScrollingMenu menu = new ScrollingMenu(
                message,
                0,
                Placement.TopCenter,
                options
            );
            Window.AddElement(menu);
            Window.ActivateElement(menu);
            var responseMenu = menu.GetResponse();
            Window.DeactivateElement(menu);
            return responseMenu!.Value;

        }

        public static (string[], ConsoleAppVisuals.Models.InteractionEventArgs<int>) ChooseTable(MySqlConnection connection)
        {
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
            return (tables, responseTable);
        }

        public static (string[], ConsoleAppVisuals.Models.InteractionEventArgs<int>) ChooseColumn(MySqlConnection connection, string tableName)
        {
            string[] columns = Helper.GetColumnsName(connection, tableName).ToArray();
            ScrollingMenu menuColumns = new ScrollingMenu(
                "Please choose a column among those below.",
                0,
                Placement.TopCenter,
                columns
            );
            Window.AddElement(menuColumns);
            Window.ActivateElement(menuColumns);

            var responseColumn = menuColumns.GetResponse();
            Window.DeactivateElement(menuColumns);
            return (columns, responseColumn);
        }

    }
}
