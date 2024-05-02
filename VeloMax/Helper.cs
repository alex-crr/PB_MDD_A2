namespace PB_MDD_A2
{
    public class Helper
    {
         public static List<string> GetColumnsName(MySqlConnection connection, string tableName){
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = 'velomax' ORDER BY ORDINAL_POSITION";
            MySqlDataReader reader = command.ExecuteReader();
            var columns = new List<string>();
            while (reader.Read()){
                for (int i = 0; i < reader.FieldCount; i++){
                    columns.Add(reader.GetString(i));
                }
            }
            reader.Close();
            return columns;
        }

        public static List<string> GetTablesName(MySqlConnection connection){
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SHOW TABLES";
            MySqlDataReader reader = command.ExecuteReader();
            var tables = new List<string>();
            while (reader.Read()){
                for (int i = 0; i < reader.FieldCount; i++){
                    tables.Add(reader.GetString(i));
                }
            }
            reader.Close();
            return tables;
        }
    }
}
