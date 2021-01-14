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
    public class ObjectsTreeViewModel : BaseMainPartViewModel
    {

        #region Commands
        public RelayCommand GenerateUMLCommand { get; private set; }
        #endregion

        #region Properties
        public ObservableCollection<CodeComponentTypeModel> TreeItems { get; set; } = new ObservableCollection<CodeComponentTypeModel>();
        #endregion

        #region Constructors
        public ObjectsTreeViewModel(MainViewModel mainVM) : base(mainVM)
        {
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
            if (mainVM.GithubVM.RepostioryID == 0)
            {
                RunOnFiles(fileModels);
            }
            else
                RunOnFiles(fileModels, mainVM.GithubVM.GitClient, mainVM.GithubVM.RepostioryID);
            TreeItems.Clear();
            foreach(var item in TreeItems)
            {
                TreeItems.Add(item);
            }
            mainVM.UmlVM.UpdateUML(GenerateUML(TreeItems));
        }

        private void RunOnFiles(List<FileModel> fileModels) //local files
        {
            foreach (var file in fileModels)
            {
                new CodeFileViewModel(System.IO.File.ReadAllText(file.FullName), mainVM.LanguagesVM.SelectedLanguage);
            }
        }

        private void RunOnFiles(List<FileModel> fileModels, GitHubClient client, long repositoryId) // github files
        {
            foreach (var file in fileModels)
            {
                try
                {
                    var code = client.Repository.Content.GetAllContents(repositoryId, file.FullName).GetAwaiter().GetResult()[0].Content;
                    new CodeFileViewModel(code, mainVM.LanguagesVM.SelectedLanguage);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private string GenerateUML(IEnumerable<IUMLTransferable> source)
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
