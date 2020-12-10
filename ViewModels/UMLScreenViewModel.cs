using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Interfaces;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.Models.FileSystemModels;

namespace UMLGenerator.ViewModels
{
    public class UMLScreenViewModel : BaseViewModel
    {
        #region Commands

        #endregion
        #region Properties
        public Dictionary<string, NamespaceModel> Namespaces { get; set; }
        public string Results { get; set; }
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

        #region Fields
        private MainViewModel mainVM;
        #endregion

        #region Constructors
        public UMLScreenViewModel(MainViewModel mainVM, List<FileModel> fileModels)
        {
            this.mainVM = mainVM;
            Namespaces = new Dictionary<string, NamespaceModel>();
            RunOnFiles(fileModels);
            Results = GenerateUML(Namespaces.Values);
        }
        #endregion

        #region Methods
        private void RunOnFiles(List<FileModel> fileModels)
        {
            foreach(var file in fileModels)
            {
                new CodeFileViewModel(file.Name, System.IO.File.ReadAllText(file.FullName), Namespaces);
            }
        }

        private string GenerateUML(IEnumerable<IUMLTransferable> source)
        {
            string res = "@startuml\n";
            foreach (var obj in source)
            {
                res += obj.TransferToUML(0);
            }
            return res + "@enduml";
        }
        #endregion
    }
}
