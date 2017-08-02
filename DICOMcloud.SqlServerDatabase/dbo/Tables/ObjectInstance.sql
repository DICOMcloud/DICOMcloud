CREATE TABLE [dbo].[ObjectInstance] (
    [ObjectInstanceKey]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [ObjectInstance_SeriesKey] BIGINT         NOT NULL,
    [SopInstanceUid]           NVARCHAR (64)  COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [SopClassUid]              NVARCHAR (64)  COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [InstanceNumber]           INT            NULL,
    [Metadata]                 TEXT           COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [Owner]                    NVARCHAR (265) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
);
GO
ALTER TABLE [dbo].[ObjectInstance]
    ADD CONSTRAINT [PK_ObjectInstance] PRIMARY KEY CLUSTERED ([ObjectInstanceKey] ASC);