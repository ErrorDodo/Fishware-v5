using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for TabView.xaml
    /// </summary>
    public partial class TabView : UserControl
    {
        bool ProcShell = false;
        bool ProcWindow = false;
        string option;
        Process Proc = new Process();


        public class Accounts
        {
            public int ID { get; set; }
            public string Display { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int Wins { get; set; }
            public int Level { get; set; }
            public string ProfileURL { get; set; }
            public string Pfp { get; set; }

            public Accounts(int id, string display, string username, int wins, int level, string profile, string pfp)
            {
                this.ID = id;
                this.Display = display;
                this.Username = username;
                this.Wins = wins;
                this.Level = level;
                this.ProfileURL = profile;
                this.Pfp = pfp;
            }

            public object Get(string propertyName)
            {
                PropertyInfo property = GetType().GetProperty(propertyName);
                return property.GetValue(this, null);
            }
        }

        public static class PasswordManager
        {
            public static string Passphrase { get; set; }
        }

        public static ObservableCollection<Accounts> account = new ObservableCollection<Accounts>();

        public TabView()
        {
            InitializeComponent();
            output.ItemsSource = DataEntry.DataAccess.GetData();
        }

        private void output_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var profile1 = account[output.SelectedIndex].ProfileURL;
                System.Diagnostics.Process.Start(profile1);
                Console.WriteLine(profile1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Click an account first");
            }

        }

        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = account[output.SelectedIndex].ID;
                DataEntry.DataAccess.ReadPassword(id);
                var password = PasswordManager.Passphrase;
                var login = account[output.SelectedIndex].Username;

                Console.WriteLine($"{login}:{password}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Click an account first");
            }
        }

        private void DeleteEntry(object sender, RoutedEventArgs e)
        {
            var id = account[output.SelectedIndex].ID;
            DataEntry.DataAccess.DeleteEntry(id);
            output.ItemsSource = DataEntry.DataAccess.GetData();
        }

        private void steam(string option)
        {
            foreach (var process in Process.GetProcessesByName("steam"))
            {
                process.Kill();
                Console.WriteLine($"{process} has been killed");
            }
            foreach (var process in Process.GetProcessesByName("csgo"))
            {
                process.Kill();
                Console.WriteLine($"{process} has been killed");
            }
            string InstallPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null);
            if (InstallPath != null)
            {
                Proc.StartInfo.FileName = $"{InstallPath}\\steam.exe";
                Proc.StartInfo.Arguments = option;
                Proc.StartInfo.UseShellExecute = ProcShell;
                Proc.StartInfo.CreateNoWindow = ProcWindow;
                Proc.Start();
            }

            else
            {
                MessageBox.Show("Steam is not installed");
            }
        }

        private void LoginSelectedNoCSGO(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = account[output.SelectedIndex].ID;
                DataEntry.DataAccess.ReadPassword(id);
                var password = PasswordManager.Passphrase;
                var login = account[output.SelectedIndex].Username;
                option = $"-login {login} {password}";
                steam(option);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Click an account first");
            }
        }

        private void LoginSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                var id = account[output.SelectedIndex].ID;
                DataEntry.DataAccess.ReadPassword(id);
                var password = PasswordManager.Passphrase;
                var login = account[output.SelectedIndex].Username;
                option = $"-applaunch 730 -login {login} {password}";
                steam(option);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Click an account first");
            }
        }

        static Dictionary<string, Func<int, int, bool>> operators = new Dictionary<string, Func<int, int, bool>>()
        {
            { ">", (a, b) => { return a > b; } },
            { "<", (a, b) => { return a < b; } },
            { "=", (a, b) => { return a == b; } },
            { ">=", (a, b) => { return a >= b; } },
            { "<=", (a, b) => { return a <= b; } },
        };

        private void searchterms_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Accounts> accountlist = new List<Accounts>(account);

            if (searchterms.Text == string.Empty) { output.ItemsSource = account; } 
            else
            {
                string search = searchterms.Text.ToLower();
                string pattern = @"^[a-zA-Z]+\s*[><=]{1,2}\s*\d+";

                if (Regex.IsMatch(search, pattern))
                {
                    var match = Regex.Matches(search, @"^[a-zA-Z]+|[><=]{1,2}|\d+");
                    var member = match[0].Value;
                    var op = match[1].Value;
                    var number = Convert.ToInt32(match[2].Value);

                    Console.WriteLine("{0}\n{1}\n{2}", member, op, number);
                    var Descending = op.Contains(">");

                    switch (member)
                    {
                        case "wins":
                            {
                                if (Descending)
                                {
                                    try
                                    {
                                        account = new ObservableCollection<Accounts>(account.OrderByDescending(o => operators[op](o.Wins, number)).OrderByDescending(o => o.Wins).ToList());
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        account = new ObservableCollection<Accounts>(account.OrderBy(o => operators[op](o.Wins, number)).OrderBy(o => o.Wins).ToList());
                                    }
                                    catch{ }
                                }

                                output.ItemsSource = account;
                                break;
                            }
                        case "level":
                            {
                                if (Descending)
                                {
                                    try
                                    {
                                        account = new ObservableCollection<Accounts>(account.OrderByDescending(o => operators[op](o.Level, number)).OrderByDescending(o => o.Level).ToList());
                                    }
                                    catch { }
                                }
                                else
                                {
                                    try
                                    {
                                        account = new ObservableCollection<Accounts>(account.OrderBy(o => operators[op](o.Level, number)).OrderBy(o => o.Level).ToList());
                                    }
                                    catch { }
                                }

                                output.ItemsSource = account;
                                break;
                            }
                        default: Console.WriteLine("member doesn't exist"); break;
                    }
                }
                else
                {
                    Console.WriteLine("{0} doesn't match pattern {1}", search, pattern);
                }
            }
        }
    }
}
