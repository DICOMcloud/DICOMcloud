CREATE TABLE [dbo].[Study] (
    [StudyKey]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [Study_PatientKey] BIGINT        NOT NULL,
    [StudyInstanceUid] NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [StudyId]          NVARCHAR (16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [AccessionNumber]  NVARCHAR (16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [StudyDate]        DATETIME      NULL, 
    [StudyDescription] NVARCHAR(64) NULL
);
GO
ALTER TABLE [dbo].[Study]
    ADD CONSTRAINT [PK_Study] PRIMARY KEY CLUSTERED ([StudyKey] ASC);