[![Join the chat at https://gitter.im/DICOMcloud/Lobby](https://badges.gitter.im/DICOMcloud/Lobby.svg)](https://gitter.im/DICOMcloud/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/DICOMcloud.Wado.WebApi.svg)](https://www.nuget.org/packages/DICOMcloud.Wado.WebApi/)
[![Build status](https://ci.appveyor.com/api/projects/status/4587kyoq6gi94vqo/branch/development?svg=true)](https://ci.appveyor.com/project/Zaid-Safadi/dicomcloud-bfpvk/branch/development)
[![Build Status](https://dev.azure.com/zaidsafadi/DICOMcloud/_apis/build/status/DICOMcloud)](https://dev.azure.com/zaidsafadi/DICOMcloud/_build/latest?definitionId=8)

# Deploy to Azure
**Development Branch:** <a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FDICOMcloud%2FDICOMcloud%2Fdevelopment%2Fazuredeploy.json">
<img src="https://camo.githubusercontent.com/9285dd3998997a0835869065bb15e5d500475034/687474703a2f2f617a7572656465706c6f792e6e65742f6465706c6f79627574746f6e2e706e67" data-canonical-src="http://azuredeploy.net/deploybutton.png" style="max-width:100%;">
</a> 

You can see the details of this [Azure Resource Manager (ARM) Template here](https://github.com/DICOMcloud/azure-deploy/tree/master/dicomcloud-server)

# Overview
The DICOMcloud is a standalone DICOMweb server with RESTful implementation of the DICOMweb/WADO services:

* **QIDO-RS:** Look up studies, series, images. 
Results will be limited to a maximum results. [more info](https://dicomcloud.github.io/docs/dicomcloud/pagination/)

* **WADO-RS:** Retrieve studies, series, images, frames and metadata

* **STOW-RS:** Store DICOM instances/images

* **WADO-URI:** Web Access to DICOM objects

Additionally, the server implements the following RESTful services which are not part of the DICOM standard:

* **DELOW-RS:** Delete DICOM instances/images

* **OHIF-Viewer:** Integration service with the OHIF viewer, return OHIF formatted study information (series and instances)
[Click here to learn more](https://dicomcloud.github.io/docs/ohif-integration/) about using the DICOMcloud server and the OHIF Viewer. 

&nbsp;

The DICOMcloud server can interface with any DICOMweb client over the current implemented features (qido-rs, wado-uri, wado-rs and stow-rs).

### Official documentation and examples can be found here: 
https://dicomcloud.github.io/docs/dicomcloud/about/


# Online Version:

An online version is hosted in Azure: [https://dicomcloud.azurewebsites.net/](https://dicomcloud.azurewebsites.net/)

A DICOMweb Client demo is hosted live at: [http://dicomweb.azurewebsites.net/](http://dicomweb.azurewebsites.net/)

The Client demo source code is avaialbile here: [https://github.com/DICOMcloud/DICOMweb-js](https://github.com/DICOMcloud/DICOMweb-js)

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
| limit | Y | |
| offset | Y | |
| dicomKeyword group element | N | |
| dicomKeyword name | Y | |
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
