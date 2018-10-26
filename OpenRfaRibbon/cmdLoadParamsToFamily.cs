#region Namespaces
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OpenRFA_WPF_CS;
#endregion

namespace OpenRFA_WPF_CS
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, 
            ref string message,
          ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Create local directory
            LocalFiles.CreateLocalDir();

            // Check OpenRFA.org for latest update to online db
            LocalFiles.GetLastUpdateJsonOnline();

            MainWindow appDialog = new MainWindow();
            appDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Check if current document is a Revit family
            if (doc.IsFamilyDocument)
            {

                // Only open window if continueCommand is set true
                if (ImportProcess.continueCommand == true)
                {
                    appDialog.ShowDialog();
                }

                // Only executes if the user clicked "OK" button
                if (appDialog.DialogResult.HasValue && appDialog.DialogResult.Value)
                {
                    
                    // Opens configuration window
                    ConfigureImport confDialog = new ConfigureImport();
                    confDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    confDialog.ShowDialog();

                    // Complete import process
                    ImportProcess.ProcessImport(doc, app, confDialog.DialogResult.HasValue, confDialog.DialogResult.Value);

                    // Clear all data in case addin is run again in the same session
                    // TODO: Call this method with every method that uses the datatables?
                    ImportProcess.ClearAllData();

                }

                return Result.Succeeded;
            }
            else
            {
                MessageBox.Show("The current document must be a Revit family to use this tool.");
                return Result.Failed;
            }

        }
    }
}
