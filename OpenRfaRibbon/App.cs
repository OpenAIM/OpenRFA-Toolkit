using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using OpenRFA_WPF_CS;

namespace OpenRfaRibbon
{
    class App : IExternalApplication
    {
        // define a method that will create our tab and button
        static void AddRibbonPanel(UIControlledApplication application)
        {
            // Create a custom ribbon tab
            String tabName = "OpenRFA";
            application.CreateRibbonTab(tabName);

            // Add a new ribbon panel
            RibbonPanel panelImport = application.CreateRibbonPanel(tabName, "Load Shared Parameters");
            RibbonPanel panelAbout = application.CreateRibbonPanel(tabName, "Additional Info");

            // Get dll assembly path
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // Create push button for LoadParametersToFamily
            PushButtonData pushButtDataLoadToFam = new PushButtonData(
                "cmdLoadParamsToFam",
                " Load Parameters " + System.Environment.NewLine + " To Current ",
                thisAssemblyPath,
                "OpenRFA_WPF_CS.Command");

            PushButton pushButtLoadToFam = panelImport.AddItem(pushButtDataLoadToFam) as PushButton;
            pushButtLoadToFam.ToolTip = "Load shared parameters into the current family.";
            BitmapImage iconLoadToFam = new BitmapImage(new Uri("pack://application:,,,/OpenRfaRibbon;component/Resources/address_book_pad.ico"));
            pushButtLoadToFam.LargeImage = iconLoadToFam;

            // Create push button for LoadParametersToMultiple
            PushButtonData pushButtDataLoadToFams = new PushButtonData(
                "cmdLoadToMultiple",
                " Load Parameters " + System.Environment.NewLine + " To Multiple Families ",
                thisAssemblyPath,
                "LoadParametersToMultiple.Command");

            PushButton pushButtLoadToMultiple = panelImport.AddItem(pushButtDataLoadToFams) as PushButton;
            pushButtLoadToMultiple.ToolTip = "Batch load shared parameters into multiple families.";
            BitmapImage iconLoadToMultiple = new BitmapImage(new Uri("pack://application:,,,/OpenRfaRibbon;component/Resources/appwizard_list.ico"));
            pushButtLoadToMultiple.LargeImage = iconLoadToMultiple;

            // Create push button for Support window
            PushButtonData pushButtDataSupport = new PushButtonData(
                "support",
                " Technical " + System.Environment.NewLine + " Support ",
                thisAssemblyPath,
                "SupportWindow.Command");

            PushButton pushButtSupport = panelAbout.AddItem(pushButtDataSupport) as PushButton;
            pushButtSupport.ToolTip = "Technical support for OpenRFA.";
            BitmapImage iconSupport = new BitmapImage(new Uri("pack://application:,,,/OpenRfaRibbon;component/Resources/address_book_users.ico"));
            pushButtSupport.LargeImage = iconSupport;

            // Create push button for About window
            PushButtonData pushButtDataAbout = new PushButtonData(
                "about",
                " About " + System.Environment.NewLine + " OpenRFA ",
                thisAssemblyPath,
                "AboutWindow.Command");

            PushButton pushButtAbout = panelAbout.AddItem(pushButtDataAbout) as PushButton;
            pushButtAbout.ToolTip = "About OpenRFA";
            BitmapImage iconAbout = new BitmapImage(new Uri("pack://application:,,,/OpenRfaRibbon;component/Resources/internet_connection_wiz.ico"));
            pushButtAbout.LargeImage = iconAbout;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // do nothing
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            // call our method that will load up our toolbar
            AddRibbonPanel(application);
            return Result.Succeeded;
        }
    }

}
