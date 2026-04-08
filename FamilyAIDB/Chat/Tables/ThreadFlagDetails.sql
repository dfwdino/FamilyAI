CREATE TABLE [Chat].[ThreadFlagDetails] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [ThreadScanResultId] INT            NOT NULL,
    [MessageId]          INT            NULL,
    [RuleType]           INT            NOT NULL,
    [RuleValue]          NVARCHAR (200) NOT NULL,
    [MatchedExcerpt]     NVARCHAR (500) NULL,
    [Source]             INT            NOT NULL,
    [IsDeleted]          BIT            NOT NULL,
    CONSTRAINT [PK_ThreadFlagDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ThreadFlagDetails_ChatLogs_MessageId] FOREIGN KEY ([MessageId]) REFERENCES [Chat].[ChatLogs] ([ID]) ON DELETE SET NULL,
    CONSTRAINT [FK_ThreadFlagDetails_ThreadScanResults_ThreadScanResultId] FOREIGN KEY ([ThreadScanResultId]) REFERENCES [Chat].[ThreadScanResults] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ThreadFlagDetails_ThreadScanResultId]
    ON [Chat].[ThreadFlagDetails]([ThreadScanResultId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ThreadFlagDetails_MessageId]
    ON [Chat].[ThreadFlagDetails]([MessageId] ASC);

