namespace GameShelf.API.Models;

/// <summary>
/// Representa un género de videojuego (RPG, Aventura, Plataformas…).
/// Relación: 1 Genre → N GameGenre (N-M con Game via tabla pivot).
/// </summary>
public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navegación
    public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
}
