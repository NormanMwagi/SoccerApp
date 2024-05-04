using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace SoccerApp
{
    /// <summary>
    /// Interaction logic for EditPlayerWindow.xaml
    /// </summary>
    public partial class EditPlayerWindow : Window
    {
        public long id { get; set; }
        public string player_name { get; set; }
        public Int64 age { get; set; }
        public string position { get; set; }
        public string club_name { get; set; }

        public EditPlayerWindow(long playerId, string playerName, long ageNo, string position, string clubName)
        {
            InitializeComponent();
            id = playerId;
            player_name = playerName;
            age = ageNo;
            position = position;
            club_name = clubName;

            playerNameTxt.Text = player_name;
            ageTxt.Text = age.ToString(); // Set the age text box to the age value
            positionTxt.Text = position;

            // Load clubs from the database and set the selected club in the combo box
            LoadClubs();
        }

        // Method to load clubs from the database and set the selected club in the combo box
        private void LoadClubs()
        {
            try
            {
                string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT id, club_name FROM club", sqlConnection);

                    sqlConnection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    DataTable clubTable = new DataTable();
                    clubTable.Load(reader);

                    // Bind clubs to the ComboBox
                    clubComboBox.ItemsSource = clubTable.DefaultView;
                    clubComboBox.SelectedValuePath = "id";
                    clubComboBox.DisplayMemberPath = "club_name";

                    // Set the selected club in the combo box
                    clubComboBox.SelectedValue = club_name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading clubs: {ex.Message}");
            }
        }



        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string playerName = playerNameTxt.Text;
            string ageNo = ageTxt.Text;
            string positionNo = positionTxt.Text;

            if (long.TryParse(ageNo, out long ageValue))
            {
                try
                {
                    // Get the selected club ID using a safer conversion method
                    if (clubComboBox.SelectedValue is decimal selectedValueDecimal)
                    {
                        long clubId = Convert.ToInt64(selectedValueDecimal);

                        string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

                        using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                        {
                            SqlCommand cmd = new SqlCommand("update_player", sqlConnection);
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@player_name", playerName);
                            cmd.Parameters.AddWithValue("@age", ageValue); // Use the parsed age value
                            cmd.Parameters.AddWithValue("@position", positionNo);
                            cmd.Parameters.AddWithValue("@club_id", clubId);

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
                    else
                    {
                        MessageBox.Show("Invalid club ID format.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Invalid age format.");
            }

            this.DialogResult = true;
        }


    }
}