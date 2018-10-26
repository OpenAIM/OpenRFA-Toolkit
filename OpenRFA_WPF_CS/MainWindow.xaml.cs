#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Net;
using System.Data;
using System.Web.Script.Serialization;
using System.Reflection;
using System.ComponentModel;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace OpenRFA_WPF_CS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        // Message to be displayed to user
        public static string notifcationMessage = "";

        public MainWindow()
        {
            //// Fields for checking for updates
            //DateTime dbLastUpdated = LocalFiles.UnixTimeStampToDateTime(LocalFiles.GetLastUpdatedDateTime());
            //DateTime localFileUpdated = File.GetLastWriteTimeUtc(LocalFiles.localJsonFile);

            InitializeComponent();

            // Prompt user to download definitions if local files are missing
            if (!File.Exists(LocalFiles.localJsonFile) || !File.Exists(LocalFiles.localSpFile))
            {
                LocalFiles.definitionsUpToDate = false;

                string messageBoxText = "You must sync with the OpenRFA.org shared parameter defintions to use this add-in. Would you like to sync now?";
                MessageBoxResult result = MessageBox.Show(messageBoxText, "OpenRFA Parameter Definitions Missing", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // Download JSON/TXT from OpenRFA.org
                        LocalFiles.DownloadLocalData();
                        RefreshData();

                        LocalFiles.definitionsUpToDate = true;
                        break;

                    case MessageBoxResult.No:
                        LocalFiles.definitionsUpToDate = false;
                        //DialogResult = true;
                        break;
                }

            }
            else
            {
                // Store date/times locally to ensure methods are called from all scopes
                DateTime localFileUpdated = LocalFiles.GetLastUpdateJsonLocal();
                DateTime dbLastUpdated = LocalFiles.GetLastUpdateJsonOnline();

                // Prompt user to updated definitions if local file is outdated
                if (localFileUpdated < dbLastUpdated)

                //&& LocalFiles.dbLastUpdated != LocalFiles.UnixTimeStampToDateTime(000))
                {
                    string messageBoxText = "OpenRFA.org was udpated: " + dbLastUpdated.ToLocalTime().ToString() +
                        "\nYour last sync: " + localFileUpdated.ToString() +
                        "\n\nWould you like to sync with OpenRFA.org?";

                    MessageBoxResult result = MessageBox.Show(messageBoxText, "OpenRFA Parameter Definitions Outdated", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            // Download JSON/TXT from OpenRFA.org
                            LocalFiles.DownloadLocalData();
                            RefreshData();

                            // Store new values for updated definitions
                            LocalFiles.definitionsUpToDate = true;
                            break;
                        case MessageBoxResult.No:
                            LocalFiles.definitionsUpToDate = false;
                            break;
                    }

                }
                if (dbLastUpdated == LocalFiles.UnixTimeStampToDateTime(000))
                {
                    MessageBox.Show("Can't connect to database. Please ensure you are online.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                // Notify the user that definitions are up to date
                if (localFileUpdated >= dbLastUpdated)
                {
                    LocalFiles.definitionsUpToDate = true;
                    //string messageBoxText = "Your parameter definitions are synced with OpenRFA.org.";
                    //MessageBoxResult result = MessageBox.Show(messageBoxText, "OpenRFA Parameter Definitions Up to Date", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                RefreshData();
            }

            // Notify users of status of parameter definitions
            if (LocalFiles.definitionsUpToDate == true)
            {
                UpdateStatusText("Your parameter definitions are up to date.");
            }
            else
            {
                UpdateStatusText("You parameter definitions are outdated.");
            }

        }

        // Access and update columns during autogeneration (for hiding columns)
        private void gridParams_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();

            if (e.PropertyName == "guid")
            {
                e.Column.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        /// <summary>
        /// Updates the status bar text with a string
        /// </summary>
        /// <param name="s">String to pass to the status bar.</param>
        private void StatusParamDefsUpdated()
        {
            
            if (LocalFiles.definitionsUpToDate == true)
            {
                textStatus.Text = "Your parameter definitions are up to date.";
            }
            if (LocalFiles.definitionsUpToDate == false)
            {
                textStatus.Text = "Your parameter definitions are out of date.";
            }
        }

        private void textFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            //// Filter name while typing. Currently a perfomance issue so disabled.
            //blv.Filter = "Name like '%" + textFilter.Text + "%'";
        }

        /// <summary>
        /// Generates a string to filter the BindingListView.
        /// </summary>
        /// <param name="_filterValues"></param>
        public string GenerateFilterString(List<string> _filterValues)
        {
            string combinedFilters = "";

            // Pass single filter value as string
            if (_filterValues.Count == 1)
            {
                combinedFilters = _filterValues[0];
            }

            // Concatenate multiple filter values as single string
            if (_filterValues.Count > 1)
            {
                foreach (string f in _filterValues)
                {
                    combinedFilters = combinedFilters + f + " and ";
                }
                // Clean up trailing " and " from string
                combinedFilters = combinedFilters.Remove(combinedFilters.Length - 5);
            }

            return combinedFilters;
        }

        // Filter button is clicked
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous values from dictionary
            ImportProcess.filterValues.Clear();

            // Filtering statements

            // Filter by name
            if (textFilter.Text != "")
            {
                ImportProcess.filterValues.Add("Name like '%" + textFilter.Text + "%'");
            }

            // Filter by datatype
            if (comboDataType.Text != "")
            {
                ImportProcess.filterValues.Add("datatype = '" + comboDataType.Text + "'");
            }

            // Filter by group
            if (comboParamGroup.Text != "")
            {
                ImportProcess.filterValues.Add("group = '" + comboParamGroup.Text + "'");
            }

            // Filter by parameter set
            if (comboParameterSet.Text != "")
            {
                ImportProcess.filterValues.Add("parameter_sets like '%" + comboParameterSet.Text + "%'");
            }

            // Create filters for State checkboxes
            List<string> stateChecks = new List<string>();
            string combinedStateFilters = "";

            if (checkApproved.IsChecked == true)
            {
                stateChecks.Add("state = 'Approved'");
            }
            if (checkProposed.IsChecked == true)
            {
                stateChecks.Add("state = 'Proposed'");
            }
            if (checkRejected.IsChecked == true)
            {
                stateChecks.Add("state = 'Rejected'");
            }
            // Pass single filter value as string
            if (stateChecks.Count == 1)
            {
                combinedStateFilters = stateChecks[0];
            }
            // Concatenate multiple filter values as single string
            if (stateChecks.Count > 1)
            {
                foreach (string f in stateChecks)
                {
                    combinedStateFilters = combinedStateFilters + f + " or ";
                }
                // Clean up trailing " and " from string
                combinedStateFilters = combinedStateFilters.Remove(combinedStateFilters.Length - 4);
            }
            ImportProcess.filterValues.Add(combinedStateFilters);

            // Generate filter string
            ImportProcess.blv.Filter = GenerateFilterString(ImportProcess.filterValues);
        
        // Update status bar text
        UpdateStatusText(ImportProcess.blv.Filter);

        }

        // Reset button is clicked
        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            ImportProcess.blv.RemoveFilter();
            textFilter.Text = "";
            comboDataType.Text = "";
            comboParamGroup.Text = "";
            comboParameterSet.Text = "";
            ImportProcess.blv.Filter = "";
            
            UpdateStatusText("Filters cleared.");
        }

        private void gridParams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Add selected shared parameters to cart DataTable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (gridParams.SelectedItems.Count > 0)
            {
                foreach (DataRowView dr in gridParams.SelectedItems)
                {
                    //DataRow selectedParam = (System.Data.DataRow)gridParams.SelectedItems[i];
                    if (dr != null)
                    {
                        ImportProcess.dtCart.Rows.Add(dr.Row.ItemArray);
                    }

                    // Update status text
                    if (gridParams.SelectedItems.Count == 1)
                    {
                        UpdateStatusText("Added (1) parameter to list of parameters to be added.");
                    }
                    if (gridParams.SelectedItems.Count > 1)
                    {
                        UpdateStatusText("Added (" + gridParams.SelectedItems.Count.ToString() + ") parameters to list of parameters to be added.");
                    }
                }
            }

            // Remove duplicate entries
            ImportProcess.dtCart = ImportProcess.RemoveDuplicateRows(ImportProcess.dtCart, "name");
            
            // Refresh DataGrid
            gridCart.Items.Refresh();
        }

        /// <summary>
        /// Cancels the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            ImportProcess.ClearAllData();
        }

        /// <summary>
        /// Saves the data from the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCommit_Click(object sender, RoutedEventArgs e)
        {
            // Clears existing data and ensures there are parameters added
            ImportProcess.paramsOut.Clear();

            if (ImportProcess.dtCart.Rows.Count < 1)
            {
                MessageBox.Show("No parameters have been selected to load to the family.");
            }

            foreach (DataRow dr in ImportProcess.dtCart.Rows)
            {
                // Adds each GUID
                //paramsOut.Add(dr[0].ToString());

                // Adds each parameter NAME (not preferred)
                ImportProcess.paramsOut.Add(dr[1].ToString());
            }

            ExportUtils.FilterCSV(LocalFiles.localSpFile, LocalFiles.tempDefinitionsFile, ImportProcess.paramsOut);

            // Stores the list of parameters to the main ImportProcess cache
            // for use in the Revit Command.
            ImportProcess.ParamCache = ImportProcess.paramsOut;

            //UpdateStatusText("Total Imported Parameters: " + paramsOut.Count.ToString());
            //UpdateStatusText(ExportUtils.startingLine.ToString());

            DialogResult = true;
        }
        
        /// <summary>
        /// Downloads online JSON/TXT and refreshes data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            // Download JSON/TXT from OpenRFA.org
            LocalFiles.DownloadLocalData();
            RefreshData();
        }

        /// <summary>
        /// Updates the status bar
        /// </summary>
        /// <param name="s"></param>
        private void UpdateStatusText(string s)
        {
            textStatus.Text = s;
        }

        /// <summary>
        /// Refreshes all data including DataTables, DataViews, DataContext, etc.
        /// </summary>
        public void RefreshData()
        {
            // Convert Json to list
            LocalFiles.JsonToList();

            // Convert list to DataTable
            ImportProcess.dtParams = SharedParameter.ToDataTable(LocalFiles.sharedParams);

            // Creates new view object of DataTable for customizing columns
            DataView view = new DataView(ImportProcess.dtParams);

            // Bind all parameter properties to the DataGrid
            gridParams.DataContext = ImportProcess.dtParams.DefaultView;

            // Create DataTable for cart to add parameters to and display in grid
            ImportProcess.dtCart = SharedParameter.ToDataTable(LocalFiles.sharedParams);
            // Clear the cart DataTable - we only add DataContext to create identical columns. Is there a better way?
            ImportProcess.dtCart.Clear();
            gridCart.DataContext = ImportProcess.dtCart.DefaultView;

            ImportProcess.blv = ImportProcess.dtParams.DefaultView;

            // Set the BindingListView to show approved parameters by default
            ImportProcess.blv.Filter = "state = 'Approved'";

            // Populate combobox with DataType options for filtering
            // Create a view of distinct datatypes
            DataTable dtParamsDistinct = view.ToTable(true, "datatype");
            dtParamsDistinct.DefaultView.Sort = "datatype asc";
            comboDataType.ItemsSource = dtParamsDistinct.DefaultView;

            // Populate Group combobox with parameter groups
            DataTable dtParamGroups = view.ToTable(true, "group");
            dtParamGroups.DefaultView.Sort = "group asc";
            comboParamGroup.ItemsSource = dtParamGroups.DefaultView;

            // Populate combobox with parameter sets
            List<string> _newSets = new List<string>();
            DataTable dtParamSets = view.ToTable(true, "parameter_sets");

            // Split comma separated values for parameters with multiple tags
            foreach (DataRow _dr in dtParamSets.Rows)
            {
                if (_dr[0].ToString().Contains(", "))
                {
                    // Split multiple values
                    foreach (string s in _dr[0].ToString().Split(','))
                    {
                        _newSets.Add(s);
                    }


                }
            }

            // Remove rows that have multiple values
            for (int i = dtParamSets.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = dtParamSets.Rows[i];
                if (dr[0].ToString().Contains(", "))
                {
                    dr.Delete();
                }

            }
            dtParamSets.AcceptChanges();

            for (int i = 0; i < _newSets.Count(); i++)
            {
                // Remove extra space from split
                if (_newSets[i].StartsWith(" "))
                {
                    _newSets[i] = _newSets[i].Substring(1);
                }

                // Add parameter sets from list as data rows
                DataRow _newRow = dtParamSets.NewRow();
                _newRow[0] = _newSets[i];
                dtParamSets.Rows.Add(_newRow);

            }

            // Remove duplicates
            ImportProcess.RemoveDuplicateRows(dtParamSets, 0);
            dtParamSets.DefaultView.Sort = "parameter_sets asc";
            comboParameterSet.ItemsSource = dtParamSets.DefaultView;

        }

        /// <summary>
        /// Removes a shared parameter from the cart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (gridCart.SelectedItems.Count > 0)
            {
                //for (DataRowView dr in gridCart.SelectedItems)
                for (int i = gridCart.SelectedItems.Count - 1; i >= 0; i--)
                {
                    //DataRow selectedParam = (System.Data.DataRow)gridParams.SelectedItems[i];
                    if (gridCart.SelectedItems[i] != null)
                    {
                        DataRowView row = (DataRowView)gridCart.SelectedItems[i];
                        ImportProcess.dtCart.Rows.Remove(row.Row);
                    }

                    // Update status text
                    if (gridParams.SelectedItems.Count == 1)
                    {
                        UpdateStatusText("Removed parameter from list of parameters to be added.");
                    }
                    if (gridParams.SelectedItems.Count > 1)
                    {
                        UpdateStatusText("Removed parameters from the list of parameters to be added.");
                    }
                    ImportProcess.dtCart.AcceptChanges();
                }
            }

        }

        private void comboParamGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void buttNewParam_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://openrfa.org/node/add/shared-parameter");
        }
    } 
}
