using Microsoft.Data.SqlClient;
using System.Data;
using System.Windows;

namespace SoccerApp
{
    public partial class EditClubWindow : Window
    {
        public long id { get; set; }
        public string club_name { get; set; }
        public string city { get; set; }
        public string league_name { get; set; }

        public EditClubWindow(long clubId, string clubName, string cityName, string leagueName)
        {
            InitializeComponent();
            id = clubId;
            club_name = clubName;
            city = cityName;
            leagueComboBox.SelectedValue = leagueName;
            clubNameTextBox.Text = club_name;
            cityNameTextBox.Text = city;
            LoadLeagues();
        }
        private void LoadLeagues()
        {
            try
            {
                string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT id, league_name FROM league", sqlConnection);

                    sqlConnection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    DataTable clubTable = new DataTable();
                    clubTable.Load(reader);

                    // Bind clubs to the ComboBox
                    leagueComboBox.ItemsSource = clubTable.DefaultView;
                    leagueComboBox.SelectedValuePath = "id";
                    leagueComboBox.DisplayMemberPath = "league_name";

                    // Set the selected club in the combo box
                    leagueComboBox.SelectedValue = club_name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading clubs: {ex.Message}");
            }
        }



        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string clubName = clubNameTextBox.Text;
            string cityName = cityNameTextBox.Text;
            long leagueId = 0; // Declare leagueId outside of the if block

            if (long.TryParse(leagueComboBox.SelectedValue?.ToString(), out long parsedLeagueId))
            {
                leagueId = parsedLeagueId;
            }
            else
            {
                MessageBox.Show("Invalid league ID format.");
                return;
            }


            try
            {
                string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("update_club", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@club_name", clubName);
                    cmd.Parameters.AddWithValue("@city", cityName);
                    cmd.Parameters.AddWithValue("@league_id", leagueId);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Data updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("No data was updated. Please check the club ID and try again.");
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
