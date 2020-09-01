FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Make directories
RUN mkdir -p ./DICOMcloud
RUN mkdir -p ./DICOMcloud.Azure
RUN mkdir -p ./DICOMcloud.Wado
RUN mkdir -p ./DICOMcloud.DataAccess.Database
RUN mkdir -p ./DICOMcloud.Wado.WebApi

#Copy solution file as well as projects
# COPY *.sln ./
# COPY ./DICOMcloud/*.csproj ./DICOMcloud
# COPY ./DICOMcloud.Azure/DICOMcloud.Azure.csproj ./DICOMcloud.Azure
# COPY ./DICOMcloud.Wado/DICOMcloud.Wado.csproj ./DICOMcloud.Wado
# COPY ./DICOMcloud.DataAccess.Database/*.csproj ./DICOMcloud.DataAccess.Database
# COPY ./DICOMcloud.Wado.WebApi/*.csproj ./DICOMcloud.Wado.WebApi
COPY . ./
#Restore
RUN dotnet restore -v n ./DICOMcloud.Wado.WebApi/DICOMcloud.Wado.WebApi.csproj
#Publish
RUN dotnet publish -c Debug -o out ./DICOMcloud.Wado.WebApi

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "DICOMcloud.Wado.WebApi.dll", "--environment=Development"]