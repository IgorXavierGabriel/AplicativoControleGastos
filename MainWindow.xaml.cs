using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MeuGerenciadorFinanceiroNovo.Data;
using MeuGerenciadorFinanceiroNovo.Models;
using MeuGerenciadorFinanceiroNovo.Utils;

namespace MeuGerenciadorFinanceiroNovo
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly FinanceDbManager _dbManager = new();

        private string _descricao = string.Empty;
        private string _tipoSelecionado = string.Empty;
        private double _valor;

        private int? _filtroMes = null;
        private int? _filtroAno = null;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Inicializar campos opcionais como vazios para evitar avisos de nulidade
            MesInput.Text = string.Empty;
            AnoInput.Text = string.Empty;

            AtualizarRelatorio();
        }

        // Propriedades bindáveis se necessário, notificando mudança (exemplo)
        public string Descricao
        {
            get => _descricao;
            set
            {
                if (_descricao != value)
                {
                    _descricao = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Descricao)));
                }
            }
        }

        // Evento para limpar os campos da interface
        private void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            ValorInput.Text = string.Empty;
            DataInput.Text = string.Empty;
            DescricaoInput.Text = string.Empty;
            Mensagem.Text = string.Empty;
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            AdicionarTransacao("Entrada");
        }

        private void AddExpense_Click(object sender, RoutedEventArgs e)
        {
            AdicionarTransacao("Gasto");
        }

        private void AdicionarTransacao(string tipo)
        {
            Mensagem.Text = string.Empty;

            // Validação do valor
            if (!double.TryParse(ValorInput.Text.Replace(',', '.'), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double valor) || valor <= 0)
            {
                Mensagem.Text = "Valor inválido. Insira um número positivo.";
                return;
            }

            // Validação da data
            if (!DateTime.TryParse(DataInput.Text, out DateTime data))
            {
                Mensagem.Text = "Data inválida. Use o formato AAAA-MM-DD.";
                return;
            }

            string descricao = DescricaoInput.Text ?? string.Empty;

            // Inserir no banco de dados
            _dbManager.InsertTransaction(tipo, valor, descricao, data);

            Mensagem.Text = $"Transação '{tipo}' adicionada com sucesso!";
            ClearFields_Click(null!, null!);
            AtualizarRelatorio();
        }

        private void AtualizarRelatorio()
        {
            int? mes = null;
            int? ano = null;

            if (int.TryParse(MesInput.Text, out int mesParse) && mesParse >= 1 && mesParse <= 12)
                mes = mesParse;

            if (int.TryParse(AnoInput.Text, out int anoParse) && anoParse >= 1)
                ano = anoParse;

            List<Transaction> transacoes = _dbManager.GetTransactions(mes, ano);

            if (transacoes.Count == 0)
            {
                RelatorioTexto.Text = "Nenhuma transação encontrada para o período informado.";
            }
            else
            {
                RelatorioTexto.Text = string.Join(Environment.NewLine, transacoes.Select(t =>
                    $"{t.Date:dd-MM-yyyy} | {t.Type} | R$ {t.Value:F2} | {t.Description}"));
            }
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            int? mes = null;
            int? ano = null;

            if (int.TryParse(MesInput.Text, out int mesParse) && mesParse >= 1 && mesParse <= 12)
                mes = mesParse;

            if (int.TryParse(AnoInput.Text, out int anoParse) && anoParse >= 1)
                ano = anoParse;

            var transacoes = _dbManager.GetTransactions(mes, ano);

            if (transacoes.Count == 0)
            {
                MessageBox.Show("Nenhuma transação para gerar relatório.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                string caminhoPdf = PdfExporter.ExportToPdf(transacoes);
                MessageBox.Show($"Relatório gerado com sucesso:\n{caminhoPdf}", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao gerar relatório: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValorInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Mensagem.Text = string.Empty;
        }

        private void DataInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Mensagem.Text = string.Empty;
        }
    }
}
