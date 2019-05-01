-- ----------------------------------------------------------------------------
-- MySQL Workbench Migration
-- Migrated Schemata: DICOMcloud
-- Source Schemata: DICOMcloud
-- Created: Thu Dec 20 01:21:34 2018
-- Workbench Version: 8.0.13
-- ----------------------------------------------------------------------------

SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------------------------------------------------------
-- Schema DICOMcloud
-- ----------------------------------------------------------------------------
DROP SCHEMA IF EXISTS `DICOMcloud` ;
CREATE SCHEMA IF NOT EXISTS `DICOMcloud` ;

-- ----------------------------------------------------------------------------
-- Table DICOMcloud.DICOMcloudDbVersion
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `DICOMcloud`.`DICOMcloudDbVersion` (
  `Major` TINYINT UNSIGNED NOT NULL,
  `Minor` TINYINT UNSIGNED NOT NULL,
  PRIMARY KEY (`Major`, `Minor`));

-- ----------------------------------------------------------------------------
-- Table DICOMcloud.ObjectInstance
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `DICOMcloud`.`ObjectInstance` (
  `ObjectInstanceKey` BIGINT NOT NULL AUTO_INCREMENT,
  `ObjectInstance_SeriesKey` BIGINT NOT NULL,
  `SopInstanceUid` VARCHAR(64) CHARACTER SET 'utf8mb4' NOT NULL,
  `SopClassUid` VARCHAR(64) CHARACTER SET 'utf8mb4' NOT NULL,
  `InstanceNumber` INT NULL,
  `Metadata` LONGTEXT NULL,
  `Owner` VARCHAR(265) CHARACTER SET 'utf8mb4' NULL,
  `CreatedOn` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Rows` INT NULL,
  `Columns` INT NULL,
  `BitsAllocated` INT NULL,
  `NumberOfFrames` INT NULL,
  PRIMARY KEY (`ObjectInstanceKey`),
  CONSTRAINT `FK_ObjectInstance_ToSeries`
    FOREIGN KEY (`ObjectInstance_SeriesKey`)
    REFERENCES `DICOMcloud`.`Series` (`SeriesKey`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

-- ----------------------------------------------------------------------------
-- Table DICOMcloud.Patient
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `DICOMcloud`.`Patient` (
  `PatientKey` BIGINT NOT NULL AUTO_INCREMENT,
  `PatientId` VARCHAR(64) CHARACTER SET 'utf8mb4' NOT NULL,
  `IssuerOfPatientId` VARCHAR(64) CHARACTER SET 'utf8mb4' NULL,
  `PatientsBirthDate` DATETIME(6) NULL,
  `PatientsSex` CHAR(1) NULL,
  `PatientsName_Family` VARCHAR(64) CHARACTER SET 'utf8mb4' NULL,
  `PatientsName_Given` VARCHAR(64) CHARACTER SET 'utf8mb4' NULL,
  `PatientsName_Middle` VARCHAR(64) CHARACTER SET 'utf8mb4' NULL,
  `PatientsName_Prefix` VARCHAR(64) CHARACTER SET 'utf8mb4' NULL,
  `PatientsName_Suffix` VARCHAR(64) CHARACTER SET 'utf8mb4' NULL,
  `EthnicGroup` VARCHAR(16) CHARACTER SET 'utf8mb4' NULL,
  PRIMARY KEY (`PatientKey`));

-- ----------------------------------------------------------------------------
-- Table DICOMcloud.RequestAttributeSequence
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `DICOMcloud`.`RequestAttributeSequence` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `ScheduledProcedureStepID` VARCHAR(16) CHARACTER SET 'utf8mb4' NULL,
  `RequestedProcedureID` VARCHAR(16) CHARACTER SET 'utf8mb4' NULL,
  `RequestAttributeSeq_SeriesKey` BIGINT NOT NULL,
  PRIMARY KEY (`Id`),
  CONSTRAINT `FK_RequestAttributeSequence_Series`
    FOREIGN KEY (`RequestAttributeSeq_SeriesKey`)
    REFERENCES `DICOMcloud`.`Series` (`SeriesKey`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

-- ----------------------------------------------------------------------------
-- Table DICOMcloud.Series
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `DICOMcloud`.`Series` (
  `SeriesKey` BIGINT NOT NULL AUTO_INCREMENT,
  `Series_StudyKey` BIGINT NOT NULL,
  `SeriesInstanceUid` VARCHAR(64) CHARACTER SET 'utf8mb4' NOT NULL,
  `SeriesNumber` INT NULL,
  `Modality` CHAR(2) NOT NULL,
  `SeriesDescription` VARCHAR(64) CHARACTER SET 'utf8mb4' NULL,
  PRIMARY KEY (`SeriesKey`),
  CONSTRAINT `FK_Series_Study`
    FOREIGN KEY (`Series_StudyKey`)
    REFERENCES `DICOMcloud`.`Study` (`StudyKey`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

-- ----------------------------------------------------------------------------
-- Table DICOMcloud.Study
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `DICOMcloud`.`Study` (
  `StudyKey` BIGINT NOT NULL AUTO_INCREMENT,
  `Study_PatientKey` BIGINT NOT NULL,
  `StudyInstanceUid` VARCHAR(64) CHARACTER SET 'utf8mb4' NOT NULL,
  `StudyId` VARCHAR(16) CHARACTER SET 'utf8mb4' NULL,
  `AccessionNumber` VARCHAR(16) CHARACTER SET 'utf8mb4' NULL,
  `StudyDate` DATETIME(6) NULL,
  `StudyDescription` VARCHAR(64) CHARACTER SET 'utf8mb4' NULL,
  PRIMARY KEY (`StudyKey`),
  CONSTRAINT `FK_Study_ToPatient`
    FOREIGN KEY (`Study_PatientKey`)
    REFERENCES `DICOMcloud`.`Patient` (`PatientKey`)
    ON DELETE CASCADE
    ON UPDATE CASCADE);

-- ----------------------------------------------------------------------------
-- Table DICOMcloud.__RefactorLog
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `DICOMcloud`.`__RefactorLog` (
  `OperationKey` VARCHAR(64) UNIQUE NOT NULL,
  PRIMARY KEY (`OperationKey`));
SET FOREIGN_KEY_CHECKS = 1;
