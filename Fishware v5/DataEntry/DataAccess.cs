using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;
using static Fishware_v5.Tabs.TabView;

namespace Fishware_v5.DataEntry
{
    class DataAccess
    {
        static string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string specificFolder = System.IO.Path.Combine(folder, "Fishware");

        static string defaultlocation = System.IO.Path.Combine(specificFolder, "csgoaccounts.mdf");

        static string cn_string = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename =" + defaultlocation + "; Integrated Security = True";

        public static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            Assembly assembly = Assembly.GetCallingAssembly();

            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
            {
                w.Write(r.ReadBytes((int)s.Length));
            }
        }

        public static void InitilizeDB()
        {


            Directory.CreateDirectory(specificFolder);


            if (!File.Exists($"{specificFolder}\\csgoaccounts.mdf"))
            {
                Extract("Fishware_v5", specificFolder, "DataEntry", "csgoaccounts.mdf");
            }
        }

        public static void AddAccounts(string DisplayName, string UserName, string Password,int Wins, int Level, string ProfileURL, string Pfp)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cn_string))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    cmd.CommandText = "INSERT INTO accounts VALUES (@DisplayName, @UserName, @Password, @Wins, @Level, @ProfileURL, @ProfilePic)";
                    cmd.Parameters.AddWithValue("@DisplayName", DisplayName);
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", Password);
                    cmd.Parameters.AddWithValue("@Wins", Wins);
                    cmd.Parameters.AddWithValue("@Level", Level);
                    cmd.Parameters.AddWithValue("@ProfileURL", ProfileURL);
                    cmd.Parameters.AddWithValue("@ProfilePic", Pfp);

                    cmd.ExecuteReader();

                    conn.Close();
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString(), "SQL Error");
            }
        }

        public static ObservableCollection<Accounts> GetData()
        {
            Tabs.TabView.account.Clear();

            ObservableCollection<Accounts> accounts = new ObservableCollection<Accounts>();
            using (SqlConnection conn = new SqlConnection(cn_string))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM accounts", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int ID = reader.GetInt32(0);
                        string Displayname = reader.GetString(1);
                        string Username = reader.GetString(2);
                        int Wins = reader.GetInt32(4);
                        int Levels = reader.GetInt32(5);
                        string ProfileURL = reader.GetString(6);
                        string ProfilePic = reader.GetString(7);

                        Tabs.TabView.account.Add(new Accounts(ID, Displayname, Username, Wins, Levels, ProfileURL, ProfilePic));
                        accounts.Add(new Tabs.TabView.Accounts(ID, Displayname, Username, Wins, Levels, ProfileURL, ProfilePic));
                    }
                    conn.Close();
                }
            }
            return accounts;
        }

        public static void ReadPassword(int ID)
        {
            using (SqlConnection conn = new SqlConnection(cn_string))
            using (SqlCommand cmd = new SqlCommand("SELECT Password FROM accounts where ID = @ID;", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@ID", ID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string password = reader.GetString(0);
                        PasswordManager.Passphrase = password;
                    }
                    conn.Close();
                }
            }
        }

        public static void DeleteEntry(int ID)
        {
            using (SqlConnection conn = new SqlConnection(cn_string))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM accounts where ID = @ID;", conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@ID", ID);
                cmd.ExecuteReader();
                conn.Close();
            }
        }

        public static void ResetTable()
        {
            using (SqlConnection conn = new SqlConnection(cn_string))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM accounts;", conn))
            {
                conn.Open();
                cmd.ExecuteReader();
                conn.Close();
            }
        }
    }
}
