#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using AboutWindow;
#endregion

namespace AboutWindow
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Popup aboutWindow = new Popup();
            aboutWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            aboutWindow.ShowDialog();

            if (aboutWindow.DialogResult.HasValue && aboutWindow.DialogResult.Value)
            {
                aboutWindow.Close();
                return Result.Succeeded;
            }
            else
            {
                aboutWindow.Close();
                return Result.Cancelled;
            }
        }
    }
}
