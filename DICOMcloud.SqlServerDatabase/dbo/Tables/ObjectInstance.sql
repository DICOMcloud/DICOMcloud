CREATE TABLE [dbo].[ObjectInstance] (
    [ObjectInstanceKey]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [ObjectInstance_SeriesKey] BIGINT         NOT NULL,
    [SopInstanceUid]           NVARCHAR (64)  NOT NULL,
    [SopClassUid]              NVARCHAR (64)  NOT NULL,
    [InstanceNumber]           INT            NULL,
    [Metadata]                 TEXT           NULL,
    [Owner]                    NVARCHAR (265) NULL,
    [CreatedOn]                DATETIME       DEFAULT (getdate()) NOT NULL,
    [Rows] INT NULL, 
    [Columns] INT NULL, 
    [BitsAllocated] INT NULL, 
    [NumberOfFrames] INT NULL, 
    CONSTRAINT [PK_ObjectInstance] PRIMARY KEY CLUSTERED ([ObjectInstanceKey] ASC),
    CONSTRAINT [FK_ObjectInstance_ToSeries] FOREIGN KEY ([ObjectInstance_SeriesKey]) REFERENCES [dbo].[Series] ([SeriesKey]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
