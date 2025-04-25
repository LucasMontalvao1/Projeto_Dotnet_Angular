
# 💰 FinanceReminderHub

Projeto full stack utilizando **.NET 8 (ASP.NET Core)** no backend e **Angular 18** no frontend, com foco no gerenciamento de lembretes financeiros, transações e visualização analítica de dados.

## 🚀 Tecnologias

### Backend
- .NET 8
- Entity Framework Core
- JWT Authentication
- RabbitMQ
- Hangfire
- xUnit + Coverlet (testes)
- Swagger

### Frontend
- Angular 18
- RxJS
- Angular Material
- NgRx (opcional)
- Chart.js / ngx-charts

---

## 📦 Funcionalidades

- [x] Cadastro e autenticação de usuários (JWT)
- [x] Cadastro de lembretes financeiros
- [x] Integração com RabbitMQ para envio de notificações assíncronas
- [x] Visualização de dados em dashboards
- [x] Agendamento de tarefas com Hangfire
- [x] API REST com Swagger
- [x] Testes automatizados com cobertura

---

## 🔧 Como rodar o projeto

### Backend (.NET)
```bash
# Clone o repositório
git clone https://github.com/LucasMontalvao1/Projeto_Dotnet_Angular.git
cd Projeto_Dotnet_Angular/backend

# Restaure os pacotes
dotnet restore

# Aplique as migrations (caso esteja usando migrations)
dotnet ef database update

# Rode o projeto
dotnet run
```

### Frontend (Angular)
```bash
# Vá até a pasta frontend
cd Projeto_Dotnet_Angular/frontend

# Instale as dependências
npm install

# Rode a aplicação Angular
ng serve
```

---

## 🧪 Testes

```bash
# Execute os testes unitários do backend
dotnet test /p:CollectCoverage=true

# Geração de relatório de cobertura (via coverlet)
```

---

## 📊 Dashboard

O sistema conta com uma página de dashboard onde é possível visualizar:

- Gastos por categoria
- Histórico de lembretes
- Tarefas agendadas
- Status em tempo real (via RabbitMQ)

---

## ⚙️ Integrações

| Ferramenta  | Finalidade                     |
|-------------|--------------------------------|
| RabbitMQ    | Notificações em tempo real     |
| Hangfire    | Agendamento de tarefas         |
| Swagger     | Documentação da API            |
| PostgreSQL / SQL Server | Banco de dados relacional |

---

## 🛡 Segurança

- Autenticação JWT
- Validação de entrada (Data Annotations + FluentValidation)
- Autorização baseada em claims
- Criptografia de senhas com Identity

---

## ✨ Melhorias Futuras

- [ ] Implementar CQRS com MediatR
- [ ] Adicionar internacionalização (i18n) no frontend
- [ ] Adicionar testes de integração
- [ ] Deploy via GitHub Actions

---

## 🧠 Autor

Desenvolvido por [Lucas Montalvão](https://github.com/LucasMontalvao1) 👨‍💻  
Projeto para fins de estudo e aprimoramento de tecnologias modernas full stack.

---

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).
