CREATE TABLE [Chat].[Users] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Username]  NVARCHAR (50)  NOT NULL,
    [Password]  NVARCHAR (255) NOT NULL,
    [IsDeleted] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    UNIQUE NONCLUSTERED ([Username] ASC)
);

