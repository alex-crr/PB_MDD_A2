using MySqlX.XDevAPI.Relational;
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

            Home(connection);
        }
    }
}