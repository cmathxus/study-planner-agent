# Study Planner Agent

Projeto de estudo com .NET, Microsoft Foundry, MCP e Supabase.

A ideia é simples: um planner de estudos onde o usuário cadastra tópicos por dia da semana e registra o progresso dos estudos.

## Stack

- .NET 9
- ASP.NET Core
- EF Core
- PostgreSQL / Supabase
- JWT
- MCP Server
- Microsoft Foundry
- Azure Container Apps

## Projetos

```text
src/
  StudyPlannerAgent.Api
  StudyPlannerAgent.Application
  StudyPlannerAgent.Domain
  StudyPlannerAgent.Infrastructure
  StudyPlannerAgent.McpServer
```

## API

```bash
dotnet run --project src/StudyPlannerAgent.Api
```

Swagger:

```text
http://localhost:5082/swagger
```

Principais endpoints:

```text
POST   /auth/register
POST   /auth/login
GET    /auth/me

GET    /study-topics
POST   /study-topics
PUT    /study-topics/{id}
DELETE /study-topics/{id}

GET    /study-plan/today
GET    /study-plan/week

POST   /progress
GET    /progress/summary

POST   /chat
```

## MCP Server

```bash
dotnet run --project src/StudyPlannerAgent.McpServer --urls http://localhost:5091
```

Endpoint:

```text
http://localhost:5091/mcp
```

Tools disponíveis:

```text
get_today_study_plan
get_weekly_study_schedule
get_study_topics
get_study_topic_by_id
create_study_topic
update_study_topic
delete_study_topic
record_study_progress
get_progress_summary
```

## Variáveis de ambiente

Supabase:

```powershell
$env:ConnectionStrings__Supabase="postgresql://postgres:<password>@<host>:5432/postgres"
```

JWT:

```powershell
$env:Jwt__Secret="<secret>"
```

Foundry:

```powershell
$env:Foundry__Endpoint="<foundry-endpoint>"
$env:Foundry__AgentId="<agent-id>"
```

Se `ConnectionStrings__Supabase` não for informada, o projeto usa dados em memória.

## Docker MCP

```bash
docker build -f src/StudyPlannerAgent.McpServer/Dockerfile -t study-planner-mcp .
```

```bash
docker run --rm -p 5091:8080 \
  -e ConnectionStrings__Supabase="postgresql://postgres:<password>@<host>:5432/postgres" \
  study-planner-mcp
```

## Objetivo

Esse projeto foi criado para estudar, na prática, como uma API em .NET pode ser integrada com um agente do Microsoft Foundry usando MCP.
