using MVVMLibrary.ViewModels;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UMLGenerator.Interfaces;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.Models.FileSystemModels;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.Main
{
    public class ObjectsTreeViewModel : BaseGridColumnViewModel
    {

        #region Commands
        public RelayCommand GenerateUMLCommand { get; private set; }
        #endregion

        #region Properties
        private CodeProjectModel codeProject;

        public CodeProjectModel CodeProject
        {
            get { return codeProject; }
            set { codeProject = value; NotifyPropertyChanged(); }
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
        public ObjectsTreeViewModel(MainViewModel mainVM) : base(300, new GridLength(1, GridUnitType.Star), false)
        {
            this.mainVM = mainVM;
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            GenerateUMLCommand = new RelayCommand((o) =>
            {
                mainVM.UmlVM.UpdateUML(CodeProject.TransferToUML());
            }, o => CodeProject != null && CodeProject.Children.Count > 0);
        }

        public void GenerateObjectsTree(List<FileModel> fileModels)
        {
            IsLoading = true;
            CodeProject = new CodeProjectModel(mainVM.LanguagesVM.SelectedLanguage);
            List<string> filesContent = GetFilesContent(fileModels);
            RunOnFiles(filesContent);
            IsLoading = false;
            GenerateUMLCommand.Execute(null);
        }
        private List<string> GetFilesContent(List<FileModel> fileModels)
        {
            if (mainVM.GithubVM.RepostioryID == 0)
            {
                return fileModels.Select(file => System.IO.File.ReadAllText(file.FullName)).ToList();
            }
            else
            {
                return Task.WhenAll(fileModels.Select(file => GetGithubFileContent(file))).GetAwaiter().GetResult().ToList();
            }
        }
        private void RunOnFiles(List<string> filesContent)
        {
            filesContent.ForEach(code => 
            {
                var items = Libraries.ObjectsTreeLibrary.GetFileObjects(code, CodeProject);
                App.Current.Dispatcher.Invoke(() =>
                {
                    items.ForEach(item => CodeProject.Children.Add(item));
                });
            });
            Libraries.ObjectsTreeLibrary.SetPathFields(CodeProject.Children);
        }

        private async Task<string> GetGithubFileContent(FileModel file)
        {
            try
            {
                return (await mainVM.GithubVM.GitClient.Repository.Content
                    .GetAllContents(mainVM.GithubVM.RepostioryID, file.FullName))[0].Content;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return "";
            }
        }
        #endregion
    }
}
