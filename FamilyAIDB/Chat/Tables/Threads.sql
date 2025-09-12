CREATE TABLE [Chat].[Threads] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [ThreadName] NVARCHAR (100) NOT NULL,
    [IsDeleted]  BIT            CONSTRAINT [DF__Threads__IsDelet__44FF419A] DEFAULT ((0)) NOT NULL,
    [UserId]     INT            NOT NULL,
    CONSTRAINT [PK__Threads__3214EC270717178B] PRIMARY KEY CLUSTERED ([ID] ASC)
);

