FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS production
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8000
CMD ["dotnet", "api.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . .
EXPOSE 8000
CMD ["dotnet", "watch", "run"]