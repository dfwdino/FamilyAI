CREATE TABLE [Chat].[UserSettings] (
    [ID]               INT IDENTITY (1, 1) NOT NULL,
    [UserId]           INT NOT NULL,
    [PromptTemplateId] INT NOT NULL,
    [IsDeleted]        BIT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_UserSettings_Template] FOREIGN KEY ([PromptTemplateId]) REFERENCES [Chat].[PromptTemplates] ([ID]),
    CONSTRAINT [FK_UserSettings_User] FOREIGN KEY ([UserId]) REFERENCES [Chat].[Users] ([ID])
);

