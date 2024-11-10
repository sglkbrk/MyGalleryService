# Base image olarak ASP.NET runtime kullanılıyor
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

# SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve restore et
COPY ["MyGallery.Api/MyGallery.Api.csproj", "MyGallery.Api/"]
COPY ["MyGallery.Data/MyGallery.Data.csproj", "MyGallery.Data/"]
COPY ["MyGallery.Domain/MyGallery.Domain.csproj", "MyGallery.Domain/"]
COPY ["MyGallery.Services/MyGallery.Services.csproj", "MyGallery.Services/"]
RUN dotnet restore "MyGallery.Api/MyGallery.Api.csproj"

# Diğer dosyaları kopyalayın ve uygulamayı build edin
COPY . .
WORKDIR "/src/MyGallery.Api"
RUN dotnet build "MyGallery.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyGallery.Api.csproj" -c Release -o /app/publish

# Son olarak uygulamayı çalıştırmak için base image'e geçiş yapıyoruz
FROM base AS final
WORKDIR /app
COPY --from=build /src/MyGallery.Api/wwwroot ./wwwroot
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:5001
ENTRYPOINT ["dotnet", "MyGallery.Api.dll"]
