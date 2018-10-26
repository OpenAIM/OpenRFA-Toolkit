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

namespace OpenRFA_WPF_CS
{
    public static class ExportUtils
    {

        // Test: What line is the fitler starting at?
        public static int startingLine;

        /// <summary>
        /// Filters the given shared parameter text file showing only the selected GUIDs
        /// </summary>
        /// <param name="_filePath">Path to the shared parameters text file</param>
        /// <param name="_newFile">Path to save the filtered shared parameters text file</param>
        /// <param name="_guids">The GUIDs to keep in the filtered text file</param>
        public static void FilterCSV(string _filePath, string _newFile, List<string> _guids)
        {
            // Read text file as list of strings
            List<string> lines = new List<string>(System.IO.File.ReadAllLines(_filePath));

            // Find the line that the parameters start on (skip the header)
            startingLine = lines.FindIndex(x => x.StartsWith("*PARAM"));

            // Loop through all lines and serch for matching GUIDs
            for (int i = lines.Count() - 1; i > startingLine; i--)
            {
                if (!_guids.Any(lines[i].Contains))
                {
                    lines.RemoveAt(i);
                }
            }
            
            // Write the filtered shared parameters to file
            System.IO.File.WriteAllLines(_newFile, lines);
        }

        /// <summary>
        /// Export a data table of Parameters to a CSV file
        /// </summary>
        /// <param name="dtDataTable">Data table to convert</param>
        /// <param name="strFilePath">Path to the CSV file</param>
        public static void ToCSV(this DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}
