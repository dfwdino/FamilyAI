CREATE TABLE [Chat].[ParentFlagRules] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [ParentUserId] INT            NOT NULL,
    [Type]         INT            NOT NULL,
    [Value]        NVARCHAR (200) NOT NULL,
    [Description]  NVARCHAR (500) NOT NULL,
    [GlobalRuleId] INT            NULL,
    [IsActive]     BIT            NOT NULL,
    [IsDeleted]    BIT            NOT NULL,
    CONSTRAINT [PK_ParentFlagRules] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ParentFlagRules_GlobalFlagRules_GlobalRuleId] FOREIGN KEY ([GlobalRuleId]) REFERENCES [Chat].[GlobalFlagRules] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_ParentFlagRules_Users_ParentUserId] FOREIGN KEY ([ParentUserId]) REFERENCES [Chat].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_ParentFlagRules_ParentUserId]
    ON [Chat].[ParentFlagRules]([ParentUserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ParentFlagRules_GlobalRuleId]
    ON [Chat].[ParentFlagRules]([GlobalRuleId] ASC);

