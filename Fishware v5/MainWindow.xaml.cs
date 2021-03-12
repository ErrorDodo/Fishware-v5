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

namespace Fishware_v5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataEntry.DataAccess.InitilizeDB();

            InitializeComponent();
        }

        public static void RestartApp()
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        private void ButtonPopUPExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void ButtonPopUPSettings_Click(object sender, RoutedEventArgs e)
        {
            UserControl usc = null;
            MainGrid.Children.Clear();

            usc = new Tabs.TabSettings();
            MainGrid.Children.Add(usc);
        }

        private void ButtonPopUPCredits_Click(object sender, RoutedEventArgs e)
        {
            UserControl usc = null;
            MainGrid.Children.Clear();

            usc = new Tabs.TabCredits();
            MainGrid.Children.Add(usc);
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            MainGrid.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Tag.ToString())
            {
                case "Home":
                    usc = new Tabs.TabHome();
                    MainGrid.Children.Add(usc);
                    break;
                case "View":
                    usc = new Tabs.TabView();
                    MainGrid.Children.Add(usc);
                    break;
                case "Add":
                    usc = new Tabs.TabAdd();
                    MainGrid.Children.Add(usc);
                    break;
            }
        }

        private void HeaderGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
