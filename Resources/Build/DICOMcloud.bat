copy "Default.nuspec" "../../DICOMcloud\DICOMcloud.nuspec"
copy "Default.nuspec" "../../DICOMcloud.Azure\DICOMcloud.Azure.nuspec"
copy "Default.nuspec" "../../DICOMcloud.DataAccess.Database\DICOMcloud.DataAccess.Database.nuspec"
copy "Default.nuspec" "../../DICOMcloud.Wado\DICOMcloud.Wado.nuspec"

nuget pack ../../DICOMcloud\DICOMcloud.csproj -Prop Configuration=Release  -OutputDirectory ../../dist\NuGet\ -IncludeReferencedProjects
nuget pack ../../DICOMcloud.Azure\DICOMcloud.Azure.csproj -Prop Configuration=Release  -OutputDirectory ../../dist\NuGet\ -IncludeReferencedProjects
nuget pack ../../DICOMcloud.DataAccess.Database\DICOMcloud.DataAccess.Database.csproj -Prop Configuration=Release  -OutputDirectory ../../dist\NuGet\ -IncludeReferencedProjects
nuget pack ../../DICOMcloud.Wado\DICOMcloud.Wado.csproj -Prop Configuration=Release -OutputDirectory ../../dist\NuGet\ -IncludeReferencedProjects
nuget pack ../Build\DICOMcloud.Wado.WebApi\DICOMcloud.Wado.WebApi.csproj -Prop Configuration=Release  -OutputDirectory ../../dist\NuGet\ -IncludeReferencedProjects -tool -build -verbosity detailed