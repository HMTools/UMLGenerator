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
        private MainViewModel mainVM;
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
            if (mainVM.GithubVM.RepostioryID == 0)
            {
                RunOnFiles(fileModels);
                mainVM.UmlVM.UpdateUML(CodeProject.TransferToUML());
            }
            else
            {
                IsLoading = true;
                RunOnFiles(fileModels, mainVM.GithubVM.GitClient, mainVM.GithubVM.RepostioryID).ContinueWith(t => 
                {
                    IsLoading = false;
                    mainVM.UmlVM.UpdateUML(CodeProject.TransferToUML());
                });
            }
        }

        private void RunOnFiles(List<FileModel> fileModels) //local files
        {
            CodeProject = new CodeProjectModel(mainVM.LanguagesVM.SelectedLanguage);
            foreach (var file in fileModels)
            {
                var vm = new CodeFileViewModel(System.IO.File.ReadAllText(file.FullName), CodeProject);
                vm.GetLanguageObjects().ForEach(item => CodeProject.Children.Add(item));
            }
        }

        private async Task RunOnFiles(List<FileModel> fileModels, GitHubClient client, long repositoryId) // github files
        {
            CodeProject = new CodeProjectModel(mainVM.LanguagesVM.SelectedLanguage);
            List<Task> tasks = new List<Task>();
            fileModels.ForEach(file => tasks.Add(GetFileContent(file)));
            await Task.WhenAll(tasks);

            async Task GetFileContent(FileModel file)
            {
                try
                {
                    await Task.Run(() =>
                    {
                        var code = client.Repository.Content.GetAllContents(repositoryId, file.FullName).GetAwaiter().GetResult()[0].Content;
                        var vm = new CodeFileViewModel(code, CodeProject);
                        vm.GetLanguageObjects().ForEach(item =>
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                CodeProject.Children.Add(item);
                            });
                        });
                    });

                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
        #endregion
    }
}
