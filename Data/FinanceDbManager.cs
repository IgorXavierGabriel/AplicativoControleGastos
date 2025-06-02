using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using MeuGerenciadorFinanceiroNovo.Models;

namespace MeuGerenciadorFinanceiroNovo.Data
{
    public class FinanceDbManager
    {
        private const string DbName = "finance.db";
        private string ConnectionString => $"Data Source={DbName}";

        public FinanceDbManager()
        {
            InitDb();
        }

        public void InitDb()
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS transacoes (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        tipo TEXT NOT NULL,
                        valor REAL NOT NULL,
                        descricao TEXT,
                        data TEXT NOT NULL
                    )";
                command.ExecuteNonQuery();
            }
        }

        public void InsertTransaction(string type, double value, string description, DateTime? date = null)
        {
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO transacoes (tipo, valor, descricao, data)
                    VALUES (@tipo, @valor, @descricao, @data)";

                command.Parameters.AddWithValue("@tipo", type);
                command.Parameters.AddWithValue("@valor", value);
                command.Parameters.AddWithValue("@descricao", description);
                command.Parameters.AddWithValue("@data", (date ?? DateTime.Now).ToString("yyyy-MM-dd"));

                command.ExecuteNonQuery();
            }
        }

        public List<Transaction> GetTransactions(int? month = null, int? year = null)
        {
            var transactions = new List<Transaction>();
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT id, tipo, valor, descricao, data FROM transacoes";

                var conditions = new List<string>();
                if (month.HasValue)
                {
                    conditions.Add("strftime('%m', data) = @month");
                    command.Parameters.AddWithValue("@month", month.Value.ToString("00"));
                }
                if (year.HasValue)
                {
                    conditions.Add("strftime('%Y', data) = @year");
                    command.Parameters.AddWithValue("@year", year.Value.ToString());
                }

                if (conditions.Count > 0)
                {
                    command.CommandText += " WHERE " + string.Join(" AND ", conditions);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        transactions.Add(new Transaction
                        {
                            Id = reader.GetInt32(0),
                            Type = reader.GetString(1),
                            Value = reader.GetDouble(2),
                            Description = reader.GetString(3),
                            Date = DateTime.Parse(reader.GetString(4))
                        });
                    }
                }
            }
            return transactions;
        }
    }
}