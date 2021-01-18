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
        public ObservableCollection<CodeObjectModel> TreeItems { get; set; } = new ObservableCollection<CodeObjectModel>();
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
                mainVM.UmlVM.UpdateUML(GenerateUML(TreeItems));
            }, o => TreeItems.Count > 0);
        }

        public void GenerateObjectsTree(List<FileModel> fileModels)
        {
            TreeItems.Clear();
            if (mainVM.GithubVM.RepostioryID == 0)
            {
                RunOnFiles(fileModels);
            }
            else
                RunOnFiles(fileModels, mainVM.GithubVM.GitClient, mainVM.GithubVM.RepostioryID);
            
            mainVM.UmlVM.UpdateUML(GenerateUML(TreeItems));
        }

        private void RunOnFiles(List<FileModel> fileModels) //local files
        {
            var project = new CodeProjectModel(mainVM.LanguagesVM.SelectedLanguage);
            foreach (var file in fileModels)
            {
                var vm = new CodeFileViewModel(System.IO.File.ReadAllText(file.FullName), project);
                vm.GetLanguageObjects().ForEach(item => TreeItems.Add(item)); 
            }
        }

        private void RunOnFiles(List<FileModel> fileModels, GitHubClient client, long repositoryId) // github files
        {
            var project = new CodeProjectModel(mainVM.LanguagesVM.SelectedLanguage);
            foreach (var file in fileModels)
            {
                try
                {
                    var code = client.Repository.Content.GetAllContents(repositoryId, file.FullName).GetAwaiter().GetResult()[0].Content;
                    var vm = new CodeFileViewModel(code, project);
                    vm.GetLanguageObjects().ForEach(item => TreeItems.Add(item));
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private string GenerateUML(IEnumerable<CodeObjectModel> source)
        {
            string res = "@startuml\n";
            foreach (var obj in source)
            {
                //res += obj.TransferToUML(0, null, null);
            }
            return res + "@enduml";
        }
        #endregion
    }
}
