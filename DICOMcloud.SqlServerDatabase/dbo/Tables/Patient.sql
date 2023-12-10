CREATE TABLE [dbo].[Patient] (
    [PatientKey]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [PatientId]           NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    [IssuerOfPatientId]   NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PatientsBirthDate]   DATETIME      NULL,
    [PatientsSex]         CHAR (1)      COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PatientsName_Family] NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PatientsName_Given]  NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PatientsName_Middle] NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PatientsName_Prefix] NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [PatientsName_Suffix] NVARCHAR (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
    [EthnicGroup]         NVARCHAR (16) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
);
GO
ALTER TABLE [dbo].[Patient]
    ADD CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED ([PatientKey] ASC);