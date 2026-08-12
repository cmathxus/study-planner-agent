# Study Planner Agent

Projeto de estudo para aprender .NET, Clean Architecture, MCP e a base para uma futura integracao com Microsoft Foundry.

## Ideia

Um planner de estudos semanal onde cada dia tem um topico principal. Ao registrar estudo, o progresso minimo diario precisa ser de 20%.

## Projetos

```text
src/
  StudyPlannerAgent.Api
  StudyPlannerAgent.Application
  StudyPlannerAgent.Domain
  StudyPlannerAgent.Infrastructure
  StudyPlannerAgent.McpServer
```

## Rodar API

```bash
dotnet run --project src/StudyPlannerAgent.Api/StudyPlannerAgent.Api.csproj --urls http://localhost:5090
```

Swagger:

```text
http://localhost:5090/swagger
```

Endpoints iniciais:

```text
POST /auth/register
POST /auth/login
GET  /auth/me
GET  /study-topics
GET  /study-topics/{id}
POST /study-topics
PUT  /study-topics/{id}
DELETE /study-topics/{id}
GET  /study-plan/today
GET  /study-plan/week
POST /progress
GET  /progress/summary
```

Os endpoints de estudo usam JWT. Primeiro registre ou faca login:

```json
{
  "name": "Caio Matheus",
  "email": "caio@example.com",
  "password": "password123"
}
```

Use o `access_token` retornado no header:

```text
Authorization: Bearer <access_token>
```

Exemplo de topico:

```json
{
  "name": "EF Core",
  "description": "Estudar migrations, relacionamentos e tracking.",
  "weekday": "Friday"
}
```

Valores aceitos para `weekday`: `Sunday`, `Monday`, `Tuesday`, `Wednesday`, `Thursday`, `Friday`, `Saturday`.

Exemplo de progresso:

```json
{
  "topic_id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1",
  "percentage": 20,
  "notes": "Revisei LINQ e async/await."
}
```

## Rodar MCP Server

```bash
dotnet run --project src/StudyPlannerAgent.McpServer/StudyPlannerAgent.McpServer.csproj --urls http://localhost:5091
```

O MCP Server expoe tools para um agente chamar:

```text
get_today_study_plan
get_weekly_study_schedule
record_study_progress
get_progress_summary
```

As tools que consultam ou registram progresso recebem `userId` como parametro.

## Supabase + EF Core

Por padrao, o projeto usa dados em memoria. Para usar Supabase/Postgres:

1. Configure a connection string fora do Git:

```powershell
$env:ConnectionStrings__Supabase="postgresql://postgres:<password>@<host>:5432/postgres"
```

Tambem funciona no formato do Npgsql:

```powershell
$env:ConnectionStrings__Supabase="Host=<host>;Port=5432;Database=postgres;Username=postgres;Password=<password>;SSL Mode=Require"
```

2. Configure uma chave JWT fora do Git para deploy:

```powershell
$env:Jwt__Secret="use-uma-chave-grande-e-segura-aqui"
```

3. Rode a API ou o MCP Server normalmente.

O EF Core aplica as migrations automaticamente quando a connection string existe.
Se `ConnectionStrings__Supabase` nao existir, o projeto volta para os repositorios em memoria.

Se voce ja criou as tabelas antigas manualmente no Supabase, limpe essas tabelas antes de rodar a API com EF Core.

## Padroes usados

- Clean Architecture: separa dominio, casos de uso, infraestrutura e entradas externas.
- SOLID: os casos de uso dependem de interfaces, nao de banco ou framework.
- Result Pattern: erros esperados voltam como resultado, sem usar exception para fluxo comum.
- Repository Pattern: persistencia fica atras de interfaces.
- EF Core: mapeia as entidades de persistencia para Postgres/Supabase sem SQL na mao.
- Value Object: `ProgressPercentage` valida a regra de progresso minimo.
- Ports and Adapters: REST API e MCP Server sao portas diferentes usando a mesma aplicacao.
- JWT Authentication: usuarios fazem login e recebem token para acessar recursos protegidos.
- Password Hashing: senhas sao armazenadas com hash BCrypt.

## Proximo passo

Adicionar testes de integracao.
