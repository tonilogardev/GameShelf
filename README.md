# GameShelf API — Proyecto de clase

Proyecto de demostración para las **clases 3 y 4** de la asignatura  
*Aplicaciones basadas en tecnologías web* — Bachelor en Informática, Universitat Carlemany.

---

## Estructura del proyecto

```
GameShelf/
├── GameShelf.sln
├── GameShelf.API/
│   ├── Controllers/
│   │   └── GamesController.cs      ← CLASE 4: controlador, DTOs, códigos HTTP
│   ├── Data/
│   │   └── GameShelfDbContext.cs   ← CLASE 3: DbContext, relaciones, seed
│   ├── DTOs/
│   │   └── GameDtos.cs             ← CLASE 4: contrato de API (≠ modelo de DB)
│   ├── Models/
│   │   ├── Game.cs                 ← CLASE 3: entidad central
│   │   ├── Genre.cs                ← CLASE 3: relación 1-N con Game
│   │   ├── GameGenre.cs            ← CLASE 3: tabla pivot N-M
│   │   └── Review.cs               ← CLASE 3: 1-N con Game, permite AVG/COUNT
│   ├── Repositories/
│   │   ├── IGameRepository.cs      ← CLASE 3: contrato del repositorio
│   │   └── GameRepository.cs       ← CLASE 3: implementación (todo el SQL aquí)
│   ├── Program.cs                  ← registro de servicios y pipeline
│   └── appsettings.json            ← connection string (LocalDB por defecto)
├── GameShelf.Tests/
│   └── GamesControllerTests.cs     ← CLASE 4: tests unitarios (Arrange/Act/Assert)
└── sql/
    ├── migrations/
    │   └── 01_create_schema.sql    ← CLASE 3: creación de tablas y FK
    └── seed/
        ├── 02_seed_data.sql        ← CLASE 3: datos de ejemplo
        ├── 03_clase3_queries.sql   ← CLASE 3: consultas demo en vivo
        └── 04_clase4_postman_reference.sql  ← CLASE 4: SQL equivalente a cada endpoint
```

---

## Requisitos

- Visual Studio 2022 (v17+)
- .NET 8 SDK
- SQL Server Express o LocalDB (incluido con VS 2022)

---

## Cómo arrancar

### Opción A — LocalDB (recomendado para clase, sin configuración)

1. Abrir `GameShelf.sln` en Visual Studio 2022
2. `Ctrl+F5` para arrancar sin depurador
3. La API aplica el esquema y el seed automáticamente al arrancar  
   (`EnsureCreated()` en `Program.cs`)
4. Abrir Swagger en `https://localhost:{puerto}/swagger`

### Opción B — SQL Server Express (connection string manual)

1. Ejecutar en SSMS:
   - `sql/migrations/01_create_schema.sql`
   - `sql/seed/02_seed_data.sql`
2. Ajustar `appsettings.json`:
```json
"GameShelf": "Server=.\\SQLEXPRESS;Database=GameShelfDb;Trusted_Connection=True;"
```
3. Arrancar la API

---

## Endpoints disponibles

| Método | Ruta                        | Descripción                          |
|--------|-----------------------------|--------------------------------------|
| GET    | `/api/games`                | Lista de juegos con nota media       |
| GET    | `/api/games/{id}`           | Un juego concreto (404 si no existe) |
| GET    | `/api/games/{id}/reviews`   | Reviews de un juego                  |
| POST   | `/api/games/{id}/reviews`   | Añadir una review (score 1-10)       |

### Ejemplos Postman (clase 4)

```
GET  https://localhost:7xxx/api/games           → 200 + lista
GET  https://localhost:7xxx/api/games/1         → 200 + Hollow Knight
GET  https://localhost:7xxx/api/games/999       → 404
GET  https://localhost:7xxx/api/games/1/reviews → 200 + lista de reviews

POST https://localhost:7xxx/api/games/1/reviews
Content-Type: application/json
{ "score": 9, "comment": "Increíble" }          → 201 Created

POST https://localhost:7xxx/api/games/1/reviews
{ "score": 15 }                                  → 400 Bad Request
```

---

## Tests

```
# Desde terminal en la raíz del proyecto:
dotnet test

# Desde Visual Studio:
Test → Run All Tests  (o Ctrl+R, A)
```

Los tests están en `GameShelf.Tests/GamesControllerTests.cs`.  
Usan **Moq** para aislar el controlador sin necesitar base de datos.

---

## Conceptos cubiertos

### Clase 3 — Bases de datos y patrones

| Concepto            | Dónde verlo en el código                        |
|---------------------|-------------------------------------------------|
| Relación 1-N        | `Game` → `Review` (FK `game_id`)               |
| Relación N-M        | `Game` ↔ `Genre` via `GameGenre` (pivot)        |
| SQL: JOIN + GROUP   | `sql/seed/03_clase3_queries.sql`                |
| SQL: AVG + HAVING   | Consulta 2 en el script de queries              |
| Repository Pattern  | `IGameRepository` + `GameRepository`            |
| Unit of Work        | `SaveChangesAsync()` en `AddReviewAsync`        |
| CRUD vs CQRS        | Comentarios en `IGameRepository.cs`             |

### Clase 4 — API REST y testing

| Concepto            | Dónde verlo en el código                        |
|---------------------|-------------------------------------------------|
| Controlador         | `GamesController.cs`                            |
| DTO vs Modelo       | `GameDto` vs `Game` (campos `internal_score`)   |
| Códigos HTTP        | `Ok()`, `NotFound()`, `BadRequest()`, `Created` |
| Inyección DI        | Constructor de `GamesController`                |
| Arrange/Act/Assert  | Todos los tests en `GamesControllerTests.cs`    |
| Mock de repositorio | `Mock<IGameRepository>` en los tests            |
| Demo Postman        | `sql/seed/04_clase4_postman_reference.sql`      |
