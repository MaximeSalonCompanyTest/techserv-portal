# TechServ customer portal

[English](#english) | [Português](#português)

---

## English

Customers sign up, open service requests and follow their status. Agents see every request and update its status. Built for a junior developer technical test.

Stack: PostgreSQL 17, ASP.NET Core (.NET 10) REST API with EF Core and JWT, React with Vite (JavaScript), xUnit, Swagger, Docker Compose.

### Live demo

- Website: https://techserv-web.onrender.com
- API docs (Swagger): https://techserv-portal.onrender.com/swagger

The free API sleeps when idle, so the first request can take about a minute. Log in with the test users below.

### Run with Docker (recommended)

Requires Docker.

```bash
git clone https://github.com/MaximeSalonCompanyTest/techserv-portal.git
cd techserv-portal
docker compose up -d --build
```

On first start, the API creates the tables and the test data.

- Portal: http://localhost:5173
- API: http://localhost:5208
- Swagger: http://localhost:5208/swagger

Stop with `docker compose down`. Add `-v` to also delete the database.

### Run in development mode

Requires Docker (database only), .NET 10 SDK and Node.js 22+. Uses the same ports as the Docker setup, so run one or the other.

```bash
docker compose up -d db
cd backend/TechServ.Api && dotnet run       # terminal 1
cd frontend && npm install && npm run dev   # terminal 2
```

### Test users

| Role | Email | Password |
| --- | --- | --- |
| Customer | customer@techserv.com | Customer123! |
| Agent | agent@techserv.com | Agent123! |

### Tests and API docs

- Tests: `cd backend && dotnet test`. They check the status rules.
- Swagger: log in with `POST /api/auth/login`, copy the token, click Authorize and paste it.

### What's included

- Required: sign-up with a unique email, login and logout, profile (email read-only), create, list (status filter, newest first), view and cancel requests. Customers only see their own requests.
- Bonus: Agent role, status history, pagination, automated tests, Docker Compose, Swagger, free deployment.

### Decisions and assumptions

- Sign-up always creates a customer. Agent accounts only come from the seed data.
- The profile edits name and phone. Password change wasn't requested, so it's not included.
- Status flow: Open → In progress → Completed, or Open → Cancelled. Customers can only cancel, and only while the request is Open.
- Opening another customer's request returns 404, as if it didn't exist.
- Logout deletes the token in the browser. Tokens expire after 2 hours.
- Code and API messages are in English. Screens are in Portuguese.

### With more time

- Refresh tokens with a short expiry, so a copied token stops working after logout.
- Integration tests for the API routes.
- Email notifications on status changes.
- A screen to manage agent accounts, and password change.
- Update the name in the menu right after a profile edit (today it updates on the next page change).
- Remove the development JWT key and database password from `appsettings.json` (the deployed version already reads them from environment variables).

### AI use

I built this project with Claude (Anthropic) as an AI coding agent, under my supervision. I directed the work and reviewed every change.

### Deployment

- Database: Neon (free Postgres).
- API: Render web service, built from `backend/TechServ.Api/Dockerfile`. The connection string, JWT key and allowed frontend address come from environment variables.
- Frontend: Render static site (`npm run build`, with `VITE_API_URL` pointing to the API, and a rewrite of `/*` to `/index.html`).

---

## Português

Clientes criam uma conta, abrem solicitações de serviço e acompanham o status. Atendentes veem todas as solicitações e atualizam o status. Feito para um teste técnico de desenvolvedor júnior.

Tecnologias: PostgreSQL 17, API REST em ASP.NET Core (.NET 10) com EF Core e JWT, React com Vite (JavaScript), xUnit, Swagger, Docker Compose.

### Demo online

- Site: https://techserv-web.onrender.com
- Documentação da API (Swagger): https://techserv-portal.onrender.com/swagger

A API gratuita hiberna quando fica parada, então a primeira requisição pode levar cerca de um minuto. Entre com os usuários de teste abaixo.

### Rodar com Docker (recomendado)

Requer Docker.

```bash
git clone https://github.com/MaximeSalonCompanyTest/techserv-portal.git
cd techserv-portal
docker compose up -d --build
```

Na primeira inicialização, a API cria as tabelas e os dados de teste.

- Portal: http://localhost:5173
- API: http://localhost:5208
- Swagger: http://localhost:5208/swagger

Para parar: `docker compose down`. Com `-v`, o banco também é apagado.

### Rodar em modo desenvolvimento

Requer Docker (só para o banco), .NET 10 SDK e Node.js 22+. Usa as mesmas portas do Docker, então rode um ou outro.

```bash
docker compose up -d db
cd backend/TechServ.Api && dotnet run       # terminal 1
cd frontend && npm install && npm run dev   # terminal 2
```

### Usuários de teste

| Perfil | E-mail | Senha |
| --- | --- | --- |
| Cliente | customer@techserv.com | Customer123! |
| Atendente | agent@techserv.com | Agent123! |

### Testes e documentação da API

- Testes: `cd backend && dotnet test`. Eles verificam as regras de status.
- Swagger: faça login em `POST /api/auth/login`, copie o token, clique em Authorize e cole.

### O que foi entregue

- Obrigatório: cadastro com e-mail único, login e logout, perfil (e-mail não editável), criar, listar (filtro por status, mais recentes primeiro), ver e cancelar solicitações. O cliente só vê as próprias solicitações.
- Diferenciais: perfil de Atendente, histórico de status, paginação, testes automatizados, Docker Compose, Swagger, deploy gratuito.

### Decisões e suposições

- O cadastro sempre cria um cliente. Contas de atendente vêm apenas dos dados iniciais.
- O perfil edita nome e telefone. A troca de senha não foi pedida, então ficou de fora.
- Fluxo de status: Aberta → Em andamento → Concluída, ou Aberta → Cancelada. O cliente só pode cancelar, e só enquanto a solicitação está Aberta.
- Abrir a solicitação de outro cliente retorna 404, como se ela não existisse.
- O logout apaga o token no navegador. O token expira em 2 horas.
- Código e mensagens da API em inglês. Telas em português.

### Com mais tempo

- Refresh tokens com expiração curta, para que um token copiado pare de funcionar depois do logout.
- Testes de integração das rotas da API.
- Notificação por e-mail quando o status muda.
- Uma tela para gerenciar atendentes, e troca de senha.
- Atualizar o nome no menu logo após editar o perfil (hoje atualiza na próxima troca de página).
- Tirar a chave do JWT e a senha do banco de desenvolvimento do `appsettings.json` (a versão publicada já lê esses valores de variáveis de ambiente).

### Uso de IA

Desenvolvi este projeto com o Claude (Anthropic) como agente de IA, sob minha supervisão. Eu direcionei o trabalho e revisei cada alteração.

### Deploy

- Banco: Neon (Postgres gratuito).
- API: web service no Render, gerado a partir de `backend/TechServ.Api/Dockerfile`. A connection string, a chave do JWT e o endereço permitido do frontend vêm de variáveis de ambiente.
- Frontend: static site no Render (`npm run build`, com `VITE_API_URL` apontando para a API, e um rewrite de `/*` para `/index.html`).
