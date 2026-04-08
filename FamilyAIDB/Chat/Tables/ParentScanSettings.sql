CREATE TABLE [Chat].[ParentScanSettings] (
    [Id]           INT IDENTITY (1, 1) NOT NULL,
    [ParentUserId] INT NOT NULL,
    [Sensitivity]  INT NOT NULL,
    [IsDeleted]    BIT NOT NULL,
    CONSTRAINT [PK_ParentScanSettings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ParentScanSettings_Users_ParentUserId] FOREIGN KEY ([ParentUserId]) REFERENCES [Chat].[Users] ([ID])
);


GO
CREATE NONCLUSTERED INDEX [IX_ParentScanSettings_ParentUserId]
    ON [Chat].[ParentScanSettings]([ParentUserId] ASC);

