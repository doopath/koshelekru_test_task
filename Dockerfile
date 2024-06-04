FROM mcr.microsoft.com/dotnet/sdk:8.0 AS runtime
WORKDIR /app
COPY dist/ ./
EXPOSE 5228
ENTRYPOINT ["dotnet", "api.dll", "--urls", "http://*:5228"]