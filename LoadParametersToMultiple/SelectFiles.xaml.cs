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

namespace LoadParametersToMultiple
{
    /// <summary>
    /// Interaction logic for SelectFiles.xaml
    /// </summary>
    public partial class SelectFiles : Window
    {
        // Store selected files as list
        public static List<string> selectedFiles = new List<string>();

        public SelectFiles()
        {
            InitializeComponent();

        }
    }
}
