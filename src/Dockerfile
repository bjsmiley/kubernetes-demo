FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
# EXPOSE 80
# EXPOSE 443
EXPOSE 8080

# RUN apt-get update
# RUN apt-get -y install libnss3-tools
# RUN mkdir -p $HOME/.pki/nssdb

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Kube.Demo.csproj", "./"]
RUN dotnet restore "Kube.Demo.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "Kube.Demo.csproj" -c Release -o /app/build
# RUN dotnet dev-certs https
# RUN dotnet dev-certs https -ep /usr/local/share/ca-certificates/aspnet/https.crt --format PEM
# RUN dotnet dev-certs https --trust

FROM build AS publish
RUN dotnet publish "Kube.Demo.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use openssl to generate a self signed certificate cert.pfx with password $env:certPassword
WORKDIR /app/publish
ENV certPassword supersecretpazzword123
RUN openssl genrsa -des3 -passout pass:${certPassword} -out server.key 2048
RUN openssl rsa -passin pass:${certPassword} -in server.key -out server.key
RUN openssl req -sha256 -new -key server.key -out server.csr -subj '/CN=weatherapi-service'
RUN openssl x509 -req -sha256 -days 3650 -in server.csr -signkey server.key -out server.crt
RUN openssl pkcs12 -export -out cert.pfx -inkey server.key -in server.crt -passout pass:${certPassword}
# RUN chmod 755 /app/cert.pfx

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chmod 755 /app/cert.pfx
ENTRYPOINT ["dotnet", "Kube.Demo.dll"]
