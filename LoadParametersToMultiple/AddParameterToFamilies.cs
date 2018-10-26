using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.ComponentModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using OpenRFA_WPF_CS;

namespace LoadParametersToMultiple
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]
    public class AddParameters
    {
        static Autodesk.Revit.ApplicationServices.Application app;
        UIApplication uiApp;
        static Autodesk.Revit.DB.Document doc;

    }
}
