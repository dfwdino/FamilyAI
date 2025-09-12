CREATE TABLE [Chat].[ChatLogs] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [UserID]    INT            NOT NULL,
    [IsReply]   BIT            CONSTRAINT [DF__ChatLog__IsReply__3493CFA7] DEFAULT ((0)) NOT NULL,
    [Text]      NVARCHAR (MAX) NOT NULL,
    [EntryTime] DATETIME2 (7)  CONSTRAINT [DF__ChatLog__EntryTi__3587F3E0] DEFAULT (getdate()) NOT NULL,
    [IsDeleted] BIT            CONSTRAINT [DF__ChatLog__IsDelet__367C1819] DEFAULT ((0)) NOT NULL,
    [ThreadID]  INT            NOT NULL,
    CONSTRAINT [PK__ChatLog__3214EC27B7FD4400] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ChatLog_User] FOREIGN KEY ([UserID]) REFERENCES [Chat].[Users] ([ID])
);

