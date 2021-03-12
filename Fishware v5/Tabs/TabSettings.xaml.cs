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

namespace Fishware_v5.Tabs
{
    /// <summary>
    /// Interaction logic for TabSettings.xaml
    /// </summary>
    public partial class TabSettings : UserControl
    {
        public TabSettings()
        {
            InitializeComponent();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            headerpicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#FF3971E6");
            sidepicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#FF2F61CB");
            titlecolour.SelectedColor = (Color)ColorConverter.ConvertFromString("#ffffff");
            sidetextcolour.SelectedColor = (Color)ColorConverter.ConvertFromString("#FF9BB8F5");
            boxcolour.SelectedColor = (Color)ColorConverter.ConvertFromString("#ffffff");
        }

        private void headerpicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Properties.Settings.Default.headercolour = headerpicker.SelectedColor.ToString();
            Properties.Settings.Default.Save();
        }

        private void ResetColourHeader_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.headercolour = "#FF3971E6";
            Properties.Settings.Default.Save();
            headerpicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#FF3971E6");
        }

        private void sidepicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Properties.Settings.Default.sidebarcolour = sidepicker.SelectedColor.ToString();
            Properties.Settings.Default.Save();
        }

        private void ResetSideColour_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.sidebarcolour = "#FF2F61CB";
            Properties.Settings.Default.Save();
            sidepicker.SelectedColor = (Color)ColorConverter.ConvertFromString("#FF2F61CB");
        }

        private void titlecolour_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Properties.Settings.Default.titlecolour = titlecolour.SelectedColor.ToString();
            Properties.Settings.Default.Save();
        }
        private void ResetTitleColour_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.titlecolour = "#ffffff";
            Properties.Settings.Default.Save();
            titlecolour.SelectedColor = (Color)ColorConverter.ConvertFromString("#ffffff");
        }

        private void sidecolour_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Properties.Settings.Default.subtitlecolour = sidetextcolour.SelectedColor.ToString();
            Properties.Settings.Default.Save();
        }

        private void ResetSideTextColour_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.subtitlecolour = "#FF9BB8F5";
            Properties.Settings.Default.Save();
            sidetextcolour.SelectedColor = (Color)ColorConverter.ConvertFromString("#FF9BB8F5");
        }

        private void boxcolour_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            Properties.Settings.Default.boxcolour = boxcolour.SelectedColor.ToString();
            Properties.Settings.Default.Save();
        }

        private void ResetBoxColour_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.boxcolour = "#ffffff";
            Properties.Settings.Default.Save();
            boxcolour.SelectedColor = (Color)ColorConverter.ConvertFromString("#ffffff");
        }

        private void Settings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = Settings.SelectedIndex;

            switch (selection)
            {
                case 0:
                    Console.WriteLine("General");
                    // Hiding Theme Settings
                    HeaderStack.Visibility = Visibility.Collapsed;
                    SideStack.Visibility = Visibility.Collapsed;
                    SideTextStack.Visibility = Visibility.Collapsed;
                    TitleStack.Visibility = Visibility.Collapsed;
                    BoxStack.Visibility = Visibility.Collapsed;

                    // Showing General Settings
                    GeneralMain.Visibility = Visibility.Visible;
                    ResetDB.Visibility = Visibility.Visible;
                    break;
                case 1:
                    Console.WriteLine("Theme");
                    // Showing Theme Settings
                    HeaderStack.Visibility = Visibility.Visible;
                    SideStack.Visibility = Visibility.Visible;
                    SideTextStack.Visibility = Visibility.Visible;
                    TitleStack.Visibility = Visibility.Visible;
                    BoxStack.Visibility = Visibility.Visible;

                    //Hiding General Settings
                    GeneralMain.Visibility = Visibility.Collapsed;
                    ResetDB.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    Console.WriteLine("Misc");
                    //Hiding Theme Settings
                    HeaderStack.Visibility = Visibility.Collapsed;
                    SideStack.Visibility = Visibility.Collapsed;
                    SideTextStack.Visibility = Visibility.Collapsed;
                    TitleStack.Visibility = Visibility.Collapsed;
                    BoxStack.Visibility = Visibility.Collapsed;

                    // Hiding General Settings
                    GeneralMain.Visibility = Visibility.Collapsed;
                    ResetDB.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Settings.Text = "General";
        }

        private void ResetDB_Click(object sender, RoutedEventArgs e)
        {
            DataEntry.DataAccess.ResetTable();
            MessageBox.Show("Account list has been reset", "Success");
        }
    }
}
