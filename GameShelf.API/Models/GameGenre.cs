namespace GameShelf.API.Models;

/// <summary>
/// Tabla pivot que resuelve la relación N-M entre Game y Genre.
///
/// CLASE 3 — Modelado:
///   Un juego puede tener varios géneros (RPG + Aventura).
///   Un género agrupa muchos juegos.
///   Esta tabla solo conecta — no tiene lógica propia.
/// </summary>
public class GameGenre
{
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}
