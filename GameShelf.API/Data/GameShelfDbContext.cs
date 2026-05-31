using GameShelf.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameShelf.API.Data;

/// <summary>
/// DbContext de GameShelf.
/// Configura las relaciones y la tabla pivot GameGenre.
///
/// CLASE 3 — Modelado:
///   EF Core traduce estas configuraciones al esquema SQL.
///   La relación N-M (Game ↔ Genre) se configura aquí.
/// </summary>
public class GameShelfDbContext : DbContext
{
    public GameShelfDbContext(DbContextOptions<GameShelfDbContext> options)
        : base(options) { }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<GameGenre> GameGenres => Set<GameGenre>();
    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Tabla pivot GameGenre ──────────────────────────────
        // PK compuesta: (GameId, GenreId)
        modelBuilder.Entity<GameGenre>()
            .HasKey(gg => new { gg.GameId, gg.GenreId });

        modelBuilder.Entity<GameGenre>()
            .HasOne(gg => gg.Game)
            .WithMany(g => g.GameGenres)
            .HasForeignKey(gg => gg.GameId);

        modelBuilder.Entity<GameGenre>()
            .HasOne(gg => gg.Genre)
            .WithMany(g => g.GameGenres)
            .HasForeignKey(gg => gg.GenreId);

        // ── Review → Game (FK) ─────────────────────────────────
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Game)
            .WithMany(g => g.Reviews)
            .HasForeignKey(r => r.GameId);

        // ── Precisión decimal ──────────────────────────────────
        modelBuilder.Entity<Game>()
            .Property(g => g.InternalScore)
            .HasPrecision(5, 2);

        // ── Seed data (para clase, arranca con datos) ──────────
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "RPG" },
            new Genre { Id = 2, Name = "Aventura" },
            new Genre { Id = 3, Name = "Plataformas" },
            new Genre { Id = 4, Name = "Estrategia" }
        );

        modelBuilder.Entity<Game>().HasData(
            new Game { Id = 1, Title = "Hollow Knight",    ReleaseYear = 2017, InternalScore = 9.2m, AdminNotes = "Top seller" },
            new Game { Id = 2, Title = "The Witcher 3",    ReleaseYear = 2015, InternalScore = 9.5m, AdminNotes = "GOTY 2015" },
            new Game { Id = 3, Title = "Celeste",          ReleaseYear = 2018, InternalScore = 9.0m, AdminNotes = null },
            new Game { Id = 4, Title = "Hades",            ReleaseYear = 2020, InternalScore = 9.3m, AdminNotes = null },
            new Game { Id = 5, Title = "Disco Elysium",    ReleaseYear = 2019, InternalScore = 9.1m, AdminNotes = "Niche audience" }
        );

        modelBuilder.Entity<GameGenre>().HasData(
            new GameGenre { GameId = 1, GenreId = 3 },   // Hollow Knight → Plataformas
            new GameGenre { GameId = 1, GenreId = 2 },   // Hollow Knight → Aventura
            new GameGenre { GameId = 2, GenreId = 1 },   // Witcher 3 → RPG
            new GameGenre { GameId = 2, GenreId = 2 },   // Witcher 3 → Aventura
            new GameGenre { GameId = 3, GenreId = 3 },   // Celeste → Plataformas
            new GameGenre { GameId = 4, GenreId = 1 },   // Hades → RPG
            new GameGenre { GameId = 4, GenreId = 2 },   // Hades → Aventura
            new GameGenre { GameId = 5, GenreId = 1 }    // Disco Elysium → RPG
        );

        modelBuilder.Entity<Review>().HasData(
            new Review { Id = 1,  GameId = 1, Score = 10, Comment = "Obra maestra",        CreatedAt = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 2,  GameId = 1, Score = 9,  Comment = "Difícil pero épico",  CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 3,  GameId = 1, Score = 8,  Comment = "Muy bueno",           CreatedAt = new DateTime(2024, 2,  1, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 4,  GameId = 2, Score = 10, Comment = "Imprescindible",      CreatedAt = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 5,  GameId = 2, Score = 9,  Comment = "Historia increíble",  CreatedAt = new DateTime(2024, 2,  5, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 6,  GameId = 3, Score = 9,  Comment = "Plataformas y alma",  CreatedAt = new DateTime(2024, 3,  1, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 7,  GameId = 4, Score = 10, Comment = "Roguelike perfecto",  CreatedAt = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 8,  GameId = 4, Score = 8,  Comment = "Muy adictivo",        CreatedAt = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 9,  GameId = 5, Score = 10, Comment = "Único en su especie", CreatedAt = new DateTime(2024, 4,  1, 0, 0, 0, DateTimeKind.Utc) },
            new Review { Id = 10, GameId = 5, Score = 7,  Comment = "Muy denso, pero genial", CreatedAt = new DateTime(2024, 4, 5, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
