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

namespace Fishware_v5.Tabs
{
    /// <summary>
    /// Interaction logic for TabCredits.xaml
    /// </summary>
    public partial class TabCredits : UserControl
    {
        public TabCredits()
        {
            InitializeComponent();
        }

        private void GithubDodo_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Fishw4re");
        }
    }
}
