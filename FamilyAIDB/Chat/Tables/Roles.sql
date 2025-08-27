CREATE TABLE [Chat].[Roles] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [RoleName]  NVARCHAR (100) NOT NULL,
    [IsDeleted] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

