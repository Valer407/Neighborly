# Neighborly

Neighborly to platforma społecznościowa ułatwiająca sąsiadom wymianę pomocy w codziennych obowiązkach. Projekt oparty jest o ASP.NET Core i Tailwind CSS.

## Wykorzystane biblioteki

### Backend (.NET)
- **Microsoft.EntityFrameworkCore** 9.0.6
- **Microsoft.EntityFrameworkCore.Design** 9.0.6
- **Microsoft.EntityFrameworkCore.SqlServer** 9.0.6

### Testy jednostkowe
- **Azure.Core** 1.38.0
- **Microsoft.AspNetCore.Mvc** 2.3.0
- **Microsoft.EntityFrameworkCore** 9.0.6
- **Microsoft.EntityFrameworkCore.InMemory** 9.0.6
- **Moq** 4.20.72
- **xunit** 2.9.3
- **xunit.runner.visualstudio** 2.5.4
- **Microsoft.NET.Test.Sdk** 17.10.0

### Frontend 
- **autoprefixer** 10.4.21
- **postcss** 8.5.3
- **postcss-cli** 11.0.1
- **tailwindcss** 3.4.17
- **tailwindcss-animate** 1.0.7

## Instalacja i konfiguracja

1. Zainstaluj **.NET SDK 8.0** oraz **Node.js 18+**.
2. Sklonuj repozytorium:
   ```bash
   git clone <adres-repozytorium>
   cd Neighborly
   ```
3. Przywróć zależności projektu:
   ```bash
   dotnet restore
   cd Neighborly
   npm install
   ```
4. Skonfiguruj plik `appsettings.json` (lub `appsettings.Development.json`):
   - ustaw łańcuch połączenia `ConnectionStrings:DefaultConnection`
   - wprowadź klucz `GoogleMaps:ApiKey`
5. Zastosuj migracje bazy danych:
   ```bash
   dotnet ef database update
   ```
   Jeśli narzędzie `dotnet-ef` nie jest zainstalowane:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
6. Zbuduj zasoby Tailwind CSS:
   ```bash
   npm run build-css       # jednorazowa kompilacja
   npm run watch-css       # obserwowanie zmian podczas pracy
   ```
7. Uruchom aplikację:
   ```bash
   dotnet run --project Neighborly
   ```
   Do automatycznej rekompilacji możesz użyć `dotnet watch run`.

## Struktura projektu
- `Neighborly/` – główny projekt ASP.NET Core MVC
  - `Controllers/` – kontrolery MVC
  - `Data/` – kontekst EF Core i migracje
  - `Models/` – modele domenowe
  - `Views/` – widoki Razor
  - `wwwroot/` – pliki statyczne i skompilowane zasoby CSS/JS
  - `package.json` oraz `tailwind.config.js` – konfiguracja narzędzi frontendowych
