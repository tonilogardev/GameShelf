using GameShelf.API.Controllers;
using GameShelf.API.DTOs;
using GameShelf.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameShelf.Tests;

/// <summary>
/// CLASE 4 — Tests unitarios del GamesController.
///
/// Por qué tests unitarios y no de integración aquí:
///   - No necesitan DB → rápidos y sin dependencias externas
///   - Testean la lógica del controlador de forma aislada
///   - Usan Mock para controlar exactamente qué devuelve el repositorio
///
/// Patrón: Arrange / Act / Assert
///   Arrange  → preparamos el escenario (mock + controller)
///   Act      → ejecutamos la acción
///   Assert   → comprobamos el resultado
/// </summary>
public class GamesControllerTests
{
    // ── Helper: crea un mock limpio del repositorio ─────────────
    private static (GamesController controller, Mock<IGameRepository> mock) CreateSut()
    {
        var mock = new Mock<IGameRepository>();
        var controller = new GamesController(mock.Object);
        return (controller, mock);
    }

    // ══════════════════════════════════════════════════════════
    //  GetAll
    // ══════════════════════════════════════════════════════════

    [Fact]
    public async Task GetAll_Returns200_WithGameList()
    {
        // ARRANGE
        var (controller, mock) = CreateSut();
        var fakeGames = new List<GameDto>
        {
            new() { Id = 1, Title = "Hollow Knight", AvgScore = 9.0, Genres = ["Plataformas", "Aventura"] },
            new() { Id = 2, Title = "Hades",         AvgScore = 9.0, Genres = ["RPG"] }
        };
        mock.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeGames);

        // ACT
        var result = await controller.GetAll();

        // ASSERT
        var ok = Assert.IsType<OkObjectResult>(result);
        var games = Assert.IsAssignableFrom<IEnumerable<GameDto>>(ok.Value);
        Assert.Equal(2, games.Count());
    }

    [Fact]
    public async Task GetAll_Returns200_WithEmptyList_WhenNoGames()
    {
        // ARRANGE — repositorio vacío
        var (controller, mock) = CreateSut();
        mock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<GameDto>());

        // ACT
        var result = await controller.GetAll();

        // ASSERT — 200 con lista vacía, NO un 404
        var ok = Assert.IsType<OkObjectResult>(result);
        var games = Assert.IsAssignableFrom<IEnumerable<GameDto>>(ok.Value);
        Assert.Empty(games);
    }

    // ══════════════════════════════════════════════════════════
    //  GetById
    // ══════════════════════════════════════════════════════════

    [Fact]
    public async Task GetById_Returns200_WithGame_WhenExists()
    {
        // ARRANGE
        var (controller, mock) = CreateSut();
        var fakeGame = new GameDto { Id = 1, Title = "Hollow Knight", AvgScore = 9.0 };
        mock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeGame);

        // ACT
        var result = await controller.GetById(1);

        // ASSERT
        var ok = Assert.IsType<OkObjectResult>(result);
        var game = Assert.IsType<GameDto>(ok.Value);
        Assert.Equal("Hollow Knight", game.Title);
    }

    [Fact]
    public async Task GetById_Returns404_WhenGameNotFound()
    {
        // ARRANGE — el juego 999 no existe
        var (controller, mock) = CreateSut();
        mock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((GameDto?)null);

        // ACT
        var result = await controller.GetById(999);

        // ASSERT — 404, no una excepción
        Assert.IsType<NotFoundResult>(result);
    }

    // ══════════════════════════════════════════════════════════
    //  GetReviews
    // ══════════════════════════════════════════════════════════

    [Fact]
    public async Task GetReviews_Returns200_WithReviews_WhenGameExists()
    {
        // ARRANGE
        var (controller, mock) = CreateSut();
        mock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        mock.Setup(r => r.GetReviewsByGameAsync(1)).ReturnsAsync(new List<ReviewDto>
        {
            new() { Id = 1, Score = 10, Comment = "Obra maestra" },
            new() { Id = 2, Score = 9,  Comment = "Muy difícil pero épico" }
        });

        // ACT
        var result = await controller.GetReviews(1);

        // ASSERT
        var ok = Assert.IsType<OkObjectResult>(result);
        var reviews = Assert.IsAssignableFrom<IEnumerable<ReviewDto>>(ok.Value);
        Assert.Equal(2, reviews.Count());
    }

    [Fact]
    public async Task GetReviews_Returns200_WithEmptyList_WhenGameHasNoReviews()
    {
        // ARRANGE — el juego existe pero no tiene reviews
        var (controller, mock) = CreateSut();
        mock.Setup(r => r.ExistsAsync(3)).ReturnsAsync(true);
        mock.Setup(r => r.GetReviewsByGameAsync(3)).ReturnsAsync(new List<ReviewDto>());

        // ACT
        var result = await controller.GetReviews(3);

        // ASSERT — 200 con [], NO un 404
        var ok = Assert.IsType<OkObjectResult>(result);
        var reviews = Assert.IsAssignableFrom<IEnumerable<ReviewDto>>(ok.Value);
        Assert.Empty(reviews);
    }

    [Fact]
    public async Task GetReviews_Returns404_WhenGameNotFound()
    {
        // ARRANGE
        var (controller, mock) = CreateSut();
        mock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // ACT
        var result = await controller.GetReviews(999);

        // ASSERT
        Assert.IsType<NotFoundResult>(result);
    }

    // ══════════════════════════════════════════════════════════
    //  AddReview
    // ══════════════════════════════════════════════════════════

    [Fact]
    public async Task AddReview_Returns201_WhenValid()
    {
        // ARRANGE
        var (controller, mock) = CreateSut();
        var dto = new CreateReviewDto { Score = 8, Comment = "Muy bueno" };
        var created = new ReviewDto { Id = 11, Score = 8, Comment = "Muy bueno", CreatedAt = DateTime.UtcNow };

        mock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        mock.Setup(r => r.AddReviewAsync(1, dto)).ReturnsAsync(created);

        // ACT
        var result = await controller.AddReview(1, dto);

        // ASSERT — 201 Created
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
    }

    [Fact]
    public async Task AddReview_Returns400_WhenScoreOutOfRange()
    {
        // ARRANGE
        var (controller, mock) = CreateSut();
        mock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        var dto = new CreateReviewDto { Score = 15 }; // inválido

        // ACT
        var result = await controller.AddReview(1, dto);

        // ASSERT — 400 Bad Request
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddReview_Returns404_WhenGameNotFound()
    {
        // ARRANGE
        var (controller, mock) = CreateSut();
        mock.Setup(r => r.ExistsAsync(99)).ReturnsAsync(false);

        // ACT
        var result = await controller.AddReview(99, new CreateReviewDto { Score = 8 });

        // ASSERT
        Assert.IsType<NotFoundResult>(result);
    }
}
