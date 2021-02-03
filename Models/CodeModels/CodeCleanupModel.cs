namespace UMLGenerator.Models.CodeModels
{
    public class CodeCleanupModel : BaseModel
    {
        #region Properties

        private string name = "";

        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged(); }
        }

        public string Pattern { get; set; } = "";
        public bool ReplaceWithNewLine { get; set; }
        public bool RegexSignleLine { get; set; }

        #endregion Properties
    }
}