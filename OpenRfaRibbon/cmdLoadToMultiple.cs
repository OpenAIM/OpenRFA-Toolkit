#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using OpenRFA_WPF_CS;
using LoadParametersToMultiple;
#endregion

namespace LoadParametersToMultiple
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
            Document docUi = uidoc.Document;

            bool allGoodInTheHood = false;

            //app.SharedParametersFilename = LocalFiles.localSpFile;
            //DefinitionFile defFile = app.OpenSharedParameterFile();
            //DefinitionGroups myGroups = defFile.Groups;
            //DefinitionGroup myGroup = myGroups.get_Item("Mechanical");
            //Definitions myDefinitions = myGroup.Definitions;
            //ExternalDefinition eDef = myDefinitions.get_Item("AirFlowAirTerminal") as ExternalDefinition;

            // Create local directory
            LocalFiles.CreateLocalDir();

            // Checks last time the OpenRFA online database was udpated
            LocalFiles.GetLastUpdateJsonOnline();

            MainWindow appDialog = new MainWindow();
            appDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Prompt user to select files
            Microsoft.Win32.OpenFileDialog openFilesDlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            openFilesDlg.DefaultExt = ".rfa";
            openFilesDlg.Filter = "Revit Family (*.rfa)|*.rfa";
            openFilesDlg.Multiselect = true;
            openFilesDlg.Title = "Select Revit families to load parameters";

            // Only open window if continueCommand is set true
            if (ImportProcess.continueCommand == true)
            {
                openFilesDlg.ShowDialog();

                // Only open MainWindow if files are selected
                if (openFilesDlg.FileNames.Length > 0)
                {
                    appDialog.ShowDialog();

                    // Print all families to be modified
                    StringBuilder sb = new StringBuilder();
                    sb.Append("The following files will be modified. It is recommended to backup your families before proceeding. Would you like to continue?\n\n");
                    foreach (string fileName in openFilesDlg.FileNames)
                    {
                        sb.Append(fileName + "\n");
                    }

                    MessageBoxResult resultConfirmOverwrite = System.Windows.MessageBox.Show(sb.ToString(), "Warning", MessageBoxButton.OKCancel);
                    switch (resultConfirmOverwrite)
                    {
                        // Execute command if user confirmed
                        case MessageBoxResult.OK:


                            // Only executes if the user clicked "OK" button
                            if (appDialog.DialogResult.HasValue && appDialog.DialogResult.Value)
                            {

                                // Opens configuration window
                                ConfigureImport confDialog = new ConfigureImport();
                                confDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                confDialog.ShowDialog();

                                // Only execute command if configure process is committed by user
                                if (confDialog.DialogResult.HasValue && confDialog.DialogResult.Value)
                                {
                                    // Iterate through selected families and add parameters to each
                                    foreach (string fileName in openFilesDlg.FileNames)
                                    {
                                        // Check if user is trying to modify active doc
                                        if (fileName == docUi.PathName)
                                        {
                                            System.Windows.MessageBox.Show("This addin cannot be run on the active document. This family has been skipped in the process: \n" + fileName);
                                        }
                                        else
                                        {
                                            System.Windows.MessageBox.Show("Adding parameters to: " + fileName);
                                            doc = app.OpenDocumentFile(fileName);

                                            // Complete import process
                                            ImportProcess.ProcessImport(doc, app, confDialog.DialogResult.HasValue, confDialog.DialogResult.Value);
                                            doc.Close(true);

                                        }
                                    }
                                }
                            }

                            // Clear all data in case addin is run again in the same session
                            // TODO: Call this method with every method that uses the datatables?
                            ImportProcess.ClearAllData();

                            allGoodInTheHood = true;
                            break;

                        case MessageBoxResult.Cancel:
                            ImportProcess.ClearAllData();
                            allGoodInTheHood = false;
                            break;
                    }
                }
            }


            // Return results
            if (allGoodInTheHood)
            {
                return Result.Succeeded;
            }
            else
            {
                return Result.Cancelled;
            }

        }

    }
}
