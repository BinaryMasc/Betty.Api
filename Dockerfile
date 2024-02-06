FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /app

COPY . .

RUN dotnet restore

COPY . ./

#	Debug mode set
RUN dotnet dev-certs https
RUN dotnet build -c Debug -o /app/build

EXPOSE 5000

CMD ["dotnet", "run", "--project", "Betty.Api/Betty.Api.csproj"]

