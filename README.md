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

Endpoints iniciais:

```text
GET  /study-plan/today
GET  /study-plan/week
POST /progress
GET  /progress/summary
```

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

## Supabase

Por padrao, o projeto usa dados em memoria. Para usar Supabase/Postgres:

1. Rode o script no SQL Editor do Supabase:

```text
database/schema.sql
```

2. Configure a connection string fora do Git:

```powershell
$env:ConnectionStrings__Supabase="postgresql://postgres:<password>@<host>:5432/postgres"
```

3. Rode a API ou o MCP Server normalmente.

Se `ConnectionStrings__Supabase` nao existir, o projeto volta para os repositorios em memoria.

## Padroes usados

- Clean Architecture: separa dominio, casos de uso, infraestrutura e entradas externas.
- SOLID: os casos de uso dependem de interfaces, nao de banco ou framework.
- Result Pattern: erros esperados voltam como resultado, sem usar exception para fluxo comum.
- Repository Pattern: persistencia fica atras de interfaces.
- Value Object: `ProgressPercentage` valida a regra de progresso minimo.
- Ports and Adapters: REST API e MCP Server sao portas diferentes usando a mesma aplicacao.

## Proximo passo

Adicionar migrations automatizadas e testes de integracao.
