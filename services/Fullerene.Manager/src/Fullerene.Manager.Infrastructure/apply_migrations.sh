#!/bin/bash

dotnet ef database update \
  --project ./Fullerene.Manager.Infrastructure.csproj \
  --startup-project ../Fullerene.Manager.Api/Fullerene.Manager.Api.csproj
