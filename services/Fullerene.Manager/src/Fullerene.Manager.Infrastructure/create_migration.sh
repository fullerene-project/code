dotnet ef migrations add "$1" \
  --project ./Fullerene.Manager.Infrastructure.csproj \
  --startup-project ../Fullerene.Manager.Api/Fullerene.Manager.Api.csproj \
  --output-dir ./Persistence/Migrations
