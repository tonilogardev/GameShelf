namespace GameShelf.API.Models;

/// <summary>
/// Entidad central de GameShelf.
/// Tiene géneros (N-M via GameGenre) y reviews (1-N).
///
/// NOTA CLASE 4 — DTO:
///   Este modelo tiene campos internos (InternalScore, AdminNotes)
///   que NO deben exponerse al cliente. Por eso existe GameDto.
/// </summary>
public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }

    // Campos internos — solo para uso administrativo, NO van al DTO
    public decimal InternalScore { get; set; }
    public string? AdminNotes { get; set; }

    // Navegación
    public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
