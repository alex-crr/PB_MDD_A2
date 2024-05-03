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
            string[] options = ["Load from table", "See some stuff", "Modify a table", "Demo", "Exit"];

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
                                SeeStuff(connection);
                                Home(connection);
                                break;

                            case 2:// Modify a table
                                if (GetId(connection) == "bozo")
                                {
                                    Dialog text = new Dialog(
                                        new List<string>()
                                        {
                                            "You don't have the rights to modify tables.",
                                        },
                                        null,
                                        "Go back"
                                        );
                                    Window.AddElement(text);
                                    Window.ActivateElement(text);
                                    Home(connection);
                                }
                                ModifyTable(connection);
                                Home(connection);
                                break;
                            case 3: // Demo
                                Demo(connection);
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

        public static void Demo(MySqlConnection connection)
        {
            Window.Open();
            Title title = new Title("VeloMax");
            Window.AddElement(title);
            Window.Render(title);
            SeeTable(connection, tableName: "client");
            Window.DeactivateAllElements();
            Window.Clear();

            string query = "INSERT INTO client  (idClient, nomClient, prenomClient, adresseClient, creaFidelite, idProgramme) VALUES (5, 'Federer', 'Roger', 'Paris', '2023-01-01', 4);";
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            SeeTable(connection, tableName: "client");
            Window.DeactivateAllElements();
            Window.Clear();

            query = "UPDATE client SET nomClient = 'Nadal' WHERE idClient = 5;";
            command = connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            SeeTable(connection, tableName: "client");
            Window.DeactivateAllElements();
            Window.Clear();

            query = "DELETE FROM client WHERE idClient = 5;";
            command = connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();

            SeeTable(connection, tableName: "client");
            Window.DeactivateAllElements();
            Window.Clear();

            query = "SELECT COUNT(*) FROM client;";
            var headers = new List<string> { "Clients count" };
            var data = SelectFromCommand(connection, query);

            TableView seeTable = new TableView("Total clients", headers, data);
            Window.AddElement(seeTable);
            Window.Render(seeTable);
            Window.Freeze();
            Window.DeactivateAllElements();
            Window.Clear();

            query = "SELECT prenomClient, nomClient, SUM(c.prixCommande) from client natural join commande c group by prenomClient, nomClient;";
            headers = new List<string> { "Prenom", "Nom", "Total" };
            data = SelectFromCommand(connection, query);
            seeTable = new TableView("Spent money", headers, data);
            Window.AddElement(seeTable);
            Window.Render(seeTable);
            Window.Freeze();
            Window.DeactivateAllElements();
            Window.Clear();

            query = query = "SELECT p.idPiece, p.refFournisseur, p.quantitePiece FROM pieces p WHERE p.quantitePiece <= 2;";
            headers = new List<string> { "Part Id", "Part ref", "Total" };
            data = SelectFromCommand(connection, query);
            seeTable = new TableView("Low quantities parts", headers, data);
            Window.AddElement(seeTable);
            Window.Render(seeTable);
            Window.Freeze();
            Window.DeactivateAllElements();
            Window.Clear();

            query = "select `nomEntreprise`, Count(f.`idPiece`) as total_count from fournisseur natural join fourni f group by `nomEntreprise` order by total_count Desc;";
            headers = new List<string> { "Suppliers", "Total supplied goods"};
            data = SelectFromCommand(connection, query);
            seeTable = new TableView("Supplied good count", headers, data);
            Window.AddElement(seeTable);
            Window.Render(seeTable);
            Window.Freeze();
            Window.DeactivateAllElements();
            Window.Clear();

            query = "SELECT prenomClient, nomClient, 'Client' AS type FROM client UNION SELECT prenomVendeur, nomVendeur, 'Salesperson' AS type FROM vendeur;";
            headers = new List<string> { "First name", "Surname", "Relation to VeloMax" };
            data = SelectFromCommand(connection, query);
            seeTable = new TableView("Contact list", headers, data);
            Window.AddElement(seeTable);
            Window.Render(seeTable);
            Window.Freeze();
            Window.DeactivateAllElements();
            Window.Clear();

            query = "SELECT m.idMagasin, SUM(c.prixCommande) AS chiffre_affaires FROM magasin m JOIN travaille t ON m.idMagasin = t.idMagasin JOIN vendeur v ON t.idVendeur = v.idVendeur JOIN commande c ON v.idVendeur = c.idVendeur GROUP BY m.idMagasin; ";
            headers = new List<string> { "Shop Id", "Turnover"};
            data = SelectFromCommand(connection, query);
            seeTable = new TableView("Shops turnover", headers, data);
            Window.AddElement(seeTable);
            Window.Render(seeTable);
            Window.Freeze();
            Window.DeactivateAllElements();
            Window.Clear();

            query = "SELECT v.idVendeur, v.prenomVendeur, v.nomVendeur, SUM(c.prixCommande) AS ventes_generées FROM vendeur v  NATURAL JOIN commande c GROUP BY v.idVendeur;";
            headers = new List<string> { "Saleperson Id", "First name", "Surname", "Turnover"};
            data = SelectFromCommand(connection, query);
            seeTable = new TableView("Sales by employees", headers, data);
            Window.AddElement(seeTable);
            Window.Render(seeTable);
            Window.Freeze();
            Window.DeactivateAllElements();
            Window.Clear();
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

        public static void SeeTable(MySqlConnection connection, (string[], ConsoleAppVisuals.Models.InteractionEventArgs<int>) responseTable = default, string tableName = "")
        {
            if (responseTable == default && tableName == "")
            {
                throw new ArgumentException("Either responseTable or tableName must be provided.");
            }
            List<List<string>>? data = new List<List<string>>();
            List<string>? headers = new List<string>();
            if (responseTable != default)
            {
                tableName = responseTable.Item1[responseTable!.Item2.Value];
                data = Select(connection, tableName);
                headers = GetColumnsName(connection, tableName);
            }
            if (tableName != "")
            {
                data = Select(connection, tableName);
                headers = GetColumnsName(connection, tableName);
            }

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
                if (responseInsert!.Status == Status.Escaped) { Window.DeactivateElement(promptInsert); Home(connection); }
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

        public static void SeeStuff(MySqlConnection connection)
        {
            string[] options = new string[] { "Clients count", "Clients spent money", "Spare parts in low quantities",
            "Spare parts count", "Shops turnover", "Sales by employee", "Sold item quantities", "Sold item quantities by shops",
            "Sold Item quantities by salesperson", "Sold fidelity programms", "Fidelity programms expiration date",
            "Average order price", "Average parts per command", "Average bike number per command", "Go back"};

            ScrollingMenu menu = new ScrollingMenu(
                "Please choose an option among those below.",
                0,
                Placement.TopCenter,
                options
            );

            Window.AddElement(menu);
            Window.ActivateElement(menu);

            var response = menu.GetResponse();
            string query = "";
            List<string> headers = new List<string>();
            switch (response!.Status)
            {
                case Status.Selected:
                    switch (response.Value)
                    {
                        case 0: // Clients count
                            query = "SELECT COUNT(*) FROM client;";
                            headers = new List<string> { "Clients count" };
                            break;
                        case 1: // Clients spent money
                            query = "SELECT prenomClient, nomClient, SUM(c.prixCommande) from client natural join commande c group by prenomClient, nomClient;";
                            headers = new List<string> { "Client", "Total" };
                            break;

                        case 2: // Spare parts in low quantities
                            query = "SELECT p.idPiece, p.quantitePiece FROM pieces p WHERE p.quantitePiece <= 2;";
                            headers = new List<string> { "Piece", "Quantity" };
                            break;
                        case 3: // Spare parts count
                            query = "SELECT f.nomEntreprise, COUNT(*) AS nombre_de_pieces_velos_fournis FROM fourni fuJOIN fournisseur f ON fu.SIRET = f.SIRETGROUP BY f.nomEntreprise;";
                            headers = new List<string> { "Supplier", "Parts quantities" };
                            break;
                        case 4: // Shops turnover
                            query = "SELECT m.idMagasin, SUM(c.prixCommande) AS chiffre_affaires FROM magasin m JOIN travaille t ON m.idMagasin = t.idMagasin JOIN vendeur v ON t.idVendeur = v.idVendeur JOIN commande c ON v.idVendeur = c.idVendeur GROUP BY m.idMagasin;";
                            headers = new List<string> { "Shop", "Turnover" };
                            break;
                        case 5: // Sales by employee // Y a une erreur dans la requête
                            query = "SELECT v.idVendeur, v.prenomVendeur, v.nomVendeur, SUM(c.prixCommande) AS ventes_generées FROM vendeur v  NATURAL JOIN commande c GROUP BY v.idVendeur;";
                            headers = new List<string> { "Employee ID", "First name", "Surname", "Turnover" };
                            break;
                        case 6: // Sold item quantities
                            query = "SELECT idProduit, SUM(quantiteV) AS quantite_vendue_totaleFROM passe_veloGROUP BY idProduit;";
                            headers = new List<string> { "Item ID", "Quantity sold" };
                            break;
                        case 7: // Sold item quantities by shops
                            query = "SELECT v.idMagasin, idProduit, SUM(quantiteV) AS quantite_vendue_magasin FROM passe_velo pv JOIN vendeur v ON pv.idVelo = v.idVendeurGROUP BY v.idMagasin, idProduit;";
                            headers = new List<string> { "Shop ID", "Item ID", "Quantity sold" };
                            break;
                        case 8: // Sold Item quantities by salesperson
                            query = "SELECT idVendeur, idProduit, SUM(quantiteV) AS quantite_vendue_vendeur FROM passe_velo GROUP BY idVendeur, idProduit;";
                            headers = new List<string> { "Employee ID", "Item ID", "Quantity sold" };
                            break;
                        case 9: // Sold fidelity programms
                            query = "SELECT idProgramme, p.libelle, COUNT(*) AS nombre_de_membres FROM Client Natural join programme p GROUP BY idProgramme;";
                            headers = new List<string> { "Program ID", "Label", "Members count" };
                            break;
                        case 10: // Fidelity programms expiration date
                            query = "SELECT idClient, DATE_ADD(creaFidelite, INTERVAL duree YEAR) AS date_expiration_adhesionFROM Client cJOIN programme p ON c.idProgramme = p.idProgramme;";
                            headers = new List<string> { "Client Id", "Expiration date" };
                            break;
                        case 11: // Average order price
                            query = "SELECT AVG(prixCommande) AS prix_moyen_commande FROM commande;";
                            headers = new List<string> { "Average order price" };
                            break;
                        case 12: // Average parts per command
                            query = "SELECT AVG(quantiteP) AS moyenne_pieces_par_commandeFROM passe_piece;";
                            headers = new List<string> { "Average parts per command" };
                            break;
                        case 13: // Average bike number per command
                            query = "SELECT AVG(quantiteV) AS moyenne_velos_par_commandeFROM passe_velo;";
                            headers = new List<string> { "Average bikes per command" };
                            break;
                        default:
                            Home(connection);
                            break;


                    }

                    var data = SelectFromCommand(connection, query);

                    TableView seeTable = new TableView(options[response.Value], headers, data);
                    Window.AddElement(seeTable);

                    Window.Render(seeTable);

                    Window.Freeze();
                    break;
                case Status.Escaped:
                case Status.Deleted:
                    // Quit the app anyway
                    Window.Close();
                    break;
            }
        }
    }
}
