using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TNFAutoFoundation.Command;

namespace TNFAutoFoundation
{
    public class TNFApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.CreateRibbonTab("TNFAutoFoundation");
            var ribbon = application.CreateRibbonPanel("TNFAutoFoundation", "TNFAutoFoundation Panel");
            CreateRibbonButton(application, ribbon, nameof(TnfAutoFoundationCommand), "Auto TNF Foundation From Excel !", @"\AddSheet.ico", "TNFAutoFoundation");
            return Result.Succeeded;
        }
        private void CreateRibbonButton(UIControlledApplication application, RibbonPanel ribbon, string commandName, string decripptionTooltip, string iconStringPath, string nameSpace)
        {
            //var thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            var folderPath = AssemblyDirection();
            var assenblyPath = folderPath + @"\" + nameSpace + ".dll";
            PushButtonData buttonData = new PushButtonData("cmd" + commandName, commandName, assenblyPath, nameSpace + ".Command" + "." + commandName);
            var pustButton = ribbon.AddItem(buttonData) as PushButton;
            pustButton.ToolTip = decripptionTooltip;
            var iconImg = new Uri(folderPath + iconStringPath);
            var bitmapImg = new BitmapImage(iconImg);
            pustButton.LargeImage = bitmapImg;
        }

        public string AssemblyDirection()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
