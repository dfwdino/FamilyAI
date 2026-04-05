-- ============================================================
-- Migration: Add OllamaSettings table
-- Stores Ollama server URL and model configurations.
-- Seeds one default entry pointing to localhost.
-- Run once against your FamilyAI database
-- ============================================================

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'Chat' AND TABLE_NAME = 'OllamaSettings'
)
BEGIN
    CREATE TABLE [Chat].[OllamaSettings] (
        [ID]        INT           IDENTITY(1,1) NOT NULL,
        [Name]      NVARCHAR(100) NOT NULL,
        [ModelUrl]  NVARCHAR(500) NOT NULL,
        [ModelName] NVARCHAR(100) NOT NULL,
        [IsActive]  BIT           NOT NULL DEFAULT (0),
        [IsDeleted] BIT           NOT NULL DEFAULT (0),
        CONSTRAINT [PK_OllamaSettings] PRIMARY KEY CLUSTERED ([ID] ASC)
    );

    -- Seed the default local Ollama configuration
    INSERT INTO [Chat].[OllamaSettings] ([Name], [ModelUrl], [ModelName], [IsActive], [IsDeleted])
    VALUES ('Local Ollama', 'http://localhost:11434/', 'llama3.2-vision', 1, 0);

    PRINT 'OllamaSettings table created and seeded.';
END
ELSE
    PRINT 'OllamaSettings table already exists.';
