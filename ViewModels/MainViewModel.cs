using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using UMLGenerator.Interfaces;
using UMLGenerator.Models.CodeModels;

namespace UMLGenerator.ViewModels
{
    public class MainViewModel :INotifyPropertyChanged
    {


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region Properties

        private string input;

        public string Input
        {
            get { return input; }
            set { input = value; AnalyzeCode(); }
        }
        private string output;

        public string Output
        {
            get { return output; }
            set { output = value; NotifyPropertyChanged(); }
        }

        public Dictionary<string, NamespaceModel> Namespaces { get; set; }
        #endregion
        #region Public Static Fields
        public static Dictionary<string, char> AccessModifiersDict = new Dictionary<string, char>()
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
        public MainViewModel()
        {
            Namespaces = new Dictionary<string, NamespaceModel>();
        }
        #endregion

        #region Private Methods
        private void AnalyzeCode()
        {
            CodeFileViewModel codeFile = new CodeFileViewModel("base.cs", Input, Namespaces);
            
            Output = GenerateUML(Namespaces.Values);
        }

        private string GenerateUML(IEnumerable<IUMLTransferable> source)
        {
            string res = "@startuml\n";
            foreach(var obj in source)
            {
                res += obj.TransferToUML(0);
            }
            return res + "@enduml";
        }
        #endregion
    }
}
