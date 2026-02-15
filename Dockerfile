FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/AuthPlaypen.Api/AuthPlaypen.Api.csproj src/AuthPlaypen.Api/
RUN dotnet restore src/AuthPlaypen.Api/AuthPlaypen.Api.csproj
COPY src/AuthPlaypen.Api src/AuthPlaypen.Api
RUN dotnet publish src/AuthPlaypen.Api/AuthPlaypen.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "AuthPlaypen.Api.dll"]
