CREATE TABLE [dbo].[RequestAttributeSequence] (
    [Id]                            INT           IDENTITY (1, 1) NOT NULL,
    [ScheduledProcedureStepID]      NVARCHAR (16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [RequestedProcedureID]          NVARCHAR (16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [RequestAttributeSeq_SeriesKey] BIGINT        NOT NULL
);
GO
ALTER TABLE [dbo].[RequestAttributeSequence]
    ADD CONSTRAINT [FK_RequestAttributeSequence_Series] FOREIGN KEY ([RequestAttributeSeq_SeriesKey]) REFERENCES [dbo].[Series] ([SeriesKey]) ON DELETE CASCADE ON UPDATE CASCADE;
GO
ALTER TABLE [dbo].[RequestAttributeSequence]
    ADD CONSTRAINT [PK_RequestAttributeSequence] PRIMARY KEY CLUSTERED ([Id] ASC);