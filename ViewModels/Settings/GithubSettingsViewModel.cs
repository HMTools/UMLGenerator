using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
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

        public RelayCommand UpdateKeyCommand { get; private set; }

        #endregion

        #region Properties
        private string apiKey;

        public string ApiKey
        {
            get { return apiKey; }
            set { apiKey = value; NotifyPropertyChanged(); }
        }


        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; NotifyPropertyChanged(); }
        }

        private List<object> repos;

        public List<object> Repos
        {
            get { return repos; }
            set { repos = value; NotifyPropertyChanged(); }
        }


        #endregion

        #region Fields
        private readonly MainViewModel mainVM;
        #endregion

        #region Constructors
        public GithubSettingsViewModel(MainViewModel mainVM)
        {
            this.mainVM = mainVM;
            Repos = new List<object>();
            AddCommands();
            LoadData();
        }
        #endregion

        #region Methods
        private void AddCommands()
        {
            UpdateKeyCommand = new RelayCommand(o => 
            {
                if(!string.IsNullOrWhiteSpace(ApiKey) && ApiKey != ConfigurationManager.AppSettings["GitApiKey"])
                {
                    var client = mainVM.GetGithubClient(ApiKey);
                    if(client != null)
                    {
                        mainVM.GitClient = client;
                        Libraries.Configurations.AddOrUpdateAppSettings("GitApiKey", ApiKey);
                        Username = mainVM.GitClient.User.Current().GetAwaiter().GetResult().Login;

                        Repos.Clear();
                        foreach (var repo in mainVM.GitClient.Repository.GetAllForCurrent().GetAwaiter().GetResult())
                        {
                            Repos.Add(new { Name = repo.Name, Private = repo.Private, Language = repo.Language });
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to load api key!");
                    }
                }
            });
        }
        private void LoadData()
        {
            ApiKey = ConfigurationManager.AppSettings["GitApiKey"];
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
