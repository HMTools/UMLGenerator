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
using UMLGenerator.WPFLibrary;

namespace UMLGenerator.ViewModels
{
    public class SelectSourceViewModel : BaseViewModel
    {
        #region Commands
        public RelayCommand SelectFolderCommand { get; private set; }
        public RelayCommand GetDirectoryTree { get; private set; }
        public RelayCommand NextCommand { get; private set; }
        
        #endregion
        #region Properties
        private SourceTypes sourceType;

        public SourceTypes SourceType
        {
            get { return sourceType; }
            set { sourceType = value; NotifyPropertyChanged(); }
        }

        private SourceLanguages sourceLanguage;

        public SourceLanguages SourceLanguage
        {
            get { return sourceLanguage; }
            set { sourceLanguage = value; NotifyPropertyChanged(); }
        }


        private string folderPath;

        public string FolderPath
        {
            get { return folderPath; }
            set { folderPath = value; NotifyPropertyChanged(); }
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
        #region Fields
        private MainViewModel mainVM;
        private GitHubClient githubClient;
        private long repoId;
        #endregion

        #region Constructors
        public SelectSourceViewModel(MainViewModel mainVM)
        {
            this.mainVM = mainVM;
            AddMethods();
        }
        #endregion

        #region Methods
        private void AddMethods()
        {
            
            SelectFolderCommand = new RelayCommand(o => 
            {
                var dialog = new VistaFolderBrowserDialog();
                dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                {
                    FolderPath = dialog.SelectedPath;
                }
            });

            GetDirectoryTree = new RelayCommand(o =>
            {
                RootDir = SourceType == SourceTypes.Folder ? GetRootFromFolder() : GetRootFromRepository().GetAwaiter().GetResult();
            });

            NextCommand = new RelayCommand(o =>
            {
                mainVM.SelectedViewModel = sourceType == SourceTypes.Folder ?
                new UMLScreenViewModel(mainVM, GetCheckedFileModels(RootDir)) : new UMLScreenViewModel(mainVM, GetCheckedFileModels(RootDir), githubClient, repoId);
            });
            //}, (o) => !string.IsNullOrEmpty(FolderPath));
        }


        private DirectoryModel GetRootFromFolder()
        {
            if (Directory.Exists(FolderPath))
            {
                return GetFolderDirectory(FolderPath);
            }
            
            MessageBox.Show("The selected directory doesn't exist");
            mainVM.SelectedViewModel = new SelectSourceViewModel(mainVM);
            return null;
        }

        private async Task<DirectoryModel> GetRootFromRepository()
        {
            var productInformation = new ProductHeaderValue("UMLGenerator");
            var credentials = new Credentials("4f7c55c94fcccc3ca26f1f59b5b80a7ff00c3ad6");
            githubClient = new GitHubClient(productInformation) { Credentials = credentials };
            try
            {
                repoId = githubClient.Repository.Get(RepositoryOwner, RepositoryName).GetAwaiter().GetResult().Id;
                return GetRepositoryDirectory(githubClient, repoId, "");
            }
            catch
            {
                MessageBox.Show("The selected repository private or doesn't exist.");
                return null;
            }
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

        private DirectoryModel GetRepositoryDirectory(GitHubClient client, long repoId, string path)
        {
            var output = new DirectoryModel() { Name = Path.GetFileName(path), FullName = path };
            var contents = path == "" ? client.Repository.Content.GetAllContents(repoId).GetAwaiter().GetResult():  client.Repository.Content.GetAllContents(repoId, path).GetAwaiter().GetResult();
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



        private List<FileModel> GetCheckedFileModels(DirectoryModel directory)
        {
            List<FileModel> output = new List<FileModel>();
            foreach (var item in directory.Items)
            {
                switch (item)
                {
                    case FileModel file:
                        if (file.IsChecked)
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
        CSharp,
        
    }
    #endregion
}
