using MVVMLibrary.ViewModels;
using Octokit;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UMLGenerator.Models.FileSystemModels;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.Main
{
    public class SourceViewModel : BaseGridColumnViewModel
    {
        #region Commands
        public RelayCommand SelectFolderCommand { get; private set; }
        public RelayCommand GetRepositoryCommand { get; private set; }
        public RelayCommand GetObjectsTreeCommand { get; private set; }
        
        #endregion
        #region Properties
        private SourceTypes sourceType;

        public SourceTypes SourceType
        {
            get { return sourceType; }
            set {
                if(value == SourceTypes.Github && mainVM.GithubVM.GitClient == null)
                {
                    MessageBox.Show("To use Github repositories as source for UML, first you need to asign a Github API Token!");
                    if (MessageBox.Show("Do you want to add your Github API Token now ?", "Github API", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        mainVM.GithubVM.IsShown = true;
                        if(mainVM.GithubVM.GitClient != null)
                        {
                            RootDir = null;
                            sourceType = value; NotifyPropertyChanged();
                        }
                    }
                    return;
                }
                else
                {
                    RootDir = null;
                    sourceType = value; NotifyPropertyChanged();
                }
                
            }
        }

        private SourceLanguages sourceLanguage;

        public SourceLanguages SourceLanguage
        {
            get { return sourceLanguage; }
            set { sourceLanguage = value; NotifyPropertyChanged(); }
        }

        private string repositoryOwner;

        public string RepositoryOwner
        {
            get { return repositoryOwner; }
            set { repositoryOwner = value; NotifyPropertyChanged(); }
        }

        private string repositoryName;

        public string RepositoryName
        {
            get { return repositoryName; }
            set { repositoryName = value; NotifyPropertyChanged(); }
        }

        private DirectoryModel rootDir;

        public DirectoryModel RootDir
        {
            get { return rootDir; }
            set { rootDir = value; NotifyPropertyChanged(); RefreshCommands(); }
        }
        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; NotifyPropertyChanged(); }
        }

        #endregion

        #region Fields
        private readonly MainViewModel mainVM;
        #endregion

        #region Constructors
        public SourceViewModel(MainViewModel mainVM) : base(300, new GridLength(1, GridUnitType.Star)) 
        {
            this.mainVM = mainVM;
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            SelectFolderCommand = new RelayCommand(o =>
            {
                var dialog = new VistaFolderBrowserDialog();
                if (dialog.ShowDialog() == true)
                {
                    Task.Run(() => SetRootDir(dialog.SelectedPath));
                 }
            });

            GetRepositoryCommand = new RelayCommand(o =>
            {
                Task.Run(async () => 
                {
                    mainVM.GithubVM.RepostioryID = (await mainVM.GithubVM.GitClient.Repository.Get(RepositoryOwner, RepositoryName)).Id;
                    await SetRootDir(""); 
                });
            }, o => !string.IsNullOrWhiteSpace(RepositoryOwner) && !string.IsNullOrWhiteSpace(RepositoryName));

            GetObjectsTreeCommand = new RelayCommand(o => 
            {
                mainVM.ObjectsTreeVM.IsShown = true;
                mainVM.UmlVM.IsShown = true;
                IsShown = false;
                Task.Run(async () => mainVM.ObjectsTreeVM.GenerateObjectsTree(await GetCheckedFileModels(RootDir)));
            }, o => RootDir != null && RootDir.Items.Any(item => item.IsChecked != false));
        }

        public async Task SetRootDir(string path)
        {
            IsLoading = true;
            RootDir = await GetDirectory(path);
            IsLoading = false;
        }

        private async Task<DirectoryModel> GetDirectory(string path)
        {
            var output = new DirectoryModel() { Name = Path.GetFileName(path), FullName = path };
            List<string> subDirs = new List<string>();
            if (SourceType == SourceTypes.Folder)
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    if (Path.GetExtension(file) == $".{mainVM.LanguagesVM.SelectedLanguage.FileExtension}")
                    {
                        output.Items.Add(new FileSystemItemModel() { Name = Path.GetFileName(file), FullName = file });
                    }
                }
                foreach (var dir in Directory.GetDirectories(path))
                {
                    subDirs.Add(dir);
                }
            }
            else
            {
                var contents = path == "" ?
                    await mainVM.GithubVM.GitClient.Repository.Content.GetAllContents(mainVM.GithubVM.RepostioryID) :
                    await mainVM.GithubVM.GitClient.Repository.Content.GetAllContents(mainVM.GithubVM.RepostioryID, path);
                    
                foreach(var file in contents.Where(content => content.Type == "file" && Path.GetExtension(content.Name) == $".{mainVM.LanguagesVM.SelectedLanguage.FileExtension}"))
                {
                    output.Items.Add(new FileSystemItemModel() { Name = file.Name, FullName = file.Path });
                }
                subDirs = contents.Where(dir => dir.Type == "dir").Select(dir => dir.Path).ToList();
            }
            foreach(var dir in await Task.WhenAll(subDirs.Select(dir => GetDirectory(dir))))
            {
                output.Items.Add(dir);
            }
            return output;

        }

        public async Task<List<FileSystemItemModel>> GetCheckedFileModels(DirectoryModel directory)
        {
            List<FileSystemItemModel> output = new List<FileSystemItemModel>();
            foreach(var file in directory.Items.Where(item => item is not DirectoryModel && item.IsChecked == true))
            {
                output.Add(file);
            }
            foreach (var dirFiles in await Task
                .WhenAll(directory.Items
                .Where(item => item is DirectoryModel && item.IsChecked != false)
                .Select(item => GetCheckedFileModels(item as DirectoryModel))))
            {
                output.AddRange(dirFiles);
            };
            return output;
        }
        #endregion
    }

    #region Enums
    public enum SourceTypes
    {
        Folder,
        Github
    }

    public enum SourceLanguages
    {
        CSharp
    }
    #endregion
}
