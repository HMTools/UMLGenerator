using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.WPFLibrary;

namespace UMLGenerator.ViewModels
{
    public class SelectSourceViewModel : BaseViewModel
    {
        #region Commands
        public RelayCommand SelectFolderCommand { get; private set; }
        public RelayCommand NextCommand { get; private set; }
        
        #endregion
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
        #region Fields
        private MainViewModel mainVM;
        #endregion

        #region Constructors
        public SelectSourceViewModel(MainViewModel mainVM)
        {
            this.mainVM = mainVM;
            AddMethods();
        }
        #endregion

        #region Methods
        private void AddMethods()
        {
            
            SelectFolderCommand = new RelayCommand(o => 
            {
                var dialog = new VistaFolderBrowserDialog();
                dialog.ShowDialog();
                if (dialog.SelectedPath != "")
                {
                    TargetPath = dialog.SelectedPath;
                }
            });
            NextCommand = new RelayCommand(o => 
            {
                mainVM.SelectedViewModel = new SelectWhichFilesViewModel(mainVM, TargetPath);
            }, (o) => !string.IsNullOrEmpty(TargetPath));
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
