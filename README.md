
***
# Announcement
This project is now moved from the original [repository](https://github.com/Zaid-Safadi/DICOMcloud) and is now maintained here in its own GitHub Orgnization. 
**The  project is now licensed/owned by the DICOMcloud project Contributors. .**
***

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/DICOMcloud.Wado.WebApi.svg)](https://www.nuget.org/packages/DICOMcloud.Wado.WebApi/)

# Overview 
The DICOMcloud is a standalone DICOMweb server with RESTful implementation of the DICOMweb/WADO services:
-	WADO-URI
-	QIDO-RS
-	WADO-RS
-	STOW-RS

The DICOMcloud is can run as a Web Application in Microsoft IIS or Microsoft Azure WebApp with no infrastructure to setup. It can be configured to use Azure Blob Storage and Azure SQL database for storing and querying DICOM Datasets. 
For complete features reference, read more **“DICOM Support”** section.

# Architecture:
The DICOMcloud is a web server that can interface with any DICOMweb client over the current implemented features (qido-rs, wado-uri, wado-rs and stow-rs).

An example DICOMweb client implementation with viewer support is provided [here](https://github.com/DICOMcloud/DICOMweb-js).

The implementation is customizable by using [StructureMap](https://github.com/structuremap/structuremap) as a DI (Dependency Injection) framework to provide a plug-in architecture.

The main layers of the DICOMcloud:

| Layer | Description | Project Name | Nuget Link |
| ------| ----------- | ------------ | -----------|
| **WebAPI RESTFUL Services** | The webservice implementation as an ASP.NET WebAPI | DICOMcloud.Wado.WebAPI | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/DICOMcloud.Wado.WebApi.svg)](https://www.nuget.org/packages/DICOMcloud.Wado.WebApi/) |
| **DICOMweb Core Services** | The DICOMweb implementation for processing web requests and returning web responses. | DICOMcloud.Wado | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/DICOMcloud.Wado.svg)](https://www.nuget.org/packages/DICOMcloud.Wado/) |
| **DICOM Services** | The core DICOM code and business services that process the DICOM datasets, perform query, retrieve and store. With interfaces to classes for storage and data access. | DICOMcloud | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/DICOMcloud.svg)](https://www.nuget.org/packages/DICOMcloud/) |
| **Data Storage and Data Access** | The specific implementation layer that physically save the DICOM dataset media to a file system or Azure Blob and interface with Microsoft/Azure SQL database. | DICOMcloud <br> DICOMcloud.Azure <br> DICOMcloud.DataAccess.Database | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/DICOMcloud.DataAccess.Database.svg)](https://www.nuget.org/packages/DICOMcloud.DataAccess.Database/) & [![NuGet Pre Release](https://img.shields.io/nuget/vpre/DICOMcloud.Azure.svg)](https://www.nuget.org/packages/DICOMcloud.Azure/) |

![DICOMcloud Architecture](https://github.com/DICOMcloud/DICOMcloud/blob/master/Resources/Docs/DICOMcloud-Arch..png)

# Platform:
The code is written in C# .NET Framework 4.5.2 and can be built using Visual Studio 2017 and can run on Windows machine or Azure WebApp.

The project uses MS SQL Database (Azure SQL Database compatabile) to query the DICOM information and saves the DICOM datasets to either the file system or an Azure Blob Storage.

# Running the code
You will need Visual Studio 2017/2015 [(can be downloaded for free here)](https://www.visualstudio.com/). Open the solution file **DICOMcloud.sln** on the root directory, if not already selected as the StartUp Project, right click on the **"DICOMcloud.Wado.WebApi"** project and select **"Set as startup project"** then run the solution by pressting **F5**. 

Once you run the project, the DICOMweb server will run on *https://localhost:44301/* and the default settings will attach an empty database to your local SQL DB server installed with Visual Studio **(LocalDb)\MSSQLLocalDB** and the images will be written to a directory under the **"App_Data"** folder. 

You can change these settings from the [web.config](https://github.com/DICOMcloud/DICOMcloud/blob/master/DICOMcloud.Wado.WebApi/Web.config) by updating the two values under the *appSettings* section:

     <add key="app:PacsStorageConnection" value="|DataDirectory|\App_Data\Storage\ds" />
     <add key="app:PacsDataArchieve" value="Data Source=(LocalDb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DB\DICOMcloud.mdf;Initial Catalog=DICOMcloud;Integrated Security=True" />  

# Online Version:
An online version is hosted in Azure and can be used for testing: [https://dicomcloud.azurewebsites.net/](https://dicomcloud.azurewebsites.net/)

A DICOMweb client demo is live at: [http://dicomweb.azurewebsites.net/](http://dicomweb.azurewebsites.net/)
The client demo source code is avaialbile here: [https://github.com/DICOMcloud/DICOMweb-js](https://github.com/DICOMcloud/DICOMweb-js)

# Endpoints
A detailed endpoints URLs with parameters can be viewed here:
[https://dicomcloud.azurewebsites.net/swagger/](https://dicomcloud.azurewebsites.net/swagger/)


# DICOM Support
The tables below describes the DICOMweb featrues currently implemented in the DICOMcloud project.

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


# Dependencies:
The DICOMcloud project utilizes the opensource [**fo-dicom**](https://github.com/fo-dicom/fo-dicom) DICOM library for operations on the DICOM datasets, such as reading and writing elements, compress/decompress the DICOM images, anonymization feature and many others.  

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
