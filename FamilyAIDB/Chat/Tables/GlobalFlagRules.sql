CREATE TABLE [Chat].[GlobalFlagRules] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Type]        INT            NOT NULL,
    [Value]       NVARCHAR (200) NOT NULL,
    [Description] NVARCHAR (500) NOT NULL,
    [IsDeleted]   BIT            NOT NULL,
    CONSTRAINT [PK_GlobalFlagRules] PRIMARY KEY CLUSTERED ([Id] ASC)
);

