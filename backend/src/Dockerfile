FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore "FarmGame.Api/FarmGame.Api.csproj"
RUN dotnet publish "FarmGame.Api/FarmGame.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000/
EXPOSE 5000
ENTRYPOINT ["dotnet", "FarmGame.Api.dll"]