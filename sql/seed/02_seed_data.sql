-- =============================================================
--  GameShelf — Script 02: Datos de ejemplo (seed)
--  CLASE 3: úsalo en DB Fiddle o SQL Server para las demos en vivo
--
--  Ejecutar DESPUÉS de 01_create_schema.sql
-- =============================================================

-- =============================================================
--  Géneros
-- =============================================================
INSERT INTO Genre (name) VALUES
    ('RPG'),
    ('Aventura'),
    ('Plataformas'),
    ('Estrategia');

-- =============================================================
--  Juegos
-- =============================================================
INSERT INTO Game (title, release_year, internal_score, admin_notes) VALUES
    ('Hollow Knight',  2017, 9.2, 'Top seller'),
    ('The Witcher 3',  2015, 9.5, 'GOTY 2015'),
    ('Celeste',        2018, 9.0, NULL),
    ('Hades',          2020, 9.3, NULL),
    ('Disco Elysium',  2019, 9.1, 'Niche audience');

-- =============================================================
--  Géneros por juego (tabla pivot)
--  CLASE 3: Hollow Knight tiene DOS géneros → por eso necesitamos N-M
-- =============================================================
INSERT INTO GameGenre (game_id, genre_id) VALUES
    (1, 3),   -- Hollow Knight  → Plataformas
    (1, 2),   -- Hollow Knight  → Aventura
    (2, 1),   -- The Witcher 3  → RPG
    (2, 2),   -- The Witcher 3  → Aventura
    (3, 3),   -- Celeste        → Plataformas
    (4, 1),   -- Hades          → RPG
    (4, 2),   -- Hades          → Aventura
    (5, 1);   -- Disco Elysium  → RPG

-- =============================================================
--  Reviews
-- =============================================================
INSERT INTO Review (game_id, score, comment, created_at) VALUES
    (1, 10, 'Obra maestra',           '2024-01-10'),
    (1,  9, 'Difícil pero épico',     '2024-01-15'),
    (1,  8, 'Muy bueno',              '2024-02-01'),
    (2, 10, 'Imprescindible',         '2024-01-20'),
    (2,  9, 'Historia increíble',     '2024-02-05'),
    (3,  9, 'Plataformas con alma',   '2024-03-01'),
    (4, 10, 'Roguelike perfecto',     '2024-03-10'),
    (4,  8, 'Muy adictivo',           '2024-03-15'),
    (5, 10, 'Único en su especie',    '2024-04-01'),
    (5,  7, 'Muy denso, pero genial', '2024-04-05');

PRINT 'Datos de ejemplo insertados correctamente.';
