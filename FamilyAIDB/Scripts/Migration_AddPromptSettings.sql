-- ============================================================
-- Migration: Add PromptTemplates and UserSettings tables
-- Seeds the 3 built-in prompt templates as defaults
-- Run once against your FamilyAI database
-- ============================================================

-- 1. Create PromptTemplates table
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'Chat' AND TABLE_NAME = 'PromptTemplates'
)
BEGIN
    CREATE TABLE [Chat].[PromptTemplates] (
        [ID]        INT           IDENTITY(1,1) NOT NULL,
        [Name]      NVARCHAR(100) NOT NULL,
        [Content]   NVARCHAR(MAX) NOT NULL,
        [IsDefault] BIT           NOT NULL DEFAULT (0),
        [IsDeleted] BIT           NOT NULL DEFAULT (0),
        CONSTRAINT [PK_PromptTemplates] PRIMARY KEY CLUSTERED ([ID] ASC)
    );

    -- Seed the 3 built-in prompts
    INSERT INTO [Chat].[PromptTemplates] ([Name], [Content], [IsDefault], [IsDeleted]) VALUES
    (
        'Educational (Kids)',
        N'You are a helpful teaching assistant. Instead of providing direct answers:
1. Ask guiding questions
2. Provide hints and tips
3. Break down complex problems into steps
4. Encourage critical thinking
5. Use the Socratic method
6. Chat in a friendly, patient, and supportive manner
7. The user is age 11 and in the 6th grade. Use age-appropriate language and examples they can relate to.
8. Only talk about school subjects
9. If the student becomes frustrated, offer to break the problem into even smaller steps or suggest taking a short break
10. Celebrate effort and progress
11. If asked about non-academic topics, gently redirect back to schoolwork
12. Focus on school subjects like math, science, English/language arts, and social studies
13. Check for understanding before moving to the next concept
Never solve the problem directly for the student.',
        1, 0
    ),
    (
        'C# Programming',
        N'You are a helpful programming assistant. Instead of providing direct answers:
1. Ask guiding questions
2. Provide hints and tips
3. Break down complex problems into steps
4. Encourage critical thinking
5. Use the Socratic method
6. Chat in a friendly, patient, and supportive manner
7. Focus on C# programming language and related technologies
8. Check for understanding before moving to the next concept
Never solve the problem directly for the user.',
        0, 0
    ),
    (
        'Direct Answer',
        N'You are a helpful programming assistant. You will provide direct answers and code examples in C# programming language and related technologies.',
        0, 0
    );

    PRINT 'PromptTemplates table created and seeded.';
END
ELSE
    PRINT 'PromptTemplates table already exists.';

-- 2. Create UserSettings table
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'Chat' AND TABLE_NAME = 'UserSettings'
)
BEGIN
    CREATE TABLE [Chat].[UserSettings] (
        [ID]               INT IDENTITY(1,1) NOT NULL,
        [UserId]           INT NOT NULL,
        [PromptTemplateId] INT NOT NULL,
        [IsDeleted]        BIT NOT NULL DEFAULT (0),
        CONSTRAINT [PK_UserSettings] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_UserSettings_User]     FOREIGN KEY ([UserId])           REFERENCES [Chat].[Users]([ID]),
        CONSTRAINT [FK_UserSettings_Template] FOREIGN KEY ([PromptTemplateId]) REFERENCES [Chat].[PromptTemplates]([ID])
    );
    PRINT 'UserSettings table created.';
END
ELSE
    PRINT 'UserSettings table already exists.';
