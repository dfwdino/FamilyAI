-- ============================================================
-- Migration: Add ParentChildren table and rename 'User' role to 'Parent'
-- Run this against your FamilyAI database
-- ============================================================

-- 1. Rename the 'User' role to 'Parent'
UPDATE [Chat].[Roles]
SET    [RoleName] = 'Parent'
WHERE  [RoleName] = 'User';

-- 2. Create the ParentChildren join table
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'Chat' AND TABLE_NAME = 'ParentChildren'
)
BEGIN
    CREATE TABLE [Chat].[ParentChildren] (
        [ID]        INT  IDENTITY (1, 1) NOT NULL,
        [ParentId]  INT  NOT NULL,
        [ChildId]   INT  NOT NULL,
        [IsDeleted] BIT  DEFAULT ((0)) NOT NULL,
        CONSTRAINT [PK_ParentChildren] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_ParentChildren_Parent] FOREIGN KEY ([ParentId]) REFERENCES [Chat].[Users] ([ID]),
        CONSTRAINT [FK_ParentChildren_Child]  FOREIGN KEY ([ChildId])  REFERENCES [Chat].[Users] ([ID])
    );
END
