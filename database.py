import sqlite3
from datetime import datetime

DB_NAME = "finance.db"

def init_db():
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    c.execute("""
        CREATE TABLE IF NOT EXISTS transacoes (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            tipo TEXT NOT NULL,
            valor REAL NOT NULL,
            descricao TEXT,
            data TEXT NOT NULL
        )
    """)
    conn.commit()
    conn.close()

def inserir_transacao(tipo, valor, descricao):
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    data = datetime.now().strftime("%Y-%m-%d")
    c.execute("INSERT INTO transacoes (tipo, valor, descricao, data) VALUES (?, ?, ?, ?)",
              (tipo, valor, descricao, data))
    conn.commit()
    conn.close()

def obter_transacoes(mes=None, ano=None):
    conn = sqlite3.connect(DB_NAME)
    c = conn.cursor()
    query = "SELECT id, tipo, valor, descricao, data FROM transacoes"
    params = []
    if mes and ano:
        query += " WHERE strftime('%m', data) = ? AND strftime('%Y', data) = ?"
        params = [f"{int(mes):02d}", str(ano)]
    elif mes:
        query += " WHERE strftime('%m', data) = ?"
        params = [f"{int(mes):02d}"]
    elif ano:
        query += " WHERE strftime('%Y', data) = ?"
        params = [str(ano)]
    c.execute(query, params)
    transacoes = c.fetchall()
    conn.close()
    return transacoes