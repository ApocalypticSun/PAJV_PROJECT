using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace Launcher_Proiect
{
    public partial class MainWindow : Window
    {
        private string playFabId;
        //D57EBD0D4BFB4FC2
        //723434C628E9B976
        string customId = "";

        public MainWindow()
        {
            InitializeComponent();

       
            PlayFabSettings.staticSettings.TitleId = "E14D9";
        }

        private async void Login(object sender, RoutedEventArgs e)
        {

            customId = UserNameBox.Text;

            var request = new LoginWithCustomIDRequest
            {
                CustomId = customId,
                CreateAccount = false 
            };

            try
            {
                var result = await PlayFabClientAPI.LoginWithCustomIDAsync(request);

                if (result.Error != null)
                {
                    MessageBox.Show($"Login failed: {result.Error.GenerateErrorReport()}");
                    return;
                }

                playFabId = result.Result.PlayFabId;
                MessageBox.Show($"Login successful! PlayFabId: {playFabId}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception during login: {ex.Message}");
            }
        }

        private void Launch(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(playFabId))
            {
                MessageBox.Show("You must log in first!");
                return;
            }

            try
            {

                string gamePath = @"";


                string parameter = customId;

                ProcessStartInfo startInfo = new ProcessStartInfo(gamePath)
                {
                    UseShellExecute = true,
                    Arguments = $"\"{parameter}\""
                };

                Process.Start(startInfo);

         
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch game: {ex.Message}");
            }
        }

        private void UserNameBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
