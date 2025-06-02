using System;

namespace MeuGerenciadorFinanceiroNovo.Models
{
    // Representa uma transação financeira
    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public double Value { get; set; }
        public string Description { get; set; } = string.Empty; // Inicializado para evitar CS8618
        public DateTime Date { get; set; }
    }
}