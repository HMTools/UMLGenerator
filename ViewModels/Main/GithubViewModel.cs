using MVVMLibrary.ViewModels;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
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

        #endregion Commands

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

        public ObservableCollection<object> Repos { get; set; } = new ObservableCollection<object>();

        #endregion Properties

        #region Fields

        private readonly MainViewModel mainVM;
        private string lastWorkedToken;

        #endregion Fields

        #region Constructors

        public GithubViewModel(MainViewModel mainVM) : base(300, new GridLength(1, GridUnitType.Star), false)
        {
            this.mainVM = mainVM;
            Task.Run(LoadData);
        }

        #endregion Constructors

        #region Methods

        protected override void AddCommands()
        {
            base.AddCommands();
            UpdateTokenCommand = new RelayCommand(o => Task.Run(UpdateToken));
            GetSelectedRepositoryCommand = new RelayCommand(GetSelectedRepository);
        }

        private async Task LoadData()
        {
            ApiToken = ConfigurationManager.AppSettings["GitApiToken"];
            await SetClient();
        }

        private async Task UpdateToken()
        {
            if (ApiToken != ConfigurationManager.AppSettings["GitApiToken"])
            {
                if (await SetClient())
                {
                    Libraries.Configurations.AddOrUpdateAppSettings("GitApiToken", ApiToken);
                    mainVM.SetStatus($"Set Github Token | Succeed | Username: {Username}", Brushes.Blue, 2000);
                }
                else
                {
                    ApiToken = lastWorkedToken;
                    mainVM.SetStatus($"Set Github Token | Failed!", Brushes.Red, 3000);
                }
            }
        }

        private async Task<bool> SetClient()
        {
            if (!string.IsNullOrWhiteSpace(ApiToken))
            {
                var client = await GetGithubClient(ApiToken);
                if (client != null)
                {
                    lastWorkedToken = ApiToken;
                    GitClient = client;
                    Username = (await GitClient.User.Current()).Login;
                    await SetUserRepositories();
                    return true;
                }
            }
            return false;
        }

        private async Task SetUserRepositories()
        {
            await System.Windows.Application.Current.Dispatcher.Invoke(async () =>
            {
                Repos.Clear();
                foreach (var repo in await GitClient.Repository.GetAllForCurrent())
                {
                    Repos.Add(new { repo.Name, repo.Private, repo.Language, repo.Id });
                }
            });
        }

        private void GetSelectedRepository(dynamic repo)
        {
            IsShown = false;
            RepostioryID = repo.Id;
            mainVM.SourceVM.SourceType = SourceTypes.Github;
            mainVM.SourceVM.RepositoryName = repo.Name;
            mainVM.SourceVM.RepositoryOwner = username;
            mainVM.SourceVM.IsShown = true;
            Task.Run(() => mainVM.SourceVM.SetRootDir(""));
        }

        private async Task<GitHubClient> GetGithubClient(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var productInformation = new ProductHeaderValue("UMLGenerator");
                var credentials = new Credentials(token);
                var client = new GitHubClient(productInformation) { Credentials = credentials };
                try
                {
                    var user = await client.User.Current();
                    return client;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }

        #endregion Methods
    }
}