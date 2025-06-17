# Neighborly

Neighborly to platforma społecznościowa, która pomaga sąsiadom wspierać się w codziennych obowiązkach, takich jak zakupy, opieka nad zwierzętami czy drobne naprawy. Projekt został stworzony w oparciu o ASP.NET Core MVC, Entity Framework Core oraz Tailwind CSS.

## Funkcje
- Konta użytkowników i profile
- Tworzenie oraz przeglądanie ogłoszeń pomocy sąsiedzkiej
- Wiadomości między użytkownikami
- Oceny, ulubione i zgłaszanie naruszeń

## Wymagania
- [.NET SDK 8.0](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) w wersji 18 lub nowszej
- Serwer SQL Server lub lokalna instancja LocalDB

## Konfiguracja projektu
1. **Sklonuj repozytorium**
   ```bash
   git clone <adres-repozytorium>
   cd Neighborly
   ```
2. **Przywróć zależności**
   ```bash
   dotnet restore
   cd Neighborly
   npm install
   ```
3. **Ustaw plik konfiguracyjny**
   Skopiuj `appsettings.json` lub utwórz nowy na podstawie poniższego przykładu. Uaktualnij łańcuch połączenia `DefaultConnection` podając dane swojego serwera SQL.
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=NeighborlyDb;Trusted_Connection=True;"
     },
     "GoogleMaps": {
       "ApiKey": ""
     }
   }
   ```
4. **Zastosuj migracje bazy danych**
   ```bash
   dotnet ef database update
   ```
   (W razie potrzeby zainstaluj narzędzie EF Core CLI poleceniem `dotnet tool install --global dotnet-ef`.)
5. **Zbuduj zasoby Tailwind CSS**
   ```bash
   npm run build-css       # jednorazowa kompilacja
   npm run watch-css       # obserwowanie zmian podczas pracy
   ```
6. **Uruchom aplikację**
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
