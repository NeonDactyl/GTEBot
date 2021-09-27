FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . ./
WORKDIR /src
RUN file="$(ls -1 ..)" && echo $file
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]

FROM build AS publish
RUN dotnet publish BlazorGuessTheElo -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorGuessTheElo.dll"]
