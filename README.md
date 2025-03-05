# FinanceReminderHub - Sistema de lembretes e financeiro 🚀📅💸

## Descrição do Projeto

FinanceReminderHub é um sistema de gerenciamento de lembretes e transações financeiras, projetado para simplificar o acompanhamento de tarefas e finanças pessoais, com arquitetura moderna, integração em tempo real e recursos de automação.

## 🌟 Recursos Principais

### Funcionalidades Avançadas
- Gerenciamento completo de lembretes
- Controle de transações financeiras
- Notificações em tempo real
- Automação de tarefas
- Logging detalhado
- Comunicação assíncrona com RabbitMQ
- Dashboards e gráficos analíticos
- Autenticação com Bearer Token
- Cobertura de teste com o Coverage report
- Acesso restrito de lembretes e transações por usuário logado

## 🛠 Tecnologias Utilizadas

| Categoria | Tecnologia | Versão | Propósito |
|----------|------------|--------|-----------|
| Backend | ASP.NET Core | 8.0.401 | API e lógica de negócios |
| Frontend | Angular | 18 | Interface de usuário |
| Banco de Dados | MySQL | 8.0 | Persistência de dados |
| Mensageria | RabbitMQ | - | Comunicação assíncrona |
| Automação | Hangfire | - | Tarefas em background |
| Logging | Serilog | - | Registro de eventos |
| Testes | xUnit | - | Testes unitários |
| Mocking | Moq | - | Criação de mocks |
| Cobertura | Coverlet | - | Análise de cobertura de código |
| Node.js | 18.19.1 | - | Ambiente de desenvolvimento |
| NPM | 10.8.2 | - | Gerenciamento de pacotes |

## 📂 Estrutura do Projeto

```plaintext
.
├── ApiWeb/                 # Backend da API
│   ├── Controllers/     # Controladores da API
│   ├── Services/        # Lógica de negócio
│   ├── Repositories/    # Acesso a dados
│   ├── Logs/    # Logs da API
├── ApiWeb.Tests/           # Testes da API
│   ├── Controllers/       # Testes unitários dos Controlers
│   └── Mocks /# Mocks para os testes
│   └── Models /# Testes dos Models
│   └── Repositories /# Testes dos Repositories
│   └── Services /# Testes dos Services
├── FrontAngular /             # Frontend Angular
│   ├── src/             # Código fonte
│   ├── components/      # Componentes reutilizáveis
│   └── services/        # Serviços de comunicação
│   └── environments/        # Serviços de comunicação com a API
├── database/            # Scripts de banco de dados
│   ├── create_tables.sql
│   ├── insert_data.sql
│   ├── Jobs.sql
│   └── triggers.sql
└── README.md
```

## 🔬 Recursos Avançados

### Comunicação em Tempo Real
- **RabbitMQ**: Mensageria para comunicação instantânea
- Fluxo de comunicação:
  1. Lembrete disparado via Swagger
  2. Mensagem enviada ao RabbitMQ
  3. Frontend atualizado automaticamente
  4. Notificação push para usuário

### Autenticação e Segurança
- Autenticação via Bearer Token
- Acesso restrito baseado em usuário
- Endpoints protegidos
- Lembretes e transações isolados por usuário

### Gráficos e Dashboards
- Visualizações de dados em tempo real
- Tipos de gráficos:
  - Receitas vs Despesas
  - Evolução do Saldo
  - Distribuição de Despesa

### Testes e Qualidade

#### Estratégia de Testes
- **Framework**: xUnit
- **Mocking**:  Moq
- **Cobertura de Código**: Coverage Report

##### Comandos para Execução de Testes

1. Executar todos os testes:
```bash
dotnet test
```

2. Gerar relatório de cobertura:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./coverage/
```

3. Visualizar relatório de cobertura:
```bash
reportgenerator -reports:"./coverage/coverage.lcov" -targetdir:"coveragereport" -reporttypes:Html
```

## 🔧 Configuração do Ambiente

### Pré-requisitos

- .NET SDK 8.0
- Node.js 18.x
- MySQL 8.0
- Angular CLI
- RabbitMQ

### Configuração do Banco de Dados

1. Crie o banco de dados:
```bash
CREATE DATABASE FinanceReminderHub;
```

2. Execute os scripts SQL:
```bash
mysql -u root -p FinanceReminderHub < ./database/create_tables.sql
mysql -u root -p FinanceReminderHub < ./database/insert_data.sql
mysql -u root -p FinanceReminderHub < ./database/triggers.sql
```

### Configuração do Backend

1. Navegue até o diretório da API:
```bash
cd api/
```

2. Restaure dependências:
```bash
dotnet restore
```

3. Configure a conexão no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;userid=root;password=sua_senha;database=reminderhub"
  },
  "RabbitMq": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  }
}
```

4. Execute a API:
```bash
dotnet run
```

### Configuração do Frontend

1. Navegue até o diretório do Angular:
```bash
cd angular/
```

2. Instale dependências:
```bash
npm install
```

3. Configure o ambiente em `environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7263',
  rabbitMqConfig: {
    host: 'localhost',
    port: 5672
  },
  endpoints: {
    login: '/api/login',
    reminders: '/api/reminders',
    transactions: '/api/transactions'
  }
};
```

4. Inicie o servidor de desenvolvimento:
```bash
ng serve
```

## 🌐 Acesso à Aplicação

- **Frontend:** http://localhost:4200/
- **Backend (Swagger):** https://localhost:7263/swagger/index.html
- **RabbitMQ Management:** http://localhost:15672/
- **Hangfire Dashboard:** http://localhost:7263/hangfire

## 🔐 Credenciais Padrão

- **Usuário:** `lucas`
- **Senha:** `123`

## 📊 Monitoramento e Logs

### Verificação de Logs
```bash
cd ApiWeb/logs
cat myapp-20250304.log
```

### Verificação de Triggers
```sql
SHOW TRIGGERS;
```

### Monitoramento de Filas (RabbitMQ)
- Acompanhamento de mensagens
- Estatísticas de processamento
- Verificação de filas pendentes

## 📞 Contato

- **Desenvolvedor:** Lucas Montalvão
- **GitHub:** [LucasMontalvao1](https://github.com/LucasMontalvao1)

