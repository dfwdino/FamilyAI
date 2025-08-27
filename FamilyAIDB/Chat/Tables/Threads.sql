CREATE TABLE [Chat].[Threads] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [ThreadName] NVARCHAR (100) NOT NULL,
    [IsDeleted]  BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

