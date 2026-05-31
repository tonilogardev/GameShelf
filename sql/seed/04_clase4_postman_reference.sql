-- =============================================================
--  GameShelf — Script 04: Referencia SQL para demo Postman (clase 4)
--
--  Este archivo NO es para ejecutar en la API.
--  Es la traducción SQL de lo que hacen los endpoints,
--  útil para mostrar en clase la equivalencia entre
--  el código C# del repositorio y el SQL real.
-- =============================================================


-- =============================================================
--  GET /api/games
--  → GameRepository.GetAllAsync()
--
--  Devuelve todos los juegos con nota media y géneros.
--  Nota: internal_score y admin_notes NO aparecen (quedan en la DB).
-- =============================================================
SELECT
    g.id,
    g.title,
    g.release_year,
    ROUND(AVG(CAST(r.score AS FLOAT)), 1)                   AS avg_score,
    STRING_AGG(ge.name, ', ') WITHIN GROUP (ORDER BY ge.name) AS genres
FROM Game g
LEFT JOIN Review    r  ON r.game_id  = g.id
LEFT JOIN GameGenre gg ON gg.game_id = g.id
LEFT JOIN Genre     ge ON ge.id      = gg.genre_id
GROUP BY g.id, g.title, g.release_year
ORDER BY g.id;


-- =============================================================
--  GET /api/games/{id}       ejemplo: id = 1
--  → GameRepository.GetByIdAsync(1)
--
--  Si no existe → null en C# → 404 en el controlador
-- =============================================================
SELECT
    g.id,
    g.title,
    g.release_year,
    ROUND(AVG(CAST(r.score AS FLOAT)), 1)                   AS avg_score,
    STRING_AGG(ge.name, ', ') WITHIN GROUP (ORDER BY ge.name) AS genres
FROM Game g
LEFT JOIN Review    r  ON r.game_id  = g.id
LEFT JOIN GameGenre gg ON gg.game_id = g.id
LEFT JOIN Genre     ge ON ge.id      = gg.genre_id
WHERE g.id = 1
GROUP BY g.id, g.title, g.release_year;


-- =============================================================
--  GET /api/games/{id}/reviews       ejemplo: id = 1
--  → GameRepository.GetReviewsByGameAsync(1)
--
--  Si el juego existe pero no tiene reviews → lista vacía → 200 con []
--  Si el juego no existe → 404 (comprobado con ExistsAsync antes)
--
--  Pregunta Postman demo:
--  "¿Qué devolvemos si el juego existe pero no tiene reviews?"
--  → 200 con [] (distinto de 404)
-- =============================================================
SELECT
    r.id,
    r.score,
    r.comment,
    r.created_at
FROM Review r
WHERE r.game_id = 1
ORDER BY r.created_at DESC;

-- Verificar que el juego existe (equivale a ExistsAsync)
SELECT COUNT(1) FROM Game WHERE id = 1;


-- =============================================================
--  POST /api/games/{id}/reviews
--  → GameRepository.AddReviewAsync(1, dto)
--
--  CLASE 3 — Unit of Work:
--  En la implementación real, el INSERT no se ejecuta hasta
--  SaveChangesAsync(). Si hubiera más operaciones antes,
--  irían en la misma transacción.
-- =============================================================
-- Equivalente al AddReviewAsync:
INSERT INTO Review (game_id, score, comment, created_at)
VALUES (1, 8, 'Muy adictivo', GETUTCDATE());

-- Verificar que se insertó:
SELECT TOP 1 * FROM Review ORDER BY id DESC;


-- =============================================================
--  CÓDIGOS DE ESTADO — referencia rápida para la demo Postman
--
--  200 OK          → GET exitoso (incluso con lista vacía)
--  201 Created     → POST exitoso, nuevo recurso creado
--  400 Bad Request → score fuera de rango (1-10)
--  404 Not Found   → juego no existe
--  500 Server Error→ excepción no controlada (bug real)
-- =============================================================
