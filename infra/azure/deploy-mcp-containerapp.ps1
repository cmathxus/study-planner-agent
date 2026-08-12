param(
    [string] $ResourceGroup = "rg-study-planner-agent",
    [string] $Location = "brazilsouth",
    [string] $ContainerAppsEnvironment = "cae-study-planner-agent",
    [string] $ContainerAppName = "study-planner-mcp",
    [string] $AcrName = "studyplanneracr$((Get-Random -Minimum 10000 -Maximum 99999))",
    [string] $ImageName = "study-planner-mcp",
    [string] $ImageTag = "latest"
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($env:ConnectionStrings__Supabase)) {
    throw "Set ConnectionStrings__Supabase before running this script."
}

$image = "${ImageName}:${ImageTag}"

Write-Host "Registering Azure resource providers"
az provider register --namespace Microsoft.App --wait --output none
az provider register --namespace Microsoft.OperationalInsights --wait --output none
az provider register --namespace Microsoft.ContainerRegistry --wait --output none

Write-Host "Creating resource group: $ResourceGroup"
az group create `
    --name $ResourceGroup `
    --location $Location `
    --output none

Write-Host "Creating Azure Container Registry: $AcrName"
az acr create `
    --resource-group $ResourceGroup `
    --name $AcrName `
    --sku Basic `
    --admin-enabled true `
    --output none

Write-Host "Building image in ACR: $image"
az acr build `
    --registry $AcrName `
    --image $image `
    --file src/StudyPlannerAgent.McpServer/Dockerfile `
    . `
    --output none

$loginServer = az acr show `
    --name $AcrName `
    --query "loginServer" `
    --output tsv

$acrUsername = az acr credential show `
    --name $AcrName `
    --query "username" `
    --output tsv

$acrPassword = az acr credential show `
    --name $AcrName `
    --query "passwords[0].value" `
    --output tsv

Write-Host "Creating Container Apps environment: $ContainerAppsEnvironment"
az containerapp env create `
    --resource-group $ResourceGroup `
    --name $ContainerAppsEnvironment `
    --location $Location `
    --output none

Write-Host "Creating MCP Container App: $ContainerAppName"
az containerapp create `
    --resource-group $ResourceGroup `
    --name $ContainerAppName `
    --environment $ContainerAppsEnvironment `
    --image "$loginServer/$image" `
    --target-port 8080 `
    --ingress external `
    --registry-server $loginServer `
    --registry-username $acrUsername `
    --registry-password $acrPassword `
    --secrets "supabase-connection=$env:ConnectionStrings__Supabase" `
    --env-vars "ASPNETCORE_ENVIRONMENT=Production" "ConnectionStrings__Supabase=secretref:supabase-connection" `
    --cpu 0.25 `
    --memory 0.5Gi `
    --min-replicas 0 `
    --max-replicas 1 `
    --output none

$fqdn = az containerapp show `
    --resource-group $ResourceGroup `
    --name $ContainerAppName `
    --query "properties.configuration.ingress.fqdn" `
    --output tsv

Write-Host ""
Write-Host "MCP Server deployed."
Write-Host "Health URL: https://$fqdn/health"
Write-Host "MCP URL:    https://$fqdn/mcp"
