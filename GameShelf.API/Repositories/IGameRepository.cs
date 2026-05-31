using GameShelf.API.DTOs;
using GameShelf.API.Models;

namespace GameShelf.API.Repositories;

/// <summary>
/// Contrato del repositorio de juegos.
///
/// CLASE 3 — Repository Pattern:
///   El controlador solo conoce esta interfaz.
///   No sabe si por detrás hay SQL Server, un array en memoria o un mock.
///   Eso es exactamente lo que queremos: poder cambiar la implementación
///   sin tocar el controlador ni los tests.
/// </summary>
public interface IGameRepository
{
    /// <summary>Devuelve todos los juegos con nota media y géneros (ya mapeados a DTO).</summary>
    Task<IEnumerable<GameDto>> GetAllAsync();

    /// <summary>Devuelve un juego por id, o null si no existe.</summary>
    Task<GameDto?> GetByIdAsync(int id);

    /// <summary>Devuelve las reviews de un juego concreto.</summary>
    Task<IEnumerable<ReviewDto>> GetReviewsByGameAsync(int gameId);

    /// <summary>Añade una review a un juego. Devuelve la review creada.</summary>
    Task<ReviewDto> AddReviewAsync(int gameId, CreateReviewDto dto);

    /// <summary>Comprueba si existe un juego con ese id.</summary>
    Task<bool> ExistsAsync(int id);
}
