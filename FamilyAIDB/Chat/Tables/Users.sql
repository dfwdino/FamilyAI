CREATE TABLE [Chat].[Users] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Username]  NVARCHAR (50)  NOT NULL,
    [Password]  NVARCHAR (255) NOT NULL,
    [IsDeleted] BIT            CONSTRAINT [DF__Users__IsDeleted__3D5E1FD2] DEFAULT ((0)) NOT NULL,
    [Name]      VARCHAR (50)   NOT NULL,
    CONSTRAINT [PK__Users__3214EC27B80C5BF3] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ__Users__536C85E4B066822E] UNIQUE NONCLUSTERED ([Username] ASC)
);

