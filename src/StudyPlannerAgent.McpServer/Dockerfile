FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY StudyPlannerAgent.sln ./
COPY src/StudyPlannerAgent.Domain/StudyPlannerAgent.Domain.csproj src/StudyPlannerAgent.Domain/
COPY src/StudyPlannerAgent.Application/StudyPlannerAgent.Application.csproj src/StudyPlannerAgent.Application/
COPY src/StudyPlannerAgent.Infrastructure/StudyPlannerAgent.Infrastructure.csproj src/StudyPlannerAgent.Infrastructure/
COPY src/StudyPlannerAgent.McpServer/StudyPlannerAgent.McpServer.csproj src/StudyPlannerAgent.McpServer/

RUN dotnet restore src/StudyPlannerAgent.McpServer/StudyPlannerAgent.McpServer.csproj

COPY . .

RUN dotnet publish src/StudyPlannerAgent.McpServer/StudyPlannerAgent.McpServer.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StudyPlannerAgent.McpServer.dll"]
