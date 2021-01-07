using MVVMLibrary.ViewModels;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.ViewModels.Main;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Commands

        public RelayCommand SourceSelectionCommand { get; private set; }
        public RelayCommand PlantUMLCommand { get; private set; }
        public RelayCommand SettingsCommand { get; private set; }
        #endregion

        #region Properties
        public SourceViewModel sourceViewModel { get; set; }
        public UMLViewModel umlViewModel { get; set; }

        private BaseViewModel selectedViewModel;

        public BaseViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; NotifyPropertyChanged(); }
        }

        public GitHubClient GitClient { get; set; }
        public long RepostioryID { get; set; }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            sourceViewModel = new SourceViewModel(this);
            SelectedViewModel = sourceViewModel;
            AddCommands();
            LoadData();
        }
        #endregion

        #region Methods
        private void AddCommands()
        {
            SourceSelectionCommand = new RelayCommand((o) => 
            {
                SelectedViewModel = sourceViewModel;
            });

            PlantUMLCommand = new RelayCommand((o) =>
            {
                SelectedViewModel = new UMLViewModel(this, sourceViewModel.GetCheckedFileModels(sourceViewModel.RootDir));
            }, (o) => sourceViewModel.RootDir != null);

            SettingsCommand = new RelayCommand((o) =>
            {
               new Views.Settings.SettingsView() { DataContext = new Settings.SettingsViewModel(this) }.ShowDialog();
            });
        }

        private void LoadData()
        {
            GitClient = GetGithubClient(ConfigurationManager.AppSettings["GitApiKey"]);
        }

        public GitHubClient GetGithubClient(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                var productInformation = new ProductHeaderValue("UMLGenerator");
                var credentials = new Credentials(key);
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
