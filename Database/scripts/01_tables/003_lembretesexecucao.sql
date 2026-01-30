CREATE TABLE IF NOT EXISTS lembretesexecucao (
    ExecucaoID INT AUTO_INCREMENT PRIMARY KEY,
    LembreteID INT,
    NovoLembreteID INT,
    DataExecucao DATETIME DEFAULT CURRENT_TIMESTAMP,
    Status VARCHAR(20) DEFAULT 'CRIADO',
    FOREIGN KEY (LembreteID) REFERENCES lembretes(LembreteID),
    FOREIGN KEY (NovoLembreteID) REFERENCES lembretes(LembreteID)
);
