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
    public partial class PlayerWindow : Window
    {
        private DataTable dt;
        public string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

        public PlayerWindow()
        {
            InitializeComponent();
            dt = new DataTable();
            getData();
        }

        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            string playerName = playerNameTxt.Text;
            string playerAge = ageTxt.Text;
            string positions = positionTxt.Text;
            long clubId = Convert.ToInt64(clubComboBox.SelectedValue); // Get selected club ID

            try
            {
                Int64 i = 0;
                string ConnString = "Server=DESKTOP-GM2U4UN\\SQLEXPRESS;Database=Football; User ID = sa; Password = norman@2020; TrustServerCertificate=True; MultipleActiveResultSets=true";

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("add_player", sqlConnection);

                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter idParam = new SqlParameter("@id", SqlDbType.BigInt)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idParam);
                    cmd.Parameters.AddWithValue("@player_name", playerName);
                    cmd.Parameters.AddWithValue("@age", playerAge);
                    cmd.Parameters.AddWithValue("@position", positions);
                    cmd.Parameters.AddWithValue("@club_id", clubId);
                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        if (cmd.Parameters["@id"].Value != DBNull.Value)
                        {
                            long player_Id = (long)cmd.Parameters["@id"].Value;
                            MessageBox.Show($"Data added with ID: {player_Id}");
                        }
                        else
                        {
                            MessageBox.Show("Data inserted successfully  but no id returned");
                        }
                        // Clear the input fields
                        playerNameTxt.Text="";
                        ageTxt.Text ="";
                        positionTxt.Text = "";
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
            DataRowView selectedRow = (DataRowView)playerDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                // Assuming 'id' is the name of your ID column in the DataTable
                long playerId = Convert.ToInt64(selectedRow["id"]);
                string playerName = selectedRow["player_name"].ToString();
                long ageNo = Convert.ToInt64(selectedRow["age"]);
                string position = selectedRow["position"].ToString();
                string clubName = selectedRow["club_name"].ToString();
                // Open the edit window and pass the selected league's details
                EditPlayerWindow editWindow = new EditPlayerWindow(playerId, playerName, ageNo, position, clubName);
                editWindow.ShowDialog();

                // Refresh the DataGrid after editing
                getData();
            }
        }
        // Event handler for Delete button click
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected item
            DataRowView selectedRow = (DataRowView)playerDataGrid.SelectedItem;
            if (selectedRow != null)
            {
                long leagueId = Convert.ToInt64(selectedRow["id"]);

                using (SqlConnection sqlConnection = new SqlConnection(ConnString))
                {
                    SqlCommand cmd = new SqlCommand("delete_player", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", leagueId);

                    sqlConnection.Open();
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        MessageBox.Show("Player deleted successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete the Player.");
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
                // Fetch clubs from the database
                SqlCommand clubCmd = new SqlCommand("get_club", sqlConnection);
                clubCmd.CommandType = CommandType.StoredProcedure;
                sqlConnection.Open();
                SqlDataReader reader = clubCmd.ExecuteReader();

                DataTable clubTable = new DataTable();
                clubTable.Load(reader);

                // Bind clubs to the ComboBox
                clubComboBox.ItemsSource = clubTable.DefaultView;
                clubComboBox.SelectedValuePath = "id";
                clubComboBox.DisplayMemberPath = "club_name";
            }

            // Bind the DataTable to the DataGrid as before
            SqlConnection connection = new SqlConnection(ConnString);
            SqlCommand playerCmd = new SqlCommand("get_player", connection);
            SqlDataAdapter da = new SqlDataAdapter(playerCmd);
            dt.Clear(); // Clear the existing data
            da.Fill(dt); // Fill the DataTable with new data

            playerDataGrid.ItemsSource = dt.DefaultView; // Bind the DataTable to the DataGrid
        }


    }
}