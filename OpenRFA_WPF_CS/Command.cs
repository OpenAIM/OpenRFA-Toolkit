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
        public static DefinitionFile defFile;

        public Result Execute(ExternalCommandData commandData, 
            ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Launches the application window (WPF) for user input
            OpenRFA_WPF_CS.MainWindow appDialog = new MainWindow();
            appDialog.ShowDialog();


            {
                // Only executes if the user clicked "OK" button
                if (appDialog.DialogResult.HasValue && appDialog.DialogResult.Value)
                {

                    // Opens configuration window
                    OpenRFA_WPF_CS.ConfigureImport confDialog = new ConfigureImport();
                    confDialog.ShowDialog();

                    using (Transaction trans = new Transaction(doc, "AddParams"))
                    {
                        // Save list of parameters from MainWindow
                        ImportProcess.ParamCache = MainWindow.paramsOut;

                        // Check if configuration options have been saved
                        if (confDialog.DialogResult.HasValue && confDialog.DialogResult.Value)
                        {
                            // Test text for showing if datatable has been updated.
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Updated datatable: \n");
                            foreach (DataRow dr in ConfigureImport.dtConfig.Rows)
                            {
                                sb.Append(dr[0] + ", " + dr[1] + "," + dr[2] + "\n");
                            }
                            MessageBox.Show(sb.ToString());

                            trans.Start();

                            // Set current shared parameters file
                            app.SharedParametersFilename = LocalFiles.tempDefinitionsFile;
                            defFile = app.OpenSharedParameterFile();

                            // Adds shared parameters to family
                            // TODO: Pass a list of BuiltInParameterGroup (currently only a placeholder) for overload
                            //SharedParameter.ImportParameterToFamily(doc, defFile, BuiltInParameterGroup.PG_MECHANICAL);

                            foreach (DataRow _row in ConfigureImport.dtConfig.Rows)
                            {
                                // Check if configuration is set to instance.
                                // TODO: Turn this into a method.
                                bool _instance = false;
                                if (_row[2].ToString() == "Instance")
                                {
                                    _instance = true;
                                }
                                else
                                {
                                    _instance = false;
                                }

                                // Get BuiltInParameterGroup by name
                                BuiltInParameterGroup _bipGroup = new BuiltInParameterGroup();
                                _bipGroup = SPBuiltInGroup.GetByName(_row[1].ToString());

                                // Get BIPG using the BuiltinParameterGroupLookup Class
                                var lookup = new BuiltInParameterGroupLookup();
                                BuiltInParameterGroup _selectedGroup = lookup[_row[1].ToString()];

                                // Write shared parameter to family
                                SharedParameter.ImportParameterToFamily(doc, defFile, _row, _selectedGroup, _instance);
                            }

                            trans.Commit();
                        }
                        else
                        {
                            MessageBox.Show("Operation canceled.");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Operation canceled.");
                }
                
            }

            // Clear DataTables. TODO: Turn this into a method.
            ConfigureImport.dtConfig.Clear();
            ImportProcess.RemoveColumns(ConfigureImport.dtConfig);

            return Result.Succeeded;
        }
    }
}
