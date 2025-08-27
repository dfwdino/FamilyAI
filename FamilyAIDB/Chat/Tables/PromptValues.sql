CREATE TABLE [Chat].[PromptValues] (
    [ID]         INT           IDENTITY (1, 1) NOT NULL,
    [PromptID]   INT           NOT NULL,
    [PrompValue] VARCHAR (255) NOT NULL,
    [IsDeleted]  BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PromptValues_Prompt] FOREIGN KEY ([PromptID]) REFERENCES [Chat].[Prompts] ([ID])
);

