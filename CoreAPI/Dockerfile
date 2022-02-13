#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS base
WORKDIR /app
EXPOSE 9122
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["/", ""]
RUN dotnet restore "./CoreAPI/CoreAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CoreAPI/CoreAPI.csproj" -c Release -o /app/build

FROM build AS publish
COPY --from=build /app CoreAPI/Uploads
RUN dotnet publish "CoreAPI/CoreAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app 
COPY --from=publish /app/publish .
#define value for connection string as per env
ENV DB_Conn "<ConnectionString>"
ENTRYPOINT ["dotnet", "CoreAPI.dll"]