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
                mainVM.SelectedViewModel = new UMLScreenViewModel(mainVM, GetCheckedFileModels(RootDir));
                //mainVM.SelectedViewModel = new SelectWhichFilesViewModel(mainVM, FolderPath);
            }, (o) => !string.IsNullOrEmpty(FolderPath));
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
            var client = new GitHubClient(new ProductHeaderValue("UMLGenerator"));

            //try
            //{
                Repository repository = client.Repository.Get("octokit", "octokit.net").Result;
                return await GetRepositoryDirectory(client, repository.Id, "");
            //}
            //catch
            //{
            //    return null;
            //}
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

        private async Task<DirectoryModel> GetRepositoryDirectory(GitHubClient client, long repoId, string path)
        {
            var output = new DirectoryModel() { Name = Path.GetFileName(path), FullName = path };
            var contents = client.Repository.Content.GetAllContents(repoId).Result;
            foreach (var content in contents)
            {
                if(content.Type == "dir")
                {
                    output.Items.Add(GetRepositoryDirectory(client, repoId, content.Path).Result);
                }
                else if(content.Type == "file")
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
