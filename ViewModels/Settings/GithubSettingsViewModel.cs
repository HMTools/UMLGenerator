using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.Settings
{
    public class GithubSettingsViewModel : BaseViewModel
    {

        #region Commands

        public RelayCommand UpdateTokenCommand { get; private set; }

        #endregion

        #region Properties
        private string apiToken;

        public string ApiToken
        {
            get { return apiToken; }
            set { apiToken = value; NotifyPropertyChanged(); }
        }


        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<object> Repos { get; set; }


        #endregion

        #region Fields
        private readonly MainViewModel mainVM;
        #endregion

        #region Constructors
        public GithubSettingsViewModel(MainViewModel mainVM)
        {
            this.mainVM = mainVM;
            Repos = new ObservableCollection<object>();
            LoadData();
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            UpdateTokenCommand = new RelayCommand(o => 
            {
                if(!string.IsNullOrWhiteSpace(ApiToken) && ApiToken != ConfigurationManager.AppSettings["GitApiToken"])
                {
                    var client = mainVM.GetGithubClient(ApiToken);
                    if(client != null)
                    {
                        mainVM.GitClient = client;
                        Libraries.Configurations.AddOrUpdateAppSettings("GitApiToken", ApiToken);
                        Username = mainVM.GitClient.User.Current().GetAwaiter().GetResult().Login;

                        Repos.Clear();
                        foreach (var repo in mainVM.GitClient.Repository.GetAllForCurrent().GetAwaiter().GetResult())
                        {
                            Repos.Add(new { Name = repo.Name, Private = repo.Private, Language = repo.Language });
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to load api token!");
                    }
                }
            });
        }
        private void LoadData()
        {
            ApiToken = ConfigurationManager.AppSettings["GitApiToken"];
            if (mainVM.GitClient != null)
            {
                Username = mainVM.GitClient.User.Current().GetAwaiter().GetResult().Login;
                foreach(var repo in mainVM.GitClient.Repository.GetAllForCurrent().GetAwaiter().GetResult())
                {
                    Repos.Add(new { Name = repo.Name,  Private = repo.Private, Language = repo.Language });
                }
            }
        }
        #endregion
    }
}
