namespace UMLGenerator.Models.FileSystemModels
{
    public class FileSystemItemModel : BaseModel
    {
        #region Properties

        public string Name { get; set; }
        public string FullName { get; set; }

        private bool? isChecked = false;

        public bool? IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; NotifyPropertyChanged(); }
        }

        public bool IsChangingCheck { get; set; } = false;

        #endregion Properties
    }
}