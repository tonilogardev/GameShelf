using GameShelf.API.Data;
using GameShelf.API.DTOs;
using GameShelf.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameShelf.API.Repositories;

/// <summary>
/// Implementación de IGameRepository usando EF Core + SQL Server.
///
/// CLASE 3 — Repository Pattern:
///   Todo el SQL vive aquí. El controlador nunca escribe LINQ ni SQL.
///   Si mañana migramos a PostgreSQL, solo cambia este archivo.
///
/// CLASE 3 — Unit of Work:
///   SaveChangesAsync() es el punto de confirmación de la transacción.
///   Si el INSERT en Review falla, nada se persiste.
/// </summary>
public class GameRepository : IGameRepository
{
    private readonly GameShelfDbContext _db;

    public GameRepository(GameShelfDbContext db)
    {
        _db = db;
    }

    // ── GET ALL ────────────────────────────────────────────────
    // CLASE 3 — equivalente SQL:
    //   SELECT g.title,
    //          AVG(r.score)        AS avg_score,
    //          STRING_AGG(ge.name, ', ') AS genres
    //   FROM Game g
    //   LEFT JOIN Review r    ON r.game_id = g.id
    //   LEFT JOIN GameGenre gg ON gg.game_id = g.id
    //   LEFT JOIN Genre ge    ON ge.id = gg.genre_id
    //   GROUP BY g.id, g.title, g.release_year
    public async Task<IEnumerable<GameDto>> GetAllAsync()
    {
        return await _db.Games
            .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
            .Include(g => g.Reviews)
            .Select(g => new GameDto
            {
                Id          = g.Id,
                Title       = g.Title,
                ReleaseYear = g.ReleaseYear,
                AvgScore    = g.Reviews.Any()
                                ? Math.Round(g.Reviews.Average(r => r.Score), 1)
                                : null,
                Genres      = g.GameGenres.Select(gg => gg.Genre.Name).ToList()
            })
            .ToListAsync();
    }

    // ── GET BY ID ──────────────────────────────────────────────
    // Devuelve null si no existe → el controlador responde 404
    public async Task<GameDto?> GetByIdAsync(int id)
    {
        var game = await _db.Games
            .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
            .Include(g => g.Reviews)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game is null) return null;

        return new GameDto
        {
            Id          = game.Id,
            Title       = game.Title,
            ReleaseYear = game.ReleaseYear,
            AvgScore    = game.Reviews.Any()
                            ? Math.Round(game.Reviews.Average(r => r.Score), 1)
                            : null,
            Genres      = game.GameGenres.Select(gg => gg.Genre.Name).ToList()
        };
    }

    // ── GET REVIEWS BY GAME ────────────────────────────────────
    // CLASE 3 — equivalente SQL:
    //   SELECT id, score, comment, created_at
    //   FROM Review
    //   WHERE game_id = @gameId
    //   ORDER BY created_at DESC
    public async Task<IEnumerable<ReviewDto>> GetReviewsByGameAsync(int gameId)
    {
        return await _db.Reviews
            .Where(r => r.GameId == gameId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewDto
            {
                Id        = r.Id,
                Score     = r.Score,
                Comment   = r.Comment,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    // ── ADD REVIEW ─────────────────────────────────────────────
    // CLASE 3 — Unit of Work:
    //   El INSERT no se ejecuta hasta SaveChangesAsync().
    //   Si hubiera más operaciones antes del Save, irían en la misma transacción.
    public async Task<ReviewDto> AddReviewAsync(int gameId, CreateReviewDto dto)
    {
        var review = new Review
        {
            GameId    = gameId,
            Score     = dto.Score,
            Comment   = dto.Comment,
            CreatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);
        await _db.SaveChangesAsync(); // ← aquí se confirma la transacción

        return new ReviewDto
        {
            Id        = review.Id,
            Score     = review.Score,
            Comment   = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }

    // ── EXISTS ─────────────────────────────────────────────────
    public async Task<bool> ExistsAsync(int id)
        => await _db.Games.AnyAsync(g => g.Id == id);
}
