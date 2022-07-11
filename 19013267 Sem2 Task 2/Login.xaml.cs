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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace _19013267_Sem2_Task_2
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        int id;
        public Login()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            //Connection string for database
            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=N:\\Visual Studio Program Files\\19013267 Sem2 Task 2\\19013267 Sem2 Task 2\\Database1.mdf;Integrated Security=True");


            //Try block to open connection if it is closed
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();

                //Hashing
                var hasher = new SHA256Managed();
                var unhashed = System.Text.Encoding.Unicode.GetBytes(txtPassword.Password);
                var hashed = hasher.ComputeHash(unhashed);

                //Query to find enetered values against database values, returns the first found value in the database
                String query = "SELECT COUNT(1) FROM Users WHERE [dbo].[Users].[Username] = @Username AND [dbo].[Users].[Password] = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@Password", hashed);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                //Query to send id to mainwindow
                String queryid = "Select id from Users where Username = @Username AND Password = @Password";
                SqlCommand cmdID= new SqlCommand(queryid, conn);
                cmdID.CommandType = System.Data.CommandType.Text;
                cmdID.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmdID.Parameters.AddWithValue("@Password", hashed);
                id = Convert.ToInt32(cmdID.ExecuteScalar());

                //If account was found in database. Closes login page and opens main window
                if ( count == 1)
                {
                    MessageBox.Show("Authentication sucessful!");
                    MainWindow mw = new MainWindow();
                    mw.setID(id);
                    mw.Show();
                    Close();
                }
                //If account was not found in database
                else
                {
                    MessageBox.Show("Username or password incorrect");
                }
            }
            //If connection does not work
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        //Button to return to Landing page (Starting page)
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            LandingPage lp = new LandingPage();
            lp.Show();
            Close();
        }
    }
}
