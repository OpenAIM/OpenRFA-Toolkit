#region Namespaces
using System;
using System.Collections.Generic;
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
using System.IO;
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
using OpenRFA_WPF_CS;
#endregion

namespace OpenRFA_WPF_CS
{

    /// <summary>
    /// A shared parameter from the OpenRFA list of shared parameters.
    /// </summary>
    public class SharedParameter
    {
        public string guid { get; set; }
        public string name { get; set; }
        public string datatype { get; set; }
        public string datacategory { get; set; }
        public string group { get; set; }
        public int visible { get; set; }
        public string description { get; set; }
        public int usermodifiable { get; set; }
        public string state { get; set; }
        public string parameter_sets { get; set; }
        public string cobie_parameter { get; set; }
        public string ifc_common_propertyset_name { get; set; }
        public string ifc_property_name { get; set; }
        public string lang_override_spanish { get; set; }

        /// <summary>
        /// Download serialized JSON data.
        /// </summary>
        /// <typeparam name="T">The data type to store the JSON data.</typeparam>
        /// <param name="url">The URL of the JSON file.</param>
        /// <returns></returns>
        public static T DownloadJsonAsList<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance 
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }

        /// <summary>
        /// Convert a list to a DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
                
            }
            return dataTable;
        }

        public static List<string> GetBuiltInGroups()
        {
            List<string> groupsOut = new List<string>();

            MemberInfo[] _groups = typeof(BuiltInParameterGroup).GetProperties();
            foreach (MemberInfo group in _groups)
            {
                groupsOut.Add(group.Name.ToString());
            }

            return groupsOut;
        }

        public static BuiltInParameterGroup GetBuiltInGroup(string _name)
        {
            if (_name == "Mechanical")
            {
                return BuiltInParameterGroup.PG_MECHANICAL;
            }
            else
            {
                return BuiltInParameterGroup.PG_IDENTITY_DATA;
            }
        }

    }
}
