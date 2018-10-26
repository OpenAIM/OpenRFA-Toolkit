#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json.Linq;
#endregion

namespace OpenRFA_WPF_CS
{
    public class ImportProcess
    {
        // Determine if command should be executed
        public static bool continueCommand = true;

        // Stores the list of GUIDs created by the WPF user interface
        public static List<string> ParamCache = new List<string>();

        // Stores the list of parameter NAMEs created by WPF (not preferred)
        public static List<string> NameCache = new List<string>();

        // Stores a DataTable of parameter definitions to bind propeties (e.g., group and IsInstance)
        public static DataTable dtParamCon = new DataTable();

        // Store list of parameters that were added
        public static List<string> addedParams = new List<string>();

        // Store list of parameters that were not added because they already exist
        public static List<string> existingParams = new List<string>();

        // DataTables
        public static DataTable dtParams;
        public static DataTable dtCart;

        // Misc
        public static IBindingListView blv;
        public static List<string> filterValues = new List<string>();

        // Output list of parameters from cart
        public static List<string> paramsOut = new List<string>();

        // The sp definitions file
        public static DefinitionFile defFile;

        /// <summary>
        /// Converts a list of GUID strings to a single string for testing
        /// </summary>
        /// <param name="_listOfParams">List of strings to convert</param>
        /// <returns>Converted string value</returns>
        public static string StringOfGuidsFromListItems(List<string> _listOfParams)
        {
            string stringOut = "";

            foreach (string p in _listOfParams)
            {
                stringOut += "\n" + p;
            }
            return stringOut;
        }

        /// <summary>
        /// Returns a datatable from a shared parameter definition file
        /// </summary>
        /// <param name="_defFile">The shared parameters text file.</param>
        /// <returns></returns>
        public static DataTable CreateConfigTable(DefinitionFile _defFile)
        {
            // iterate the Definition groups of this file
            foreach (DefinitionGroup group in _defFile.Groups)
            {
                // iterate the difinitions and create rows in the datatable
                foreach (Definition def in group.Definitions)
                {
                    dtParamCon.Rows.Add(def.Name.ToString());
                }
            }
            return dtParamCon;
        }

        /// <summary>
        /// Removes duplicate datarows from a datatable based on the column number
        /// </summary>
        /// <param name="dTable">The datatable to be parsed</param>
        /// <param name="colNum">The column number of to check for duplicates</param>
        /// <returns></returns>
        public static DataTable RemoveDuplicateRows(DataTable dTable, int colNum)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colNum]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colNum], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

        /// <summary>
        /// Configures parameters to be imported to a family
        /// </summary>
        /// <param name="doc">The Revit document to process (must be a Revit family).</param>
        /// <param name="app">The current Revit application.</param>
        /// <param name="dialogHasValue">Does the MainWindow object have a value?</param>
        /// <param name="dialogValue">The result of the MainWindow object.</param>
        public static void ProcessImport(
            Document doc,
            Autodesk.Revit.ApplicationServices.Application app,
            bool dialogHasValue,
            bool dialogValue
            )
        {
            using (Transaction trans = new Transaction(doc, "AddParams"))
            {
                // Save list of parameters from MainWindow
                ImportProcess.ParamCache = ImportProcess.paramsOut;

                // Check if configuration options have been saved
                if (dialogHasValue && dialogValue)
                {
                    // Start transaction
                    trans.Start();

                    // Set current shared parameters file
                    app.SharedParametersFilename = LocalFiles.tempDefinitionsFile;
                    defFile = app.OpenSharedParameterFile();

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
                        try {
                            // Write shared parameter to family
                            ImportProcess.ImportParameterToFamily(doc, defFile, _row, _selectedGroup, _instance);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Error");
                        }
                    }

                    trans.Commit();

                    // Show messages to user
                    if (ImportProcess.existingParams.Count > 0)
                    {
                        MessageBox.Show(ImportProcess.addedParams.Count + " parameters have been added to this family.\n" +
                            ImportProcess.existingParams.Count.ToString() + " parameters already exist in the family.");

                        // Clear list of parameters every time the method is complete
                        addedParams.Clear();
                        existingParams.Clear();
                    }
                    else
                    {
                        MessageBox.Show(ImportProcess.addedParams.Count + " parameters have been added to this family.\n");

                        // Clear list of parameters every time the method is complete
                        addedParams.Clear();
                        existingParams.Clear();
                    }


                }
                else
                {
                    MessageBox.Show("No parameters have been loaded.");
                }
            }
        }

        /// <summary>
        /// Adds all shared parameters from a definitions file to a family
        /// </summary>
        /// <param name="_doc">Current Revit document must be a family document</param>
        /// <param name="_defFile">The filtered shared parameters text file</param>
        /// <param name="_param">The parameter to be imported</param>
        /// <param name="_builtinGroup">The group(s) to assign the shared parameters to</param>
        /// <param name="_isInstance">Is the parameter an instance Parameter?</param>
        /// <returns></returns>
        public static string ImportParameterToFamily(Document _doc,
            DefinitionFile _defFile,
            DataRow _dataRow,
            BuiltInParameterGroup _builtinGroup,
            bool _isInstance)
        {
            StringBuilder testOutput = new StringBuilder();
            if (_doc.IsFamilyDocument)
            {
                testOutput.AppendLine("Parameters to be inserted: ");
                StringBuilder _sb = new StringBuilder();

                // iterate the Definition groups of this file
                foreach (DefinitionGroup group in _defFile.Groups)
                {
                    // iterate the definitions
                    foreach (Definition def in group.Definitions)
                    {
                        // Find matching definition from datatrow
                        if (def.Name == _dataRow[0].ToString())
                        {
                            // Create external definition and add parameter
                            ExternalDefinition extDef = def as ExternalDefinition;

                            try
                            {
                                // Add parameters to family
                                _doc.FamilyManager.AddParameter(extDef, _builtinGroup, _isInstance);
                                addedParams.Add(def.Name.ToString());
                            }
                            catch
                            {
                                // Check if parameters exist, if so, log for use as output
                                foreach (FamilyParameter familyParameter in _doc.FamilyManager.Parameters)
                                {
                                    if (familyParameter.Definition.Name == def.Name)
                                    {
                                        existingParams.Add(def.Name.ToString());
                                    }

                                }

                            }

                        }
                    }
                }

                return testOutput.ToString();
            }
            else
            {
                return "You must have a family open to use this tool.";
            }
        }

        /// <summary>
        /// Remove all columns from a datatable.
        /// </summary>
        /// <param name="_dataTable">The datatable to have the columns removed</param>
        public static void RemoveColumns(DataTable _dataTable)
        {
            for (int index = _dataTable.Columns.Count - 1; index >= 0; index--)
            {
                _dataTable.Columns.RemoveAt(index);
            }

        }

        /// <summary>
        /// Clears all data from datatables, gridviews, and lists.
        /// </summary>
        public static void ClearAllData()
        {
                ConfigureImport.dtConfig.Clear();
                RemoveColumns(ConfigureImport.dtConfig);
                addedParams.Clear();
                existingParams.Clear();
        }

        /// <summary>
        /// Removes duplicate entries in a DataTable
        /// </summary>
        /// <param name="dTable"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

    }
}
