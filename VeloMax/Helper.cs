namespace PB_MDD_A2
{
    public class Helper
    {
         public static List<string> GetColumns(MySqlConnection connection, string tableName){
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}'";
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
    }
}
