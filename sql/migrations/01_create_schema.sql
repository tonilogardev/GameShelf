-- =============================================================
--  GameShelf — Script 01: Creación del esquema
--  CLASE 3: Modelado relacional
--
--  Ejecutar primero. Crea las tablas en el orden correcto
--  para respetar las FK (primero las tablas "padre").
-- =============================================================

-- Limpieza (orden inverso de FK para no violar restricciones)
IF OBJECT_ID('GameGenre', 'U') IS NOT NULL DROP TABLE GameGenre;
IF OBJECT_ID('Review',    'U') IS NOT NULL DROP TABLE Review;
IF OBJECT_ID('Game',      'U') IS NOT NULL DROP TABLE Game;
IF OBJECT_ID('Genre',     'U') IS NOT NULL DROP TABLE Genre;

-- =============================================================
--  Genre  (tabla padre)
-- =============================================================
CREATE TABLE Genre (
    id   INT          IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL
);

-- =============================================================
--  Game  (referencia Genre via FK)
-- =============================================================
CREATE TABLE Game (
    id             INT           IDENTITY(1,1) PRIMARY KEY,
    title          NVARCHAR(150) NOT NULL,
    release_year   INT           NOT NULL,

    -- Campos internos — solo para uso administrativo
    -- CLASE 4: estos campos NO aparecen en el GameDto
    internal_score DECIMAL(5,2)  NOT NULL DEFAULT 0,
    admin_notes    NVARCHAR(500) NULL
);

-- =============================================================
--  GameGenre  — tabla pivot N-M entre Game y Genre
--  CLASE 3: "La tabla pivot no tiene lógica propia. Solo conecta."
-- =============================================================
CREATE TABLE GameGenre (
    game_id  INT NOT NULL REFERENCES Game(id)  ON DELETE CASCADE,
    genre_id INT NOT NULL REFERENCES Genre(id) ON DELETE CASCADE,
    CONSTRAINT PK_GameGenre PRIMARY KEY (game_id, genre_id)
);

-- =============================================================
--  Review  — N reviews por 1 juego
--  CLASE 3: esta tabla permite AVG(score), COUNT, GROUP BY
-- =============================================================
CREATE TABLE Review (
    id         INT           IDENTITY(1,1) PRIMARY KEY,
    game_id    INT           NOT NULL REFERENCES Game(id) ON DELETE CASCADE,
    score      INT           NOT NULL CHECK (score BETWEEN 1 AND 10),
    comment    NVARCHAR(500) NULL,
    created_at DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);

PRINT 'Esquema creado correctamente.';
