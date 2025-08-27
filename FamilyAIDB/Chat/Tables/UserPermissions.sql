CREATE TABLE [Chat].[UserPermissions] (
    [ID]        INT IDENTITY (1, 1) NOT NULL,
    [UserID]    INT NOT NULL,
    [RoleID]    INT NOT NULL,
    [IsDeleted] BIT DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_UserPermissions_Roles] FOREIGN KEY ([RoleID]) REFERENCES [Chat].[Roles] ([ID]),
    CONSTRAINT [FK_UserPermissions_User] FOREIGN KEY ([UserID]) REFERENCES [Chat].[Users] ([ID])
);

