using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UMLGenerator.Models.FileSystemModels;
using UMLGenerator.WPFLibrary;

namespace UMLGenerator.ViewModels
{
    public class SelectWhichFilesViewModel : BaseViewModel
    {
        #region Commands
        public RelayCommand NextCommand { get; private set; }
        public RelayCommand BackCommand { get; private set; }
        #endregion

        #region Properties
        public DirectoryModel rootDir { get; set; }
        #endregion

        #region Fields
        private MainViewModel mainVM;
        #endregion

        #region Constructors
        public SelectWhichFilesViewModel(MainViewModel mainVM, string rootPath)
        {
            this.mainVM = mainVM;
            AddCommands();
            GetRoot(rootPath);
        }
        #endregion

        #region Methods
        private void AddCommands()
        {

        }

        private void GetRoot(string path)
        {
            if(Directory.Exists(path))
            {
                rootDir = GetDirectory(path); 
            }
            else
            {
                MessageBox.Show("The selected directory doesn't exist");
                mainVM.SelectedViewModel = new SelectSourceViewModel(mainVM);
            }
        }
        private DirectoryModel GetDirectory(string path)
        {
            var output = new DirectoryModel() { Name = Path.GetDirectoryName(path), FullName = path };
            foreach(var dir in Directory.GetDirectories(path))
            {
                try
                {
                    var dirModel = GetDirectory(dir);
                    output.Directories.Add(dirModel);
                }
                catch
                {

                }
            }
            foreach(var file in Directory.GetFiles(path))
            {
                if(Path.GetExtension(file) == "cs")
                {
                    output.Files.Add(new FileModel() { Name = Path.GetFileName(file), FullName = file });
                }
            }
            return output;
        }
        #endregion
    }
}
