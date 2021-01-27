using MVVMLibrary.ViewModels;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.Main
{
    public class GithubViewModel : BaseGridColumnViewModel
    {

        #region Commands
        public RelayCommand UpdateTokenCommand { get; private set; }
        public RelayCommand GetSelectedRepositoryCommand { get; private set; }

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
        public GitHubClient GitClient { get; set; }
        public long RepostioryID { get; set; }

        public ObservableCollection<object> Repos { get; set; }
        #endregion

        #region Fields
        private readonly MainViewModel mainVM;
        private string lastWorkedToken;
        #endregion

        #region Constructors
        public GithubViewModel(MainViewModel mainVM) : base(300, new GridLength(1, GridUnitType.Star), false)
        {
            this.mainVM = mainVM;
            Repos = new ObservableCollection<object>();
            LoadData();
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            UpdateTokenCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrWhiteSpace(ApiToken) && ApiToken != ConfigurationManager.AppSettings["GitApiToken"])
                {
                    var client = GetGithubClient(ApiToken);
                    if (client != null)
                    {
                        lastWorkedToken = ApiToken;
                        GitClient = client;
                        Libraries.Configurations.AddOrUpdateAppSettings("GitApiToken", ApiToken);
                        Username = GitClient.User.Current().GetAwaiter().GetResult().Login;

                        Repos.Clear();
                        foreach (var repo in GitClient.Repository.GetAllForCurrent().GetAwaiter().GetResult())
                        {
                            Repos.Add(new { repo.Name, repo.Private, repo.Language , repo.Id});
                        }
                        mainVM.SetStatus($"Set Github Token | Succeed | Username: {Username}", Brushes.Blue, 2000);
                    }
                    else
                    {
                        ApiToken = lastWorkedToken;
                        mainVM.SetStatus($"Set Github Token | Failed!", Brushes.Red, 3000);
                    }
                }
            });
            GetSelectedRepositoryCommand = new RelayCommand(o => 
            {
                IsShown = false;
                dynamic repo = o;
                RepostioryID = repo.Id;
                mainVM.SourceVM.SourceType = SourceTypes.Github;
                mainVM.SourceVM.RepositoryName = repo.Name;
                mainVM.SourceVM.RepositoryOwner = username;
                mainVM.SourceVM.IsShown = true;
                mainVM.SourceVM.GetRepositoryCommand.Execute(null);
            });
        }
        private void LoadData()
        {
            ApiToken = ConfigurationManager.AppSettings["GitApiToken"];
            GitClient = GetGithubClient(ApiToken);
            if (GitClient != null)
            {
                lastWorkedToken = ApiToken;
                Username = GitClient.User.Current().GetAwaiter().GetResult().Login;
                foreach (var repo in GitClient.Repository.GetAllForCurrent().GetAwaiter().GetResult())
                {
                    Repos.Add(new { repo.Name, repo.Private, repo.Language, repo.Id });
                }
            }
        }
        private static GitHubClient GetGithubClient(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var productInformation = new ProductHeaderValue("UMLGenerator");
                var credentials = new Credentials(token);
                var client = new GitHubClient(productInformation) { Credentials = credentials };
                try
                {
                    var user = client.User.Current().GetAwaiter().GetResult();
                    return client;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Bad credentials")
                    {
                        return null;
                    }
                }
            }

            return null;
        }
        #endregion
    }
}
