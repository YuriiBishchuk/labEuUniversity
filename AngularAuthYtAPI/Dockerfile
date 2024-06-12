FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AngularAuthYtAPI/Lab.API.csproj", "AngularAuthYtAPI/"]
COPY ["src/Lab.Business/Lab.Business.csproj", "src/Lab.Business/"]
COPY ["src/Lab.DataAccess/Lab.DataAccess.csproj", "src/Lab.DataAccess/"]
RUN dotnet restore "./AngularAuthYtAPI/Lab.API.csproj"
COPY . .
WORKDIR "/src/AngularAuthYtAPI"
# Встановлюємо Node.js та npm
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

# Встановлюємо Angular CLI
RUN npm install -g @angular/cli

# Копіюємо файли зі збірки та встановлюємо залежності Angular
WORKDIR ./ClientApp
RUN npm install
RUN npm run build

# Поверніться назад до батьківської директорії ./ClientApp
WORKDIR ..

# Виконайте збірку проекту
RUN dotnet build "./Lab.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Lab.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Копіюємо папку dist поруч з Lab.API.dll
COPY --from=build /src/AngularAuthYtAPI/ClientApp/dist ./ClientApp/dist

ENTRYPOINT ["dotnet", "Lab.API.dll"]
