Getting started with DICOMcloud.Wado.WebApi
---------------------------------

To get started, add a call to GlobalConfiguration.Configuration.UseStructureMap with the DICOMcloudBuilder registry in the Application_Start method of Global.asax.cs and the Web API framework will be configured to use the DICOMcloud web server.

e.g.
 
public class WebApiApplication : System.Web.HttpApplication
{
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configure(DICOMcloudBuilder.ConfigureLogging);  /*  <----- Add this line to configure DICOMcloud custom logging handling */
            
            GlobalConfiguration.Configuration.UseStructureMap(x =>              /*  <----- Add this line to configure DICOMcloud */
            {
                x.AddRegistry<DICOMcloudBuilder>();
            });
        }            
}

Update the web.config with your file storage location and database info:
   <appSettings>
      <!--***********TODO: update your storage connection*********-->
      <!-- <add key="app:PacsStorageConnection" value="UseDevelopmentStorage=true;" />-->
      <!--<add key="app:PacsStorageConnection" value="C:\DICOMcloud\Storage" />-->
      <!--************************************************************-->

      <!--***********TODO: update your databbase connection string*********-->
      <!--<add key="app:PacsDataArchieve" value="TODO: ENTER YOUR SQL DATABASE CONNECTION STRING" />-->
      <!--***********************************************************-->
   </appSettings>

To create the database, see this wiki page: https://github.com/Zaid-Safadi/DICOMcloud/wiki/Building-and-Running-the-code