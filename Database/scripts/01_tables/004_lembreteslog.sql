CREATE TABLE IF NOT EXISTS lembreteslog (
    LogID INT AUTO_INCREMENT PRIMARY KEY,
    LembreteID INT NOT NULL,
    UsuarioID INT,
    Titulo VARCHAR(100),
    Descricao TEXT,
    DataLembrete DATETIME,
    CriadoEm DATETIME,
    IntervaloEmDias INT,
    Acao VARCHAR(10) NOT NULL,
    DataAcao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (LembreteID) REFERENCES lembretes(LembreteID)
);
