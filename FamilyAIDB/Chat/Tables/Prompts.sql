CREATE TABLE [Chat].[Prompts] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [PromptTitle] NVARCHAR (100) NOT NULL,
    [IsDeleted]   BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

