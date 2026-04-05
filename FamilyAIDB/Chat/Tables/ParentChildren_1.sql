CREATE TABLE [Chat].[ParentChildren] (
    [ID]        INT IDENTITY (1, 1) NOT NULL,
    [ParentId]  INT NOT NULL,
    [ChildId]   INT NOT NULL,
    [IsDeleted] BIT DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ParentChildren] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ParentChildren_Child] FOREIGN KEY ([ChildId]) REFERENCES [Chat].[Users] ([ID]),
    CONSTRAINT [FK_ParentChildren_Parent] FOREIGN KEY ([ParentId]) REFERENCES [Chat].[Users] ([ID])
);

