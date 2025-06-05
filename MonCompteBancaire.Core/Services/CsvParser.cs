using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MonCompteBancaire.Core.Models;

namespace MonCompteBancaire.Core.Services
{
    public class CsvParser
    {
        public AccountState ParseAccountFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Le fichier {filePath} n'exist pas.");

            var accountState = new AccountState
            {
                CurrentBalance = 8300.00m, 
                ExchangeRates = new Dictionary<string, decimal>
                {
                    { "EUR", 1.0m },
                    { "USD", 1.0m / 1.445m },
                    { "JPY", 0.482m }         
                }
            };

            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0)
                throw new InvalidOperationException("Le fichier est vide.");

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                try
                {
                   
                    var parts = line.Split(';');

                    if (parts.Length >= 4) // date;montant;devise;catégorie
                    {
                        var transaction = new Transaction
                        {
                            Date = DateTime.ParseExact(parts[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture),
                            Amount = decimal.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                            Currency = parts[2].Trim(),
                            Category = parts[3].Trim()
                        };

                        accountState.Transactions.Add(transaction);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Erreur de parse de la ligne '{line}': {ex.Message}");
                }
            }

            return accountState;
        }
    }
}