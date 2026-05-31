namespace GameShelf.API.DTOs;

// ──────────────────────────────────────────────────────────────
//  CLASE 4 — Por qué existen los DTOs
//
//  El modelo Game tiene InternalScore y AdminNotes.
//  Si devolviéramos Game directamente al cliente:
//    - Exponemos datos internos
//    - Creamos acoplamiento DB ↔ contrato de API
//    - Las referencias circulares (Game → Reviews → Game) rompen el JSON
//
//  GameDto es el CONTRATO. Solo lo que el cliente necesita.
//  Si mañana renombramos una columna en la DB, el DTO no cambia.
// ──────────────────────────────────────────────────────────────

/// <summary>
/// Contrato de respuesta para un juego.
/// AvgScore se calcula en el repositorio — no existe en la DB como campo.
/// Genres se aplana: en vez de objetos Genre, solo sus nombres.
/// </summary>
public class GameDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }

    /// <summary>Media de scores de todas las reviews. Null si no hay reviews.</summary>
    public double? AvgScore { get; set; }

    /// <summary>Nombres de los géneros aplanados (sin objetos anidados).</summary>
    public List<string> Genres { get; set; } = new();
}

/// <summary>
/// Contrato de respuesta para una review individual.
/// </summary>
public class ReviewDto
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Payload para crear una nueva review (entrada del cliente).
/// Solo los campos que el cliente puede enviar.
/// </summary>
public class CreateReviewDto
{
    public int Score { get; set; }       // validación: 1-10
    public string? Comment { get; set; }
}
