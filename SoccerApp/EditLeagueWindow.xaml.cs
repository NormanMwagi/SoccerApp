using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows;

namespace SoccerApp
{
    public partial class EditLeagueWindow : Window
    {
        public long id { get; set; }
        public string league_name { get; set; }
        public string country { get; set; }

        public EditLeagueWindow(long leagueId, string leagueName, string countryName)
        {
            InitializeComponent();
            id = leagueId;
            league_name = leagueName;
            country = countryName;
            leagueNameTextBox.Text = league_name;
            countryNameTextBox.Text = country;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string leagueName = leagueNameTextBox.Text;
            string countryName = countryNameTextBox.Text;

            try
            {
                string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("update_league", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@league_name", leagueName);
                    cmd.Parameters.AddWithValue("@country", countryName);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Data updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("No data was updated. Please check the league ID and try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            this.DialogResult = true;
        }

    }
}
