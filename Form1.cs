using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Runtime.InteropServices;

namespace MIP_Project1
{
    public partial class Form1 : Form
    {

//------------------------------- sql database handler ----------------------------------------

        public class SQLiteHandler
        {
            private static SQLiteHandler _instance;
            public SQLiteConnection DBconnect;

            private SQLiteHandler()
            {
                Console.WriteLine("Database connection established");
            }

            public static SQLiteHandler getInstance()
            {
                if(_instance == null)
                {
                    _instance = new SQLiteHandler();
                }
                return _instance;
            }

            public void ConnectToDB(string path)
            {
                var dbpath = "BlockedKeywords.db";
                if (!File.Exists(dbpath))
                {
                    SQLiteConnection.CreateFile(dbpath);

                    using (var con = new SQLiteConnection(@"Data Source=BlockedKeywords.db;Version=3;"))
                    {
                        con.Open();

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Keywords (id INTEGER PRIMARY KEY AUTOINCREMENT, keyword TEXT NOT NULL UNIQUE);";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else 
                {
                    DBconnect = new SQLiteConnection(@"Data Source=BlockedKeywords.db;Version=3;");
                    DBconnect.Open();
                }
            }

            public void DisconnectFromDB()
            {
                DBconnect.Close();
            }

            public void AddKeyword(string k)
            {
                using (var cmd = DBconnect.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Keywords (keyword) VALUES (@k)";
                    cmd.Parameters.AddWithValue("@k", k);
                    cmd.ExecuteNonQuery();

                }
            }

            public List<string> GetAllKeywords()
            {
                List<string> keywords = new List<string>();
                using (var cmd = DBconnect.CreateCommand())
                {
                    cmd.CommandText = "SELECT keyword FROM Keywords";
                    using (var reader = cmd.ExecuteReader()) {
                        while(reader.Read())
                        {
                            keywords.Add(reader.GetString(0));
                        }
                    }
                }

                return keywords;
            }

            public void DeleteKeyword(string k) 
            {
                using (var cmd = DBconnect.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Keywords WHERE keyword = @k";
                    cmd.Parameters.AddWithValue("@k", k);
                    cmd.ExecuteNonQuery();
                }
            }

        }

        SQLiteHandler database = SQLiteHandler.getInstance();

// --------------------------------------------------------------------------------------------------
       
        public Form1()
        {
            InitializeComponent();

            database.ConnectToDB("BlockedKeywords.db");
            List<string> keywords = database.GetAllKeywords();
            for (int i = 0; i < keywords.Count; i++)
            {
                toolStripComboBox.Items.Add(keywords[i]);
            }
            keywords.Clear();
            keywords = null;
            database.DisconnectFromDB();
        }

        private void tsbGO_Click(object sender, EventArgs e)
        {
            if (!tstb.Text.StartsWith("https://"))
            {
                tstb.Text = "https://" + tstb.Text;
            }
            errorProvider.SetError(tstb.Control, "");
            if (tstb.Text == "" || tstb.Text == "https://")
            {
                errorProvider.SetError(tstb.Control, "Field must not be empty");
            }
            string url = tstb.Text;

            webBrowser.Navigate(url);
        }

        private void tsbForward_Click(object sender, EventArgs e)
        {
            webBrowser.GoForward();
        }

        private void tsbBack_Click(object sender, EventArgs e)
        {
            webBrowser.GoBack();
        }

        private void tsbHome_Click(object sender, EventArgs e)
        {
            webBrowser.GoHome();
        }

        private void tstb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (!tstb.Text.StartsWith("https://"))
                {
                    tstb.Text = "https://" + tstb.Text;
                }
                errorProvider.SetError(tstb.Control, "");

                if (tstb.Text == "" || tstb.Text == "https://")
                {
                    errorProvider.SetError(tstb.Control, "Field must not be empty");
                }

                string url = tstb.Text;

                webBrowser.Navigate(url);
            }
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            database.ConnectToDB("BlockedKeywords.db");
            List<string> keywords = database.GetAllKeywords();
            for (int i = 0; i < keywords.Count(); i++)
            {
                if (e.Url.ToString().Contains(keywords[i]))
                {
                    e.Cancel = true;
                    MessageBox.Show("Site is blacklisted");
                }
            }
            keywords.Clear();
            keywords = null;
        }

        private void keywordAddButton_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.ShowDialog(this);
            if(frm2.DialogResult == DialogResult.Cancel)
            {
                frm2.Close();
            }
            if(frm2.DialogResult == DialogResult.OK)
            {
                string form2text = frm2.getText();
                if (form2text != "")
                {
                    toolStripComboBox.Items.Add(form2text);
                    database.ConnectToDB("BlockedKeywords.db");
                    database.AddKeyword(form2text);
                    database.DisconnectFromDB();
                }
                frm2.Close();
            }
        }

        private void toolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            deleteKeywordButton.Visible = true;
        }

        private void deleteKeywordButton_Click(object sender, EventArgs e)
        {
            database.ConnectToDB("BlockedKeywords.db");
            List<string> keywords = database.GetAllKeywords();
            for (int i = 0; i < keywords.Count; i++) 
            {
                if (keywords[i] == toolStripComboBox.Text)
                {
                    database.DeleteKeyword(keywords[i]);
                    toolStripComboBox.Items.Remove(toolStripComboBox.Text);
                }
            }
            keywords.Clear();
            keywords = null;
            deleteKeywordButton.Visible = false;
        }



    }
}
