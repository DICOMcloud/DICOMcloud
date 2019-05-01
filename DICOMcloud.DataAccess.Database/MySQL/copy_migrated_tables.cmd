REM Workbench Table Data copy script
REM Workbench Version: 8.0.13
REM 
REM Execute this to copy table data from a source RDBMS to MySQL.
REM Edit the options below to customize it. You will need to provide passwords, at least.
REM 
REM Source DB: Mssql@DRIVER=SQL Server Native Client 11.0;SERVER=(localdb)\MSSQLLocalDB (Microsoft SQL Server)
REM Target DB: Mysql@localhost:3306


@ECHO OFF
REM Source and target DB passwords
set arg_source_password=
set arg_target_password=
set arg_source_ssh_password=
set arg_target_ssh_password=


REM Set the location for wbcopytables.exe in this variable
set "wbcopytables_path=C:\Program Files\MySQL\MySQL Workbench 8.0 CE"

if not ["%wbcopytables_path%"] == [] set "wbcopytables_path=%wbcopytables_path%"set "wbcopytables=%wbcopytables_path%wbcopytables.exe"

if not exist "%wbcopytables%" (
	echo "wbcopytables.exe doesn't exist in the supplied path. Please set 'wbcopytables_path' with the proper path(e.g. to Workbench binaries)"
	exit 1
)

IF [%arg_source_password%] == [] (
    IF [%arg_target_password%] == [] (
        IF [%arg_source_ssh_password%] == [] (
            IF [%arg_target_ssh_password%] == [] (
                ECHO WARNING: All source and target passwords are empty. You should edit this file to set them.
            )
        )
    )
)
set arg_worker_count=2
REM Uncomment the following options according to your needs

REM Whether target tables should be truncated before copy
REM set arg_truncate_target=--truncate-target
REM Enable debugging output
REM set arg_debug_output=--log-level=debug3


REM Creation of file with table definitions for copytable

set table_file=%TMP%\wb_tables_to_migrate.txt
TYPE NUL > %TMP%\wb_tables_to_migrate.txt
ECHO [DICOMcloud]	[dbo].[Study]	`DICOMcloud`	`Study`	[StudyKey]	`StudyKey`	[StudyKey], [Study_PatientKey], [StudyInstanceUid], [StudyId], [AccessionNumber], [StudyDate], [StudyDescription] >> %TMP%\wb_tables_to_migrate.txt
ECHO [DICOMcloud]	[dbo].[Patient]	`DICOMcloud`	`Patient`	[PatientKey]	`PatientKey`	[PatientKey], [PatientId], [IssuerOfPatientId], [PatientsBirthDate], CAST([PatientsSex] as NCHAR(1)) as [PatientsSex], [PatientsName_Family], [PatientsName_Given], [PatientsName_Middle], [PatientsName_Prefix], [PatientsName_Suffix], [EthnicGroup] >> %TMP%\wb_tables_to_migrate.txt
ECHO [DICOMcloud]	[dbo].[ObjectInstance]	`DICOMcloud`	`ObjectInstance`	[ObjectInstanceKey]	`ObjectInstanceKey`	[ObjectInstanceKey], [ObjectInstance_SeriesKey], [SopInstanceUid], [SopClassUid], [InstanceNumber], CAST([Metadata] as NTEXT) as [Metadata], [Owner], [CreatedOn], [Rows], [Columns], [BitsAllocated], [NumberOfFrames] >> %TMP%\wb_tables_to_migrate.txt
ECHO [DICOMcloud]	[dbo].[Series]	`DICOMcloud`	`Series`	[SeriesKey]	`SeriesKey`	[SeriesKey], [Series_StudyKey], [SeriesInstanceUid], [SeriesNumber], CAST([Modality] as NCHAR(2)) as [Modality], [SeriesDescription] >> %TMP%\wb_tables_to_migrate.txt
ECHO [DICOMcloud]	[dbo].[RequestAttributeSequence]	`DICOMcloud`	`RequestAttributeSequence`	[Id]	`Id`	[Id], [ScheduledProcedureStepID], [RequestedProcedureID], [RequestAttributeSeq_SeriesKey] >> %TMP%\wb_tables_to_migrate.txt
ECHO [DICOMcloud]	[dbo].[__RefactorLog]	`DICOMcloud`	`__RefactorLog`	[OperationKey]	`OperationKey`	CAST([OperationKey] as VARCHAR(64)) as [OperationKey] >> %TMP%\wb_tables_to_migrate.txt
ECHO [DICOMcloud]	[dbo].[DICOMcloudDbVersion]	`DICOMcloud`	`DICOMcloudDbVersion`	[Major],[Minor]	`Major`,`Minor`	[Major], [Minor] >> %TMP%\wb_tables_to_migrate.txt


"%wbcopytables%" ^
 --odbc-source="DRIVER={SQL Server Native Client 11.0};SERVER=(localdb)\MSSQLLocalDB;DATABASE={dicomcloud};UID=sa" ^
 --source-rdbms-type=Mssql ^
 --target="root@localhost:3306" ^
 --source-password="%arg_source_password%" ^
 --target-password="%arg_target_password%" ^
 --table-file="%table_file%" ^
 --target-ssh-port="22" ^
 --target-ssh-host="" ^
 --target-ssh-user="" ^
 --source-ssh-password="%arg_source_ssh_password%" ^
 --target-ssh-password="%arg_target_ssh_password%" --thread-count=%arg_worker_count% ^
 %arg_truncate_target% ^
 %arg_debug_output%

REM Removes the file with the table definitions
DEL %TMP%\wb_tables_to_migrate.txt


