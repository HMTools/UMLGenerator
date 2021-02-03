using System.Collections.ObjectModel;

namespace UMLGenerator.Models.CodeModels
{
    public class CodeComponentTypeModel : BaseModel
    {
        #region Properties

        public string Name { get; set; } = "";
        public string Tag { get; set; } = "";
        public System.Windows.Media.Color TagColor { get; set; }
        public ObservableCollection<string> SubComponents { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<CodeFieldTypeModel> Fields { get; set; } = new ObservableCollection<CodeFieldTypeModel>();
        public ObservableCollection<CodeDelimiterModel> CodeDelimiters { get; set; } = new ObservableCollection<CodeDelimiterModel>();
        public string NamePattern { get; set; } = "";
        public ObservableCollection<CodePatternModel> TruePatterns { get; set; } = new ObservableCollection<CodePatternModel>();
        public ObservableCollection<CodePatternModel> FalsePatterns { get; set; } = new ObservableCollection<CodePatternModel>();
        public string UMLPattern { get; set; } = "";
        private bool isInCollection;

        public bool IsInCollection
        {
            get { return isInCollection; }
            set { isInCollection = value; NotifyPropertyChanged(); }
        }

        public bool IsUnique { get; set; } = false;

        public CodeComponentTypeModel()
        {
            TagColor = System.Windows.Media.Colors.Blue;
        }

        #endregion Properties
    }
}