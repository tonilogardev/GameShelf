-- =============================================================
--  GameShelf — Script 03: Consultas de clase 3 (demo en vivo)
--  CLASE 3: Bases de datos y modelado relacional
--
--  Ejecutar DESPUÉS de 01 y 02.
--  Cada bloque corresponde a un momento de la clase.
-- =============================================================


-- =============================================================
--  CONSULTA 1 — ¿Cuántas reviews tiene cada juego?
--  Concepto: LEFT JOIN + COUNT + GROUP BY + ORDER BY
--
--  Pregunta al alumno antes de ejecutar:
--  "¿Por qué LEFT JOIN y no JOIN normal?"
--  → Para ver también juegos SIN reviews (aparecerían con 0)
-- =============================================================
SELECT
    g.title,
    COUNT(r.id) AS total_reviews
FROM Game g
LEFT JOIN Review r ON r.game_id = g.id
GROUP BY g.id, g.title
ORDER BY total_reviews DESC;


-- =============================================================
--  CONSULTA 2 — Nota media por juego (solo los que tienen reviews)
--  Concepto: AVG + ROUND + HAVING
--
--  Pregunta al alumno:
--  "¿Qué diferencia hay entre WHERE y HAVING?"
--  → WHERE filtra filas ANTES de agrupar
--  → HAVING filtra grupos DESPUÉS de agrupar
-- =============================================================
SELECT
    g.title,
    ROUND(AVG(CAST(r.score AS FLOAT)), 1) AS avg_score
FROM Game g
JOIN Review r ON r.game_id = g.id
GROUP BY g.id, g.title
HAVING AVG(CAST(r.score AS FLOAT)) >= 7
ORDER BY avg_score DESC;


-- =============================================================
--  CONSULTA 3 — ¿Qué géneros tiene cada juego? (N-M en acción)
--  Concepto: dos JOINs encadenados a través de la tabla pivot
--
--  Pregunta al alumno:
--  "¿Por qué dos JOINs?"
--  → Hay tres tablas: Game → GameGenre → Genre
--  → La pivot no tiene nombre, solo conecta
-- =============================================================
SELECT
    g.title,
    ge.name AS genre
FROM Game g
JOIN GameGenre gg ON gg.game_id = g.id
JOIN Genre     ge ON ge.id       = gg.genre_id
ORDER BY g.title, ge.name;


-- =============================================================
--  MINI RETO (alumnos solos, ~3 min)
--
--  Enunciado:
--  "Quiero el título del juego y su nota media,
--   pero solo de los juegos que sean de género RPG."
--
--  Solución esperada:
-- =============================================================
SELECT
    g.title,
    ROUND(AVG(CAST(r.score AS FLOAT)), 1) AS avg_score
FROM Game g
JOIN GameGenre gg ON gg.game_id  = g.id
JOIN Genre     ge ON ge.id       = gg.genre_id
JOIN Review    r  ON r.game_id   = g.id
WHERE ge.name = 'RPG'
GROUP BY g.id, g.title
ORDER BY avg_score DESC;


-- =============================================================
--  EXTRA — Vista general del modelo completo (útil al inicio)
--  Muestra todas las relaciones en una sola consulta
-- =============================================================
SELECT
    g.title,
    g.release_year,
    ge.name                                         AS genre,
    COUNT(r.id)                                     AS total_reviews,
    ROUND(AVG(CAST(r.score AS FLOAT)), 1)           AS avg_score
FROM Game g
LEFT JOIN GameGenre gg ON gg.game_id  = g.id
LEFT JOIN Genre     ge ON ge.id       = gg.genre_id
LEFT JOIN Review    r  ON r.game_id   = g.id
GROUP BY g.id, g.title, g.release_year, ge.name
ORDER BY g.title, ge.name;
