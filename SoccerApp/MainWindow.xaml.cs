using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SoccerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dt;
        public string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

        public MainWindow()
        {
            InitializeComponent();
            ClubWindow clubWindow = new ClubWindow();
            clubWindow.Show();
            PlayerWindow playerWindow = new PlayerWindow();
            playerWindow.Show();
            dt = new DataTable();
            getData();
        }

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            string leagueName = league_name.Text;
            string countryName = countryTxt.Text;
            try
            {
                Int64 i = 0;
                string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("add_league", sqlConnection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter idParam = new SqlParameter("@id", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);
                    cmd.Parameters.AddWithValue("@league_name", leagueName);
                    cmd.Parameters.AddWithValue("@country", countryName);
                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        if (cmd.Parameters["@id"].Value != DBNull.Value)
                        {
                            long leagueId = (long)cmd.Parameters["@id"].Value;
                            MessageBox.Show($"Data added with ID: {leagueId}");
                        }
                        else
                        {
                            MessageBox.Show("Data inserted successfully  but no id returned");
                        }
                        // Clear the input fields
                        league_name.Text = "";
                        countryTxt.Text = "";
                        getData();
                    }
                    else
                    {
                        MessageBox.Show("Data insert failed ");
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message} ");
            }
        }
        // Event handler for Edit button click
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)leagueDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                // Assuming 'id' is the name of your ID column in the DataTable
                long leagueId = Convert.ToInt64(selectedRow["id"]);
                string leagueName = selectedRow["league_name"].ToString();
                string countryName = selectedRow["country"].ToString();
                // Open the edit window and pass the selected league's details
                EditLeagueWindow editWindow = new EditLeagueWindow(leagueId, leagueName, countryName);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing
                getData();
            }
        }
                // Event handler for Delete button click
                private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)leagueDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                long leagueId = Convert.ToInt64(selectedRow["id"]);

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("delete_league", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", leagueId);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("League deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the league.");
                    }
                }

                // Refresh the DataGrid after deleting
                getData();
            }
        }
                

        // Update getData method to bind the DataTable to the DataGrid
        private void getData()
        {
            string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football;User ID=sa;Password=norman@2020;TrustServerCertificate=True;MultipleActiveResultSets=true";

            using (SqlConnection sqlConnection = new SqlConnection(ConnString))
            {
                SqlCommand cmd = new SqlCommand("get_league", sqlConnection);
                sqlConnection.Open();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt.Clear(); // Clear the existing data
                da.Fill(dt); // Fill the DataTable with new data
            }

            leagueDataGrid.ItemsSource = dt.DefaultView; // Bind the DataTable to the DataGrid
        }

    }
}