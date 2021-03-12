using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Notifications.Wpf;
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
using System.Windows.Threading;
using System.Threading;

namespace Fishware_v5.Tabs
{
    /// <summary>
    /// Interaction logic for TabAdd.xaml
    /// </summary>
    public partial class TabAdd : UserControl
    {
        Queue<Task> tasks = new Queue<Task>();

        int current_count;
        int thread_count = 50;
        string date;
        int truetime;

        bool isChecking = false;

        public TabAdd()
        {
            InitializeComponent();

            DispatcherTimer clock = new DispatcherTimer();

            clock.Interval = new TimeSpan(0, 0, 0);
            clock.Tick += clock_tick;

            clock.Start();
        }

        private void clock_tick(object sender, EventArgs e)
        {
            DateTime d;

            d = DateTime.Now;

            truetime = ((d.Hour + 11) % 12) + 1;

            date = $"{truetime}:{d:mm} {d:tt}";

            if (!isChecking) { FileFind.IsEnabled = true; }
            else { FileFind.IsEnabled = false; }

        }

        private void FileFind_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog() { Filter = "Text Documents (*.txt) | *.txt" };
            if (ofd.ShowDialog() == true)
            {
                isChecking = true;
                var text = File.ReadAllLines(ofd.FileName, Encoding.UTF8);
                StartingAccount(text);
            }
        }

        private void AppendText(string str)
        {
            txt_log.Dispatcher.Invoke(new Action(() =>
            {
                txt_log.AppendText(str);
                txt_log.ScrollToEnd();
            }));
        }

        private void StartingAccount(string[] text)
        {
            new Task(() =>
            {
                foreach (var line in text)
                {
                    var log = $"[{date}] [FISHWARE] {line.Split(':')[0]}: Loaded {current_count} / {text.Length}\n";

                    current_count++;
                    var login = line;
                    tasks.Enqueue(new Task(() => CheckAccount(login, log)));
                }

                Parallel.ForEach(tasks, new ParallelOptions { MaxDegreeOfParallelism = thread_count },
                task =>
                {
                    task.Start();
                    task.Wait();
                });

                try
                {
                    AppendText($"[{date}] - [FISHWARE] All accounts have been saved to local database");

                    var notificationManager = new NotificationManager();
                    notificationManager.Show(new NotificationContent
                    {
                        Title = "Fishware Account Checker",
                        Message = $"{current_count} accounts have been loaded into local database",
                        Type = NotificationType.Information
                    });

                    isChecking = false;
                    current_count = 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }).Start();
        }

        private void CheckAccount(string login, string log)
        {
            AppendText(log);

            var details = login.Split(':');

            new DataEntry.BanCheck(details[0], details[1]).Run();
        }
    }
}
