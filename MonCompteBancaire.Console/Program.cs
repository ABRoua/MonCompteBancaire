using System;
using System.Globalization;
using System.IO;
using System.Threading;
using MonCompteBancaire.Core.Services;

namespace MonCompteBancaire.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

                string csvPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Account.csv");

                if (!File.Exists(csvPath))
                {
                    System.Console.WriteLine("Le fichier Account.csv n'exist pas!");
                    return;
                }

                var csvParser = new CsvParser();
                var accountState = csvParser.ParseAccountFile(csvPath);
                var accountService = new AccountService(accountState);

                System.Console.WriteLine("Consultation de compte bancaire");

                while (true)
                {
                    System.Console.WriteLine("\nOptions:");
                    System.Console.WriteLine("1. Consulter le solde dans une date donnée");
                    System.Console.WriteLine("2. Voir les principals catégorie de débit");
                    System.Console.WriteLine("3. quitter");
                    System.Console.Write("\nVotre choix de 1-3: ");

                    var choice = System.Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            System.Console.Write("\nEntrer une date comme la forme (dd/MM/yyyy): ");
                            if (DateTime.TryParseExact(System.Console.ReadLine(), "dd/MM/yyyy",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                            {
                                try
                                {
                                    var balance = accountService.GetBalanceAtDate(date);
                                    System.Console.WriteLine($"\nSolde au {date:dd/MM/yyyy}: {balance:N2} EUR");
                                }
                                catch (ArgumentException ex)
                                {
                                    System.Console.WriteLine($"\nErreur: {ex.Message}");
                                }
                            }
                            else
                            {
                                System.Console.WriteLine("\nFormat invalide. Utiliser la format dd/MM/yyyy");
                            }
                            break;

                        case "2":
                            System.Console.WriteLine("\nPrincipales catégories de débit:");
                            var categories = accountService.GetTopDebitCategories().ToList();

                            foreach (var category in categories)
                            {
                                System.Console.WriteLine($"{category.Category,-15}: {category.TotalDebit:N2} EUR");
                            }

                            break;

                        case "3":
                            return;

                        default:
                            System.Console.WriteLine("\nOption invalide. Choisir entre 1 et 3.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Erreur: {ex.Message}");
            }
        }
    }
}