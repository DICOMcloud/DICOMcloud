using System.Web;
using System.Web.Optimization;

namespace DICOMcloud.Wado
{
   public class BundleConfig
   {
      // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
      public static void RegisterBundles(BundleCollection bundles)
      {
         // Set EnableOptimizations to false for debugging. For more information,
         // visit http://go.microsoft.com/fwlink/?LinkId=301862
         BundleTable.EnableOptimizations = true;
      }
   }
}
