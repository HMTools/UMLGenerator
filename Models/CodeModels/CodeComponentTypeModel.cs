using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Interfaces;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeComponentTypeModel : BaseModel, IUMLTransferable
    {
        #region Properties
        public string Name { get; set; } = "";
        public ObservableCollection<string> Containers { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<CodeFieldTypeModel> Fields { get; set; } = new ObservableCollection<CodeFieldTypeModel>();

        public string NamePattern { get; set; } = "";
        public ObservableCollection<string> TruePatterns { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> FalsePatterns { get; set; } = new ObservableCollection<string>();

        public string TransferToUML(int layer, Dictionary<string, List<string>> classesDict, Dictionary<string, List<string>> interfacesDict)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
