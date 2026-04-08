CREATE TABLE [Chat].[ThreadScanResults] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [ThreadId]          INT            NOT NULL,
    [ScannedByParentId] INT            NOT NULL,
    [ScannedAt]         DATETIME2 (7)  NOT NULL,
    [IsFlagged]         BIT            NOT NULL,
    [AiSummary]         NVARCHAR (MAX) NULL,
    [RuleSetHash]       NVARCHAR (64)  NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    CONSTRAINT [PK_ThreadScanResults] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ThreadScanResults_Threads_ThreadId] FOREIGN KEY ([ThreadId]) REFERENCES [Chat].[Threads] ([ID]),
    CONSTRAINT [FK_ThreadScanResults_Users_ScannedByParentId] FOREIGN KEY ([ScannedByParentId]) REFERENCES [Chat].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_ThreadScanResults_ThreadId]
    ON [Chat].[ThreadScanResults]([ThreadId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ThreadScanResults_ScannedByParentId]
    ON [Chat].[ThreadScanResults]([ScannedByParentId] ASC);

