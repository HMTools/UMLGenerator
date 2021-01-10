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
    public class ObjectsTreeViewModel : BaseViewModel
    {
        #region Events
        public event EventHandler<string> OnSelectedObjectsUpdate;
        #endregion

        #region Commands
        public RelayCommand GenerateUMLCommand { get; private set; }
        #endregion

        #region Properties
        public Dictionary<string, NamespaceModel> Namespaces { get; set; }
        public Dictionary<string, List<string>> Classes { get; set; }
        public Dictionary<string, List<string>> Interfaces { get; set; }

        public ObservableCollection<NamespaceModel> TreeItems { get; set; } = new ObservableCollection<NamespaceModel>();

        #endregion

        #region Public Static Fields
        public static Dictionary<string, char> AccessModifiersDict = new Dictionary<string, char>() //maybe need to remove from static
        {
            { "", '-'},
            { "private", '-'},
            { "protected", '#'},
            { "private protected", '#'},
            { "protected internal", '#'},
            { "internal", '#'},
            { "public", '+'}
        };
        #endregion

        #region Constructors
        public ObjectsTreeViewModel()
        {
            Namespaces = new Dictionary<string, NamespaceModel>();
            Classes = new Dictionary<string, List<string>>();
            Interfaces = new Dictionary<string, List<string>>();
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            GenerateUMLCommand = new RelayCommand((o) =>
            {
                OnSelectedObjectsUpdate?.Invoke(this, GenerateUML(Namespaces.Values));
            });
        }

        public void GenerateObjectsTree(List<FileModel> fileModels, GitHubClient client, long repositoryId)
        {
            if (repositoryId == 0)
            {
                RunOnFiles(fileModels);
            }
            else
                RunOnFiles(fileModels, client, repositoryId);
            TreeItems.Clear();
            foreach(var item in Namespaces.Values)
            {
                TreeItems.Add(item);
            }
            OnSelectedObjectsUpdate?.Invoke(this, GenerateUML(Namespaces.Values));
        }

        private void RunOnFiles(List<FileModel> fileModels) //local files
        {
            foreach (var file in fileModels)
            {
                new CodeFileViewModel(file.Name, System.IO.File.ReadAllText(file.FullName), Namespaces, Classes, Interfaces);
            }
        }

        private void RunOnFiles(List<FileModel> fileModels, GitHubClient client, long repositoryId) // github files
        {
            foreach (var file in fileModels)
            {
                try
                {
                    var code = client.Repository.Content.GetAllContents(repositoryId, file.FullName).GetAwaiter().GetResult()[0].Content;
                    new CodeFileViewModel(file.Name, code, Namespaces, Classes, Interfaces);
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
                res += obj.TransferToUML(0, Classes, Interfaces);
            }
            return res + "@enduml";
        }
        #endregion
    }
}
