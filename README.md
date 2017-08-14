***
# Announcement
This project is now moved from the original [repository](https://github.com/Zaid-Safadi/DICOMcloud) and is now maintained here in its own GitHub Orgnization. 
**The  project is now licensed/owned by the DICOMcloud project Contributors. .**
***

# Overview 
The DICOMcloud is a standalone DICOMweb server with RESTful implementation of the DICOMweb/WADO services:
-	WADO-URI
-	QIDO-RS
-	WADO-RS
-	STOW-RS

The DICOMcloud is can run as a Web Application in Microsoft IIS or Microsoft Azure WebApp with no infrastructure to setup. It can be configured to use Azure Blob Storage and Azure SQL database for storing and querying DICOM Datasets. 
For complete features reference, read more **“DICOM Support”** section.

# Architecture:
The DICOMcloud is a web server that can interface with any DICOMweb client over the current implemented features (qido-rs, wado-uri, wado-rs and stow-rs). An example DICOMweb client is implemented [here](https://github.com/Zaid-Safadi/dicom-webJS).

The implementation is customizable by using [StructureMap](https://github.com/structuremap/structuremap) as a DI (Dependency Injection) framework to provide a plug-in architecture.

The main layers of the DICOMcloud:

1.	**WebAPI RESTFUL service**: The webservice implementation as an ASP.NET WebAPI project “DICOMcloud.Wado.WebAPI” 

2. **DICOMweb Services**: The DICOMweb implementation for processing web requests and returning web responses. Implemented in the “DICOMcloud.Wado” project.

3.	**DICOM Services**: The core DICOM code and business services that process the DICOM datasets, perform query, retrieve and store. With interfaces to classes for storage and data access. This is implemented in the “DICOMcloud” project.

4. **Data Storage and Data Access**: The specific implementation layer that physically save the DICOM dataset media to a file system or Azure Blob and interface with Microsoft/Azure SQL database. This is implemented in the “DICOMcloud”, “DICOMcloud.Azure” and “DICOMcloud.DataAccess.Database” projects.

# Platform:
The code is written in C# .NET Framework 4.5.2 and can be built using Visual Studio 2017 and can run on Windows machine or Azure WebApp.

# Dependencies:
The DICOMcloud project utilizes the opensource [**fo-dicom**](https://github.com/fo-dicom/fo-dicom) DICOM library for operations on the DICOM datasets, such as reading and writing elements, compress/decompress the DICOM images, anonymization feature and many others.  

# DICOM Support
A detailed endpoints URLs with parameters can be viewed here:
[https://dicomcloud.azurewebsites.net/swagger/](https://dicomcloud.azurewebsites.net/swagger/)

### QIDO-RS
|Feature   |  Support | Notes  |
|----------|----------|-------|
| application/dicom+xml  | Y  |  |
| application/json       | Y  | |
| Studies                | Y  |
| Series                 | Y  |
| Instances              | Y  |
| relational query       | ~  |
| fuzzy matching| Y | Always supported |
| ranges | Y | |  |
| includefield | Y | |
| sequences | Y | |
| limit | N | |
| offset | N | |
| dicomKeyword group element | N | |
| dicomKeyword name | N | |
| TimezoneOffsetFromUTC | N | |

### WADO-RS
| Feature | Support | Notes |
|---------|---------|-------|
| application/dicom+xml | Y | |
| application/json | Y | |
| transfer-syntax | Y | |
| Retrieve Study | Y | |
| Retrieve Series | Y | |
| Retrieve Instance | Y | |
| Retreive Frames | Y | |
| Retrieve Bulkdata | Y | header is missing Content-Location: {BulkDataURI} |
| Retrieve Metadata | Y | |

### STOW-RS
The server can be configured to anonymize the DICOM image by default by enabling the feature in the web.config:

     <add key="app:enableAnonymizer" value="true"/>
     <add key="app:anonymizerOptions" value="BasicProfile,RetainUIDs,RetainLongFullDates,RetainPatientChars"/>

| Feature | Support | Notes |
|---------|---------|-------|
| application/dicom | Y | |
| application/dicom+xml | Y | |
| application/dicom+json | Y | |
| Multipart store | Y | can process multiple instances in single request

## WADO-URI
| Feature | Support | Notes |
|---------|---------|-------|
| application/dicom | Y | |
| Frame Number | Y | |
| Charset | N | |
| Anonymize | N | |
| Transfer Syntax | Y |  |
| Charset | N | |
| Annotation | N | |
| Rows | N | |
| Columns | N |  |
| Region | N |  |
| Windows Center | N |  |
| Window Width | N |  |
| Image Quality | N | |
| Presentation UID | N | |
| Presentation Series UID | N | |


# Online Version:
An online version that is hosted in Azure is live at: [https://dicomcloud.azurewebsites.net/](https://dicomcloud.azurewebsites.net/)

The DICOMweb client demo is live at: [http://dicomweb.azurewebsites.net/](http://dicomweb.azurewebsites.net/)

# License
  
    Copyright 2017 DICOMcloud Contributors
    
    Licensed under the Apache License, Version 2.0 (the "License"); 
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at 
        
        http://www.apache.org/licenses/LICENSE-2.0
        
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
