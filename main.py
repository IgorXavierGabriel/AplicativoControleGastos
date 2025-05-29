from kivy.app import App
from kivy.uix.boxlayout import BoxLayout
from kivy.properties import StringProperty
from database import init_db, inserir_transacao, obter_transacoes

class FinanceManager(BoxLayout):
    relatorio_texto = StringProperty("")

    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        init_db()  # Garante que o banco está pronto

    def adicionar_entrada(self, valor, descricao):
        inserir_transacao('entrada', float(valor), descricao)

    def adicionar_gasto(self, valor, descricao):
        inserir_transacao('gasto', float(valor), descricao)

    def gerar_relatorio(self, mes=None, ano=None):
        # mes e ano podem ser None ou string/número
        transacoes = obter_transacoes(mes, ano)
        total_entradas = sum(t[2] for t in transacoes if t[1] == 'entrada')
        total_gastos = sum(t[2] for t in transacoes if t[1] == 'gasto')
        saldo = total_entradas - total_gastos
        if mes and ano:
            titulo = f"Relatório de {mes:0>2}/{ano}"
        else:
            titulo = "Relatório Geral"
        self.relatorio_texto = f"""{titulo}
Entradas: R$ {total_entradas:.2f}
Gastos: R$ {total_gastos:.2f}
Saldo: R$ {saldo:.2f}
""".strip()

class GastosApp(App):
    def build(self):
        return FinanceManager()

if __name__ == "__main__":
    GastosApp().run()