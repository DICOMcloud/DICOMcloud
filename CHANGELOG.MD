# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Next Version

### Added
- Add support for "Deploy to Azure" Button from GitHub. Issue #80

### Fixed
- Fix: Improve storage performance by disabling pre-generated J2K DICOM files and JPG images by default. Issue #98
- Fix: Server fails to store when a DICOM file is invalid (valu doesn't confirm to VR) Issue #99
- Fix: maximum request size exceeded for STOWRS (Increase max request size to 100MB). Issue #95
- Fix: Storing a DICOM instance fails if a cached version (XML, JSON, other transfer syntax) fails to be stored. Issue #86
- Fix: Problem when handling dicomdata with non english characters. Issue #84 Thanks @nutzlastfan
- Fix: Problem with decimal conversion on non different culture settings (german) Issue #81 Thanks @nutzlastfan
- Fix: The "api" route for the Store service (STOW-RS) doesn't work without a Study Instance UID. Issue #79

## v0.7.1-beta

### Added
- Add Possibility to load individual series or instances with ohif standalone viewer. Issue #75. Thanks @nutzlastfan
- Add two unit tests for testing the DICOMweb services as if running from controller. Test for cases #67 and #70
- Add support for RESTful URLs with same URL base across QIDO-RS, WADO-RS and STOW-RS. Issue #59
- Add support for enabling CORS by configuration. Issue #57
- Add support for returning the "NumberOfFrames" in the OHIF viewer endpoint. Issue #49
- Update to fo-dicom v4.0.3 Fix Issue #33. Thanks @shelbypereira

### Changed
- Increase default upload limit from 4 MB to 10MB. Issue #51

### Fixed
- Fix generating STOW-RS results "Referened Instance" Sequence. Fix #67
- Fix parsing sequences in QIDO-RS include parameters. Fix Issue #70
- Fix convert to XML when element of type AT has no value
- Fix QIDO: UID list parsing is not according to standard. Issue #41
- Fix Wado-RS GetStudies not always working. Issue #55
- Fix WADO-RS retrieve frame is not working. Issue #48
- Fix Wildcard Matching not working. Issue #101

## v0.7.0-beta

### Added
- Add support for includefield value "all" in qido-rs. Issue #42
- Support additional attributes in QIDO-RS. Issue #34 [DATABASE BREAKING CHANGE]
  - ModalitiesInStudy
  - NumberOfStudyRelatedSeries
  - NumberOfStudyRelatedInstances 
  - NumberOfSeriesRelatedInstances * Rows
  - Columns
  - BitsAllocated 
  - NumberOfFrames
- Allow configuring the StoreCommand by passing "StorageSettings" instance from the application. Issue #40
- Add a configuration option to support Shared Access Signature/Self-Signed URLs. Issue #37
- Support generating JPG pre-cached on store. Issue #35

### Changed

- BREAKING CHANGE - Code Refactoring and Name changes. Issue #38 
- Update MS dependencies to latest version. Issue #36

### Fixed
- wrong sequence being returned in stow-rs/store response. Issue #43

## v0.6.0-beta

### Added
- Add unit test to check/validate results from paged query. Issue #25
- Add support to redirect WADO-URI requests to read directly from Azure Blob Storage. #24
- Add support for "offset and "limit" parameters in QIDO-RS. Issue #17
- Add support for attributeID "dicomKeyword" format in qido-rs. Issue #18
- Add validation to empty elements

### Changed
- Start using "changelog" over "change log" since it's the common usage.
- Breaking change - Support setting the content-type on Azure Blob storage. #23
- Ignore "*" as a requested transfer
- Add config settings when control storing of DICOM images (validate duplicate, store query model, store original)
- Read Query/Qido MaximumResultsLimit from configuration. Related to issue #17

### Fixed
- Fix: Pagination is broken when including query parameters in a query level lower than requested. Issue #25
- Fix: qido-rs returns an error when query URL has an empty query paramat the end. Issue #26
- Fix: Preserve error status message from multiple store failures
- Fix: Update DS transfer syntax after recompressing. Fix Issue #22
- Fix: validation for required elements
- Fix: Sending a date range query in qido-rs request returns an error #20
- Fix: Requesting a JPEG image in WADO-URL request doesn't return a valid jpeg image format #19
- Fix: Only single item is returned in a FailedSOPSequence #15…
- Fix: Only single item is returned in a ReferencedInstanceSequence  #16