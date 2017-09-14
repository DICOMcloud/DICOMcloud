CREATE TABLE [dbo].[Series] (
    [SeriesKey]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [Series_StudyKey]   BIGINT        NOT NULL,
    [SeriesInstanceUid] NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [SeriesNumber]      INT           NULL,
    [Modality]          CHAR (2)      COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [SeriesDescription] NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
);
GO
ALTER TABLE [dbo].[Series]
    ADD CONSTRAINT [FK_Series_Study] FOREIGN KEY ([Series_StudyKey]) REFERENCES [dbo].[Study] ([StudyKey]) ON DELETE CASCADE ON UPDATE CASCADE;
GO
ALTER TABLE [dbo].[Series]
    ADD CONSTRAINT [PK_Series] PRIMARY KEY CLUSTERED ([SeriesKey] ASC);