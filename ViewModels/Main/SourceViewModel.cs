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
    public class SourceViewModel : BaseMainPartViewModel
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
                            sourceType = value; NotifyPropertyChanged();
                    }
                    return;
                }
                else
                {
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
            set { rootDir = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public SourceViewModel(MainViewModel mainVM) : base(mainVM) { }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            SelectFolderCommand = new RelayCommand(o =>
            {
                var dialog = new VistaFolderBrowserDialog();
                dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                {
                    RootDir = GetFolderDirectory(dialog.SelectedPath);
                    mainVM.GithubVM.RepostioryID = 0;
                }
            });

            GetRepositoryCommand = new RelayCommand(o =>
            {
                try
                {
                    mainVM.GithubVM.RepostioryID = mainVM.GithubVM.GitClient.Repository.Get(RepositoryOwner, RepositoryName).GetAwaiter().GetResult().Id;
                    RootDir = GetRepositoryDirectory(mainVM.GithubVM.GitClient, mainVM.GithubVM.RepostioryID, "");
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }

            });

            GetObjectsTreeCommand = new RelayCommand(o => 
            {
                mainVM.ObjectsTreeVM.IsShown = true;
                mainVM.UmlVM.IsShown = true;
                IsShown = false;
                mainVM.ObjectsTreeVM.GenerateObjectsTree(GetCheckedFileModels(RootDir));
            }, o => RootDir != null);
        }
        private DirectoryModel GetFolderDirectory(string path)
        {
            var output = new DirectoryModel() { Name = Path.GetFileName(path), FullName = path };
            foreach (var dir in Directory.GetDirectories(path))
            {
                output.Items.Add(GetFolderDirectory(dir));
            }
            foreach (var file in Directory.GetFiles(path))
            {
                if (Path.GetExtension(file) == ".cs")
                {
                    output.Items.Add(new FileModel() { Name = Path.GetFileName(file), FullName = file });
                }
            }
            return output;
        }

        public DirectoryModel GetRepositoryDirectory(GitHubClient client, long repoId, string path)
        {
            var output = new DirectoryModel() { Name = Path.GetFileName(path), FullName = path };
            var contents = path == "" ? 
                client.Repository.Content.GetAllContents(repoId).GetAwaiter().GetResult():
                client.Repository.Content.GetAllContents(repoId, path).GetAwaiter().GetResult();

            foreach (var content in contents)
            {
                if(content.Type == "dir")
                {
                    output.Items.Add(GetRepositoryDirectory(client, repoId, content.Path));
                }
                else if(content.Type == "file" && Path.GetExtension(content.Name) == ".cs")
                {
                    output.Items.Add(new FileModel() { Name = content.Name, FullName = content.Path });
                }
            }
            return output;
        }



        public List<FileModel> GetCheckedFileModels(DirectoryModel directory)
        {
            List<FileModel> output = new List<FileModel>();
            foreach (var item in directory.Items)
            {
                switch (item)
                {
                    case FileModel file:
                        if (file.IsChecked == true)
                            output.Add(file);
                        break;
                    case DirectoryModel dir:
                        output.AddRange(GetCheckedFileModels(dir));
                        break;
                }
            }
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
