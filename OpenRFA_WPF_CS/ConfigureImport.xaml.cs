using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using OpenRFA_WPF_CS;

namespace OpenRFA_WPF_CS
{

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ConfigureImport : Window
    {
        // DataTable for storing configuration settings
        public static DataTable dtConfig = new DataTable();

        public ConfigureImport()
        {

            InitializeComponent();

            // Build dataset from parameter cache only adding columns if they do not exist
            if (!dtConfig.Columns.Contains("Parameter"))
            {
                dtConfig.Columns.Add("Parameter");
            }
            if (!dtConfig.Columns.Contains("AssignGroup"))
            {
                dtConfig.Columns.Add("AssignGroup");
            }
            if (!dtConfig.Columns.Contains("InstanceOrType"))
            {
                dtConfig.Columns.Add("InstanceOrType");
            }

            // Adds all rows from ParamCache
            for (int i = 0; i < ImportProcess.ParamCache.Count; i++)
            {
                DataRow row = dtConfig.Rows.Add(i);
                dtConfig.Rows[i]["Parameter"] = ImportProcess.ParamCache[i];
            }

            // Generate list for Instance/Type comboboxes
            List<string> InstanceOrType = new List<string>();
            InstanceOrType.Add("Instance");
            InstanceOrType.Add("Type");

            gridParamCon.ItemsSource = dtConfig.DefaultView;

            // Get BuiltInParameterGroup labels for combobox
            AssignGroup.ItemsSource = SPBuiltInGroup.GetGroupLabels(SPBuiltInGroup.GroupList.GetAllBuiltInGroups());

            this.InstanceOrType.ItemsSource = InstanceOrType;

        }

        private void ButtonImport_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void gridParamCon_CurrentCellChanged(object sender, EventArgs e)
        {
         dtConfig = ((DataView)gridParamCon.ItemsSource).ToTable();
        }

        private void gridParamCon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
