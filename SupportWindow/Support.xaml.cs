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

namespace SupportWindow
{
    /// <summary>
    /// Interaction logic for Support.xaml
    /// </summary>
    public partial class Support : Window
    {
        public Support()
        {
            InitializeComponent();
        }

        private void btnSupport_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://join.slack.com/t/openrfa/shared_invite/enQtMjQ5OTA1MjgxODMwLTAyMDhiZmE0ZDM5NjMyZTE4N2M5ZWZlMTY1Mzg5OWQ5MGMyYmFjZDFmY2FlODE1ODlmNzJjMjQ2NmFlYTMzMTA");
        }

        private void btnSupportWww_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://openrfa.org/forums/technical-support");
        }
    }
}
