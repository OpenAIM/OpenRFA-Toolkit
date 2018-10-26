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

namespace AboutWindow
{

    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        // The current version of the add-in suite
        public string addinTitle = "OpenRFA Parameter Tools";
        public string addinVersion = "0.2";
        public string addinCopyright = "©2018 BIM Extension, LLC";

        public Popup()
        {
            InitializeComponent();
            textTitle.Text = addinTitle;
            textVersion.Text = "Version " + addinVersion;
            textCopyright.Text = addinCopyright;
        }

        private void buttOpenWebsite_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://openrfa.org");
        }
    }
}
