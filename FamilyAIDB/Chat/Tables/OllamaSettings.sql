CREATE TABLE [Chat].[OllamaSettings] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (100) NOT NULL,
    [ModelUrl]  NVARCHAR (500) NOT NULL,
    [ModelName] NVARCHAR (100) NOT NULL,
    [IsActive]  BIT            DEFAULT ((0)) NOT NULL,
    [IsDeleted] BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_OllamaSettings] PRIMARY KEY CLUSTERED ([ID] ASC)
);

