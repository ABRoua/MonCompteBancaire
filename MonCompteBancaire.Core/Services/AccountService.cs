using System;
using System.Collections.Generic;
using System.Linq;
using MonCompteBancaire.Core.Models;

namespace MonCompteBancaire.Core.Services
{
    public class AccountService
    {
        private readonly AccountState _accountState;

        public AccountService(AccountState accountState)
        {
            _accountState = accountState;
        }

        public decimal GetBalanceAtDate(DateTime date)
        {
            if (date < new DateTime(2022, 1, 1) || date > new DateTime(2023, 3, 1))
                throw new ArgumentException("la date doit etre entre 01/01/2022 et 01/03/2023");

            var balance = _accountState.CurrentBalance;
            var transactionsToDate = _accountState.Transactions.Where(t => t.Date <= date);

            foreach (var transaction in transactionsToDate)
            {
                balance += GetAmountInEuros(transaction);
            }

            return Math.Round(balance, 2);
        }

        public IEnumerable<(string Category, decimal TotalDebit)> GetTopDebitCategories()
        {
            return _accountState.Transactions
                .Where(t => GetAmountInEuros(t) < 0)
                .GroupBy(t => t.Category)
                .Select(g => (
                    Category: g.Key,
                    TotalDebit: Math.Abs(g.Sum(t => GetAmountInEuros(t)))
                ))
                .OrderByDescending(x => x.TotalDebit)
                .ToList();
        }

        private decimal GetAmountInEuros(Transaction transaction)
        {
            if (!_accountState.ExchangeRates.TryGetValue(transaction.Currency, out decimal rate))
            {
                throw new Exception($"Exchange n'xiste pas {transaction.Currency}");
            }

            return transaction.Amount * rate;
        }
    }
}