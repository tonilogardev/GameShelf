using GameShelf.API.DTOs;
using GameShelf.API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameShelf.API.Controllers;

/// <summary>
/// CLASE 4 — Controlador de juegos.
///
/// Responsabilidad ÚNICA del controlador:
///   1. Recibir la petición HTTP
///   2. Llamar al repositorio
///   3. Devolver la respuesta HTTP correcta (200 / 201 / 404 / 400)
///
/// Lo que el controlador NO hace:
///   - SQL (eso es el repositorio)
///   - Lógica de negocio compleja (eso iría en un Service)
///   - Mapear modelos a DTOs (eso lo hace el repositorio en este ejemplo)
/// </summary>
[ApiController]
[Route("api/games")]
public class GamesController : ControllerBase
{
    private readonly IGameRepository _repo;

    // CLASE 4 — Inyección de dependencias:
    //   Recibimos IGameRepository (interfaz), no GameRepository (implementación).
    //   En tests pasaremos un Mock<IGameRepository>.
    //   En producción, el contenedor de DI inyectará GameRepository.
    public GamesController(IGameRepository repo)
    {
        _repo = repo;
    }

    // ── GET /api/games ─────────────────────────────────────────
    /// <summary>Devuelve todos los juegos con nota media y géneros.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var games = await _repo.GetAllAsync();
        return Ok(games); // 200 + JSON array
        // Si la lista está vacía, devuelve 200 con [] — NO es un 404
    }

    // ── GET /api/games/{id} ────────────────────────────────────
    /// <summary>
    /// Devuelve un juego por id.
    /// CLASE 4 — 404 vs error de sistema:
    ///   Si el juego no existe → 404 (respuesta correcta, sistema funcionando)
    ///   Si el sistema cae     → 500 (error real)
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var game = await _repo.GetByIdAsync(id);
        if (game is null) return NotFound(); // 404
        return Ok(game); // 200
    }

    // ── GET /api/games/{id}/reviews ────────────────────────────
    /// <summary>
    /// Devuelve las reviews de un juego.
    /// CLASE 4 — Postman demo:
    ///   Si el juego existe pero no tiene reviews → 200 con []
    ///   Si el juego no existe → 404
    /// </summary>
    [HttpGet("{id:int}/reviews")]
    public async Task<IActionResult> GetReviews(int id)
    {
        if (!await _repo.ExistsAsync(id)) return NotFound();

        var reviews = await _repo.GetReviewsByGameAsync(id);
        return Ok(reviews); // 200 aunque la lista esté vacía
    }

    // ── POST /api/games/{id}/reviews ───────────────────────────
    /// <summary>
    /// Añade una review a un juego.
    /// CLASE 4 — Validación básica con [ApiController]:
    ///   Si Score está fuera de rango (requiere DataAnnotations en el DTO),
    ///   [ApiController] devuelve 400 automáticamente.
    /// </summary>
    [HttpPost("{id:int}/reviews")]
    public async Task<IActionResult> AddReview(int id, [FromBody] CreateReviewDto dto)
    {
        if (!await _repo.ExistsAsync(id)) return NotFound();

        if (dto.Score < 1 || dto.Score > 10)
            return BadRequest("La nota debe estar entre 1 y 10."); // 400

        var created = await _repo.AddReviewAsync(id, dto);

        // 201 Created + Location header apuntando al recurso padre
        return CreatedAtAction(nameof(GetReviews), new { id }, created);
    }
}
