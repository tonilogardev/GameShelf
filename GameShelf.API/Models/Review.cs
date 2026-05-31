namespace GameShelf.API.Models;

/// <summary>
/// Review de un usuario sobre un juego.
/// Relación: N Reviews → 1 Game (FK game_id).
///
/// CLASE 3 — SQL:
///   Esta tabla permite consultas con AVG(score), COUNT, GROUP BY.
/// </summary>
public class Review
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    /// <summary>Nota del 1 al 10.</summary>
    public int Score { get; set; }

    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
