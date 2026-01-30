CREATE TABLE IF NOT EXISTS lembretes (
    LembreteID INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioID INT NOT NULL,
    Titulo VARCHAR(100) NOT NULL,
    Descricao TEXT NOT NULL,
    DataLembrete DATETIME NOT NULL,
    CriadoEm DATETIME DEFAULT CURRENT_TIMESTAMP,
    IntervaloEmDias INT NOT NULL DEFAULT 0,
    Ativo TINYINT(1) DEFAULT 1,
    LembretePaiID INT DEFAULT NULL,
    UltimoProcessamento DATETIME DEFAULT NULL,
    FOREIGN KEY (UsuarioID) REFERENCES usuario(UsuarioID),
    FOREIGN KEY (LembretePaiID) REFERENCES lembretes(LembreteID)
);
