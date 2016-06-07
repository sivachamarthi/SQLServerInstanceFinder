using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace SQLServerInstanceFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsConnected = false;
        SqlConnectionStringBuilder connBuilder = null;
        public MainWindow()
        {
            InitializeComponent();


        }

        
        private async void btnFindInstances_Click(object sender, RoutedEventArgs e)
        {
            listBoxSQLServerInstance.Items.Clear();
            btnFindInstances.IsEnabled = false;
            prgSQLInstances.Visibility = Visibility.Visible;
            prgSQLInstances.IsIndeterminate = true;
            var slowTask = Task.Run(() => SqlServerInstanceList.Get());
            //this.Dispatcher.Invoke((Action)(() => { SqlServerInstanceList.Get()}));
            listBoxSQLServerInstance.Items.Add("Getting the server instances.....!");

            await slowTask;
            listBoxSQLServerInstance.Items.Clear();
            foreach (String item in slowTask.Result)
            {
                listBoxSQLServerInstance.Items.Add(item);
            }
            prgSQLInstances.IsIndeterminate = false;
            prgSQLInstances.Visibility = Visibility.Hidden;
            btnFindInstances.IsEnabled = true;
        }

        private void bindServerInstancesToListBox(List<String> _serverInstances)
        {
           
        }

        private  void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            string connError = "";

            if (TestConnection(ref connError))
            {
                MessageBox.Show("Connected successfully..!", "Success!");
            }
            else
            {
                if(!String.IsNullOrEmpty(connError))
                {
                    MessageBox.Show(connError, "Error!");
                }
            }
            btnLogin.IsEnabled = true;
        }

        
        private void btnGetDataBases_Click(object sender, RoutedEventArgs e)
        {
            string connError = "";
            if (!TestConnection(ref connError))
            {
                if (!String.IsNullOrEmpty(connError))
                {
                    MessageBox.Show(connError, "Error!");
                }
            }
            else
            {
                listBoxDatabases.Items.Clear();
                DataTable dtDatabases = new DataTable();
                using (SqlConnection conn = new SqlConnection(connBuilder.ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand("SELECT NAME FROM SYS.DATABASES", conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dtDatabases);
                        }
                    }
                }
                foreach (DataRow row in dtDatabases.Rows)
                {
                    listBoxDatabases.Items.Add(Convert.ToString(row["NAME"]));
                }
            }
        }

        /// <summary>
        /// Validates the fields in Form
        /// </summary>
        /// <returns></returns>
        private bool ValidateFields()
        {
            String dataSource = listBoxSQLServerInstance.SelectedItem != null ? listBoxSQLServerInstance.SelectedItem.ToString() : "";
            if (String.IsNullOrEmpty(dataSource))
            {
                MessageBox.Show("Please select an Instance..!", "Error!");
                return false;
            }
            else
            {
                String userName = txtUserName.Text;
                String password = txtPassword.Password;
                if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Username and password are required..!", "Error!");
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Used for testing if connection can be opened
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        private bool TestConnection(ref String error)
        {
            if (!ValidateFields())
            {
                return false;
            }

            String dataSource = listBoxSQLServerInstance.SelectedItem.ToString();
            String userName = txtUserName.Text;
            String password = txtPassword.Password;

            connBuilder = new SqlConnectionStringBuilder();
            connBuilder.DataSource = dataSource;
            connBuilder.UserID = userName;
            connBuilder.Password = password;
            using (SqlConnection conn = new SqlConnection(connBuilder.ConnectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    return false;
                }
            }
        }

        private void btnGetConnString_Click(object sender, RoutedEventArgs e)
        {
            string connError = "";
            if (!TestConnection(ref connError))
            {
                if (!String.IsNullOrEmpty(connError))
                {
                    MessageBox.Show(connError, "Error!");
                }
            }
            else
            {
                String databaseSelected = listBoxDatabases.SelectedItem != null ? listBoxDatabases.SelectedItem.ToString() : "";
                if (!String.IsNullOrEmpty(databaseSelected))
                {
                    connBuilder.InitialCatalog = databaseSelected;
                }
                txtConnectionString.Text = connBuilder.ConnectionString;
            }
        }

        private void listBoxSQLServerInstance_Change(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
