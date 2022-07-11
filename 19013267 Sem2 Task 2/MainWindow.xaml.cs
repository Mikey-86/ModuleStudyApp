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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Threading;

namespace _19013267_Sem2_Task_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int loginID = 0;
        //Connection string
        SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=N:\\Visual Studio Program Files\\19013267 Sem2 Task 2\\19013267 Sem2 Task 2\\Database1.mdf;Integrated Security=True");

        public MainWindow()
        {
            InitializeComponent();
        }

        //First Button (Module details on left)
        private void btnEnterModDetails_Click(object sender, RoutedEventArgs e)
        {
            //Variable to store study hours before moving to database
            int studyHours = _19013267_POE_Library.Class1.studyPerWeek(Convert.ToInt32(txtCreds.Text), Convert.ToInt32(txtClassHours.Text), Convert.ToInt32(txtWeeks.Text));
           
            //Taking textbox details into database
            //Mod name and loginID cannot be null
            SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Main](code,modName, credits, classHours, weeks, startDate, studyHoursLeft,LoginID) values(@code,@modName, @credits, @classHours, @weeks, @startDate, @studyHoursLeft, @LoginID)", conn);
            cmd.Parameters.AddWithValue("@code",  txtModCode.Text);
            cmd.Parameters.AddWithValue("@modName", txtModName.Text.ToUpper());
            cmd.Parameters.AddWithValue("@credits", Convert.ToInt32(txtCreds.Text));
            cmd.Parameters.AddWithValue("@classHours", Convert.ToInt32(txtClassHours.Text));
            cmd.Parameters.AddWithValue("@weeks", Convert.ToInt32(txtWeeks.Text));
            cmd.Parameters.AddWithValue("@startDate", txtStartDate.Text);
            cmd.Parameters.AddWithValue("@studyHoursLeft", studyHours);
            cmd.Parameters.AddWithValue("@LoginID", loginID);

            try
            {
                if(conn.State == System.Data.ConnectionState.Closed)
                //Opening connection string
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Module added sucessfully!");
                conn.Close();

            }
            catch (Exception)
            {
                MessageBox.Show("Error occured, please try again!");
            }
        }

        //Second Button (Study time)
        private void btnEnterStudyAmounts_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=N:\\Visual Studio Program Files\\19013267 Sem2 Task 2\\19013267 Sem2 Task 2\\Database1.mdf;Integrated Security=True");

            //Inserting values into database (Study Table)
            SqlCommand cmd = new SqlCommand("INSERT INTO [dbo].[Study](studyDate, moduleName, studyHours) values(@studyDate, @moduleName, @studyHours)", conn);
            cmd.Parameters.AddWithValue("@studyDate", txtStudyDate.Text);
            cmd.Parameters.AddWithValue("@moduleName", txtModCompareName.Text.ToUpper());
            cmd.Parameters.AddWithValue("@studyHours", Convert.ToInt32(txtHoursStudied.Text));

            //Opening connection string
            conn.Open();
            cmd.ExecuteNonQuery();
            MessageBox.Show("Study hours added successfully!");
            conn.Close();

            //Storing column in variable
            string workModuleName;
            SqlCommand moduleNameQuery = new SqlCommand("SELECT moduleName FROM [dbo].[Study] WHERE [dbo].[Study].[moduleName] = @mod", conn);
            moduleNameQuery.Parameters.AddWithValue("@mod", txtModCompareName.Text.ToUpper());
            conn.Open();
            workModuleName = moduleNameQuery.ExecuteScalar().ToString();

            //query to get total study hours
            int totalStudyhours;
            SqlCommand cmdCalc = new SqlCommand("SELECT studyHoursLeft FROM [dbo].[Main] WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
            cmdCalc.Parameters.AddWithValue("@loginIdentity", loginID);
            cmdCalc.Parameters.AddWithValue("@moduleName", workModuleName);
            totalStudyhours = Convert.ToInt32(cmdCalc.ExecuteScalar());
            conn.Close();

            //Calculating after study hours
            totalStudyhours = totalStudyhours - Convert.ToInt32(txtHoursStudied.Text);

            //Updating table with new amount
            if (totalStudyhours != 0 || totalStudyhours <0)
            {
                conn.Open();
                SqlCommand updateQuery = new SqlCommand("UPDATE [dbo].[Main] SET [dbo].[Main].[studyHoursLeft] = @totalStudyHours WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
                updateQuery.Parameters.AddWithValue("@totalStudyHours", totalStudyhours);
                updateQuery.Parameters.AddWithValue("@loginIdentity", loginID);
                updateQuery.Parameters.AddWithValue("@moduleName", workModuleName);

                updateQuery.ExecuteNonQuery();
                MessageBox.Show("Study hours removed!");
                conn.Close();

                //Commands for adding to data grid 2 (bottom datagrid)
                SqlCommand dgCmd = new SqlCommand("SELECT modName, studyHoursLeft FROM [dbo].[Main] WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
                dgCmd.Parameters.AddWithValue("@loginIdentity", loginID);
                dgCmd.Parameters.AddWithValue("@moduleName", workModuleName);
                SqlDataAdapter sda = new SqlDataAdapter(dgCmd);

                using (conn)
                {
                    DataTable dt = new DataTable("Module Study Time Left");
                    sda.Fill(dt);
                    dgHours.ItemsSource = dt.DefaultView;
                }

            }
            else
            {
                MessageBox.Show("Total hours for this module complete for the week! Well done!\nPress the reset button to start a new week");
                //Displaying no hours left
                totalStudyhours = 0;
                conn.Open();
                SqlCommand updateQuery = new SqlCommand("UPDATE [dbo].[Main] SET [dbo].[Main].[studyHoursLeft] = @totalStudyHours WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
                updateQuery.Parameters.AddWithValue("@totalStudyHours", totalStudyhours);
                updateQuery.Parameters.AddWithValue("@loginIdentity", loginID);
                updateQuery.Parameters.AddWithValue("@moduleName", workModuleName);

                updateQuery.ExecuteNonQuery();
                MessageBox.Show("Study hours removed!");
                conn.Close();

                SqlCommand dgZero = new SqlCommand("SELECT modName, studyHoursLeft FROM [dbo].[Main] WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
                dgZero.Parameters.AddWithValue("@loginIdentity", loginID);
                dgZero.Parameters.AddWithValue("@moduleName", workModuleName);
                SqlDataAdapter sda = new SqlDataAdapter(dgZero);

                using (conn)
                {
                    DataTable dt = new DataTable("Module Study Time Left");
                    sda.Fill(dt);
                    dgHours.ItemsSource = dt.DefaultView;
                }
                //Clearing textboxes
                txtHoursStudied.Clear();
                txtModCompareName.Clear();
                txtStudyDate.Clear();
                resetHours();
            }

        }

        //Setter method to bring in ID from login table
        public void setID(int id)
        {
            loginID = id;
        }

        //Button under top data grid
        private void btnShowModules_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=N:\\Visual Studio Program Files\\19013267 Sem2 Task 2\\19013267 Sem2 Task 2\\Database1.mdf;Integrated Security=True");
            conn.Open();
            string query = ("SELECT modName, studyHoursLeft FROM [dbo].[Main] WHERE @loginIdentity = [dbo].[Main].[LoginId]");
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@loginIdentity", loginID);

            using (conn)
            {                
                DataTable dt = new DataTable("Modules");
                sda.Fill(dt);
                dgModules.ItemsSource = dt.DefaultView;
            }
            conn.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void resetHours()
        {
            //Decalartions
            int creds;
            int classHours;
            int weeks;
            int totalStudyHours;
            SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=N:\\Visual Studio Program Files\\19013267 Sem2 Task 2\\19013267 Sem2 Task 2\\Database1.mdf;Integrated Security=True");

            //Sql commands to get these 3 values
            //credits
            conn.Open();
            SqlCommand credits = new SqlCommand("SELECT credits FROM [dbo].[Main] WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
            credits.Parameters.AddWithValue("@loginIdentity", loginID);
            credits.Parameters.AddWithValue("@moduleName", txtResetModName.Text.ToUpper());
            creds = Convert.ToInt32(credits.ExecuteScalar());
            conn.Close();

            //classHours
            conn.Open();
            SqlCommand classHoursCmd = new SqlCommand("SELECT classHours FROM [dbo].[Main] WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
            classHoursCmd.Parameters.AddWithValue("@loginIdentity", loginID);
            classHoursCmd.Parameters.AddWithValue("@moduleName", txtResetModName.Text.ToUpper());
            classHours = Convert.ToInt32(classHoursCmd.ExecuteScalar());
            conn.Close();

            //weeks
            conn.Open();
            SqlCommand weeksCmd = new SqlCommand("SELECT weeks FROM [dbo].[Main] WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
            weeksCmd.Parameters.AddWithValue("@loginIdentity", loginID);
            weeksCmd.Parameters.AddWithValue("@moduleName", txtResetModName.Text.ToUpper());
            weeks = Convert.ToInt32(weeksCmd.ExecuteScalar());
            conn.Close();

            //Updating total study value with library method
            totalStudyHours = _19013267_POE_Library.Class1.studyPerWeek(creds, classHours, weeks);

            //Commands to update table
            conn.Open();
            SqlCommand workModule = new SqlCommand("UPDATE [dbo].[Main] SET [dbo].[Main].[studyHoursLeft] = @totalStudyHours WHERE @loginIdentity = [dbo].[Main].[LoginId] AND @moduleName = [dbo].[Main].[modName]", conn);
            workModule.Parameters.AddWithValue("@totalStudyHours", totalStudyHours);
            workModule.Parameters.AddWithValue("@loginIdentity", loginID);
            workModule.Parameters.AddWithValue("@moduleName", txtResetModName.Text.ToUpper());
            workModule.ExecuteNonQuery();
            MessageBox.Show("Hours have been reset for " + txtResetModName.Text.ToUpper());
            conn.Close();
        }

        //Reset button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            resetHours();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            Close();
        }
    }
}
