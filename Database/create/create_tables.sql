-- Criar a tabela usuario
CREATE TABLE IF NOT EXISTS usuario (
    UsuarioID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL,
    Password VARCHAR(100) NOT NULL,
    Name VARCHAR(100),
    Foto VARCHAR(200),
    Email VARCHAR(100),
    Cargo VARCHAR(50),
    Matricula VARCHAR(50)
);

-- Criar a tabela lembretes
CREATE TABLE IF NOT EXISTS lembretes (
    LembreteID INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioID INT NOT NULL,
    Titulo VARCHAR(100) NOT NULL,
    Descricao TEXT NOT NULL,
    DataLembrete DATETIME NOT NULL,
    CriadoEm DATETIME DEFAULT CURRENT_TIMESTAMP,
    IntervaloEmDias INT NOT NULL DEFAULT 0,
    FOREIGN KEY (UsuarioID) REFERENCES usuario(UsuarioID)
);

-- Criar a tabela lembretesexecucao
CREATE TABLE IF NOT EXISTS lembretesexecucao (
    ExecucaoID INT AUTO_INCREMENT PRIMARY KEY,
    LembreteID INT,
    NovoLembreteID INT,
    DataExecucao DATETIME,
    FOREIGN KEY (LembreteID) REFERENCES lembretes(LembreteID),
    FOREIGN KEY (NovoLembreteID) REFERENCES lembretes(LembreteID)
);

-- Criar a tabela lembreteslog
CREATE TABLE IF NOT EXISTS lembreteslog (
    LogID INT AUTO_INCREMENT PRIMARY KEY,
    LembreteID INT NOT NULL,
    Titulo VARCHAR(100),
    Descricao TEXT,
    DataLembrete DATETIME,
    CriadoEm DATETIME,
    IntervaloEmDias INT,
    Acao VARCHAR(10) NOT NULL,
    DataLog DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (LembreteID) REFERENCES lembretes(LembreteID)
);

---------------------------------------------------------------

-- Tabela: Categorias
CREATE TABLE Categorias (
    CategoriaID INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(100) NOT NULL,
    Descricao VARCHAR(255)
);

-- Tabela: Transacoes
CREATE TABLE Transacoes (
    TransacaoID INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioID INT,
    CategoriaID INT,  -- Categoria associada diretamente à transação
    Tipo ENUM('Entrada', 'Saída') NOT NULL,
    Valor DECIMAL(10, 2) NOT NULL,
    Descricao VARCHAR(255),
    Data DATETIME DEFAULT CURRENT_TIMESTAMP,
    CriadoEm DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID) ON DELETE CASCADE,
    FOREIGN KEY (CategoriaID) REFERENCES Categorias(CategoriaID) ON DELETE CASCADE
);

