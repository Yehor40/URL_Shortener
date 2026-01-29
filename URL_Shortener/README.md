# URL Shortener

A modern URL Shortener application built with .NET 8 (Clean Architecture) and Angular. This project demonstrates a robust implementation of URL shortening with analytics, user management, and a clean separation of concerns.

## 🚀 Technologies

- **Backend**: .NET 8, ASP.NET Core Web API
- **Frontend**: Angular 19+ (with Standalone Components)
- **Database**: SQLite
- **Architecture**: Clean Architecture (Onion Architecture)
  - **CQRS**: Command Query Responsibility Segregation with MediatR
  - **Validation**: FluentValidation
  - **Mapping**: AutoMapper
- **Containerization**: Docker & Docker Compose

## 🏗 Architecture

The solution follows the Clean Architecture principles to ensure scalability, testability, and maintainability.

- **Domain**: Contains enterprise logic and entities (e.g., `ShortUrl`, `User`). No dependencies.
- **Application**: Contains business logic, CQRS handlers (Commands/Queries), and interfaces. DEPENDS ON Domain.
- **Infrastructure**: Implements interfaces (e.g., Database Context, Identity). DEPENDS ON Application.
- **Presentation (API)**: The entry point. DEPENDS ON Application and Infrastructure.
- **ClientApp**: Angular Single Page Application (SPA).

## 🐳 Getting Started with Docker

You can run the entire application stack using Docker Compose.

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

### Running the App

1. **Clone the repository** (if you haven't already).
2. **Navigate to the solution root** (where `docker-compose.yml` is).
3. **Build and start the containers**:
   ```bash
   docker-compose up --build
   ```

4. **Access the Application**:
   - **Frontend**: Open [http://localhost:4200](http://localhost:4200) in your browser.
   - **Backend API**: Accessible at [http://localhost:5094](http://localhost:5000).
   - **Swagger UI**: [http://localhost:5094/swagger](http://localhost:5000/swagger) (Note: Swagger might be disabled in Production mode by default, check `Program.cs` or set `ASPNETCORE_ENVIRONMENT=Development` in `docker-compose.yml` if you need it).

### Stopping the App
Press `Ctrl+C` in the terminal or run:
```bash
docker-compose down
```

## 🛠 Local Development (Without Docker)

### Backend
1. Navigate to `URL_Shortener` project directory.
2. Run `dotnet restore`.
3. Run `dotnet run`.
4. API runs on `http://localhost:5094` (or configured port).

### Frontend
1. Navigate to `URL_Shortener/ClientApp`.
2. Run `npm install`.
3. Run `npm start`.
4. App runs on `http://localhost:4200`.

## ✨ Features
- Shorten long URLs.
- Redirect to original URLs.
- Admin/User authentication.
