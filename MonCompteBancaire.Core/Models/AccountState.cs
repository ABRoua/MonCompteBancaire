using System;
using System.Collections.Generic;

namespace MonCompteBancaire.Core.Models
{
    public class AccountState
    {
        public DateTime CurrentDate { get; set; }
        public decimal CurrentBalance { get; set; }
        public Dictionary<string, decimal> ExchangeRates { get; set; }
        public List<Transaction> Transactions { get; set; }

        public AccountState()
        {
            ExchangeRates = new Dictionary<string, decimal>();
            Transactions = new List<Transaction>();
        }
    }
}