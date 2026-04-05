CREATE TABLE [Chat].[PromptTemplates] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (100) NOT NULL,
    [Content]   NVARCHAR (MAX) NOT NULL,
    [IsDefault] BIT            DEFAULT ((0)) NOT NULL,
    [IsDeleted] BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_PromptTemplates] PRIMARY KEY CLUSTERED ([ID] ASC)
);

