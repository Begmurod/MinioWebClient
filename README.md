# MinioWebClient
Основные функции веб-приложения:
1) Подключение веб-приложения к Minio с помощью API;
2) Экспорт и импорт документов в облачное хранилище через веб-приложение;
3) Веб-интерфейс для просмотра файлов;
4) Настройка контейнера в docker для запуска Minio.
Параметры API:
{
  "profiles": {
    "MinioWebClient": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:8089",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "MINIO_URI": "http://127.0.0.1:9000",
        "MINIO_BUCKET": "minio-web-client",
        "MINIO_ACCESS_KEY": "minioadmin",
        "MINIO_SECRET_KEY": "minioadmin" 
      }
    }
  }
}

Docker Compose позволяет управлять набором контейнеров, каждый из которых представляет собой один сервис проекта. Управление включает в себя сборку, 
запуск с учетом зависимостей и конфигурацию. Конфигурация Docker Compose описывается в файле docker-compose.yml, лежащем в корне проекта.

Пример файла docker-compose.yml
version: "3.8"
services:
  minio:
    image: minio/minio:RELEASE.2022-09-17T00-09-45Z
    expose:
      - "9000"
    entrypoint: sh
    command: -c 'mkdir -p /export/minio-web-client && /opt/bin/minio server /export'
    environment:
      MINIO_ROOT_USER: yUwu8ByW61
      MINIO_ROOT_PASSWORD: m8bPDnedl2Lqos4pB3JVkrMTYwljPwAx
  
  web:
    build: .
    ports:
      - "8080:80"
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      MINIO_URI: http://minio:9000
      MINIO_BUCKET: minio-web-client
      MINIO_ACCESS_KEY: yUwu8ByW61
      MINIO_SECRET_KEY: m8bPDnedl2Lqos4pB3JVkrMTYwljPwAx
    depends_on:
      - minio    
      
 Dockerfile — это текстовый файл с инструкциями, необходимыми для создания образа контейнера.
 Пример Dockerfile
 FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY ./ .
RUN dotnet restore -r linux-musl-x64

# copy everything else and build app
WORKDIR /source/MinioWebClient
RUN dotnet publish -c release -o /publish -r linux-musl-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine-amd64
WORKDIR /app
COPY --from=build /publish ./

ENTRYPOINT ["./MinioWebClient"]


