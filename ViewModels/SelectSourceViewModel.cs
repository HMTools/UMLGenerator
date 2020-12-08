using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMLGenerator.ViewModels
{
    public class SelectSourceViewModel : BaseViewModel
    {
        #region Properties
        private SourceTypes sourceType;

        public SourceTypes SourceType
        {
            get { return sourceType; }
            set { sourceType = value; NotifyPropertyChanged(); }
        }

        private SourceLanguages sourceLanguage;

        public SourceLanguages SourceLanguage
        {
            get { return sourceLanguage; }
            set { sourceLanguage = value; NotifyPropertyChanged(); }
        }


        private string targetPath;

        public string TargetPath
        {
            get { return targetPath; }
            set { targetPath = value; NotifyPropertyChanged(); }
        }


        #endregion
    }

    #region Enums
    public enum SourceTypes
    {
        Folder,
        Github
    }

    public enum SourceLanguages
    {
        CSharp,
        
    }
    #endregion
}
