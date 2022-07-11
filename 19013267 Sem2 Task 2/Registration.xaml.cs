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
using System.Data;
using System.Security.Cryptography;

namespace _19013267_Sem2_Task_2
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Connection String for database
                SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=N:\\Visual Studio Program Files\\19013267 Sem2 Task 2\\19013267 Sem2 Task 2\\Database1.mdf;Integrated Security=True");

                //Hashing
                var hasher = new SHA256Managed();
                var unhashed = System.Text.Encoding.Unicode.GetBytes(txtPassword.Password);
                var hashed = hasher.ComputeHash(unhashed);

                //SQL statements to insert entered values into Users table
                SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Users](Username, Password) values( @Username, @Password)", conn);
                cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                cmd.Parameters.AddWithValue("@Password", hashed);

                //Opening connection string
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Account created succesfully");
                conn.Close();

            }
            catch (Exception)
            {
                MessageBox.Show("Error occured, please try again!");
            }

            //Closing registration window and opening login page
            Login log = new Login();
            log.Show();
            Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            //To clear out textboxes
            txtUsername.Clear();
            txtPassword.Clear();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            //To return to Landing Page (Starting page)
            LandingPage lp = new LandingPage();
            lp.Show();
            Close();
        }
    }
}
