CREATE TABLE [dbo].[Study] (
    [StudyKey]         BIGINT        IDENTITY (1, 1) NOT NULL,
    [Study_PatientKey] BIGINT        NOT NULL,
    [StudyInstanceUid] NVARCHAR (64) NOT NULL,
    [StudyId]          NVARCHAR (16) NULL,
    [AccessionNumber]  NVARCHAR (16) NULL,
    [StudyDate]        DATETIME      NULL,
    [StudyDescription] NVARCHAR (64) NULL,
    CONSTRAINT [PK_Study] PRIMARY KEY CLUSTERED ([StudyKey] ASC),
    CONSTRAINT [FK_Study_ToPatient] FOREIGN KEY ([Study_PatientKey]) REFERENCES [dbo].[Patient] ([PatientKey]) ON DELETE CASCADE ON UPDATE CASCADE
);


GO
