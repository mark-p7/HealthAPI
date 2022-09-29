FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY dist /app
WORKDIR /app
EXPOSE 80/tcp
ENTRYPOINT [ "dotnet", "HealthAPI.dll" ]
