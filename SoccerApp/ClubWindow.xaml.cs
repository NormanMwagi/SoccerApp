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
    public partial class ClubWindow : Window
    {
        private DataTable dt;
        public string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

        public ClubWindow()
        {
            InitializeComponent();
            dt = new DataTable();
            getData();
        }

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            string clubName = clubNameTxt.Text;
            string cityName = cityNameTxt.Text;
            long leagueId = Convert.ToInt64(leagueComboBox.SelectedValue); // Get selected league ID

            try
            {
                Int64 i = 0;
                string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("add_club", sqlConnection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter idParam = new SqlParameter("@id", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);
                    cmd.Parameters.AddWithValue("@club_name", clubName);
                    cmd.Parameters.AddWithValue("@city", cityName);
                    cmd.Parameters.AddWithValue("@league_id", leagueId);
                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        if (cmd.Parameters["@id"].Value != DBNull.Value)
                        {
                            long league_Id = (long)cmd.Parameters["@id"].Value;
                            MessageBox.Show($"Data added with ID: {league_Id}");
                        }
                        else
                        {
                            MessageBox.Show("Data inserted successfully  but no id returned");
                        }
                        // Clear the input fields
                        clubNameTxt.Text = "";
                        cityNameTxt.Text = "";
                        getData();
                    }
                    else
                    {
                        MessageBox.Show("Data insert failed ");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message} ");
            }
        }
        // Event handler for Edit button click
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)clubDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                // Assuming 'id' is the name of your ID column in the DataTable
                long clubId = Convert.ToInt64(selectedRow["id"]);
                string clubName = selectedRow["club_name"].ToString();
                string cityName = selectedRow["city"].ToString();
                string leagueName = selectedRow["league_name"].ToString();
                // Open the edit window and pass the selected league's details
                EditClubWindow editWindow = new EditClubWindow(clubId, clubName, cityName, leagueName);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing
                getData();
            }
        }
        // Event handler for Delete button click
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)clubDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                long leagueId = Convert.ToInt64(selectedRow["id"]);

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("delete_club", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", leagueId);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Club deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the Club.");
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
                // Fetch leagues from the database
                SqlCommand leagueCmd = new SqlCommand("get_league", sqlConnection);
                leagueCmd.CommandType = CommandType.StoredProcedure;
                sqlConnection.Open();
                SqlDataReader reader = leagueCmd.ExecuteReader();

                DataTable leagueTable = new DataTable();
                leagueTable.Load(reader);

                // Bind leagues to the ComboBox
                leagueComboBox.ItemsSource = leagueTable.DefaultView;
                leagueComboBox.SelectedValuePath = "id";
                leagueComboBox.DisplayMemberPath = "league_name";
            }

            // Bind the DataTable to the DataGrid as before
            SqlConnection connection = new SqlConnection(ConnString);
            SqlCommand clubCmd = new SqlCommand("get_club", connection);
            SqlDataAdapter da = new SqlDataAdapter(clubCmd);
            dt.Clear(); // Clear the existing data
            da.Fill(dt); // Fill the DataTable with new data

            clubDataGrid.ItemsSource = dt.DefaultView; // Bind the DataTable to the DataGrid
        }


    }
}