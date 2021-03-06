﻿using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.Models.FileSystemModels;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.Main
{
    public class ObjectsTreeViewModel : BaseGridColumnViewModel
    {
        #region Commands

        public RelayCommand GenerateUMLCommand { get; private set; }

        #endregion Commands

        #region Properties

        private CodeProjectModel codeProject;

        public CodeProjectModel CodeProject
        {
            get { return codeProject; }
            set { codeProject = value; NotifyPropertyChanged(); }
        }

        private bool isLoading;

        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; NotifyPropertyChanged(); }
        }

        #endregion Properties

        #region Fields

        private readonly MainViewModel mainVM;

        #endregion Fields

        #region Constructors

        public ObjectsTreeViewModel(MainViewModel mainVM) : base(300, new GridLength(1, GridUnitType.Star), false, true)
        {
            this.mainVM = mainVM;
        }

        #endregion Constructors

        #region Methods

        protected override void AddCommands()
        {
            base.AddCommands();
            GenerateUMLCommand = new RelayCommand((o) =>
            {
                mainVM.UmlVM.UpdateUML(CodeProject.TransferToUML());
            }, o => CodeProject != null && CodeProject.Children.Count > 0 && CodeProject.Children.Any(child => child.IsChecked != false));
        }

        public void GenerateObjectsTree(List<FileSystemItemModel> fileModels)
        {
            IsLoading = true;
            CodeProject = new CodeProjectModel(mainVM.LanguagesVM.SelectedLanguage);
            List<string> filesContent = GetFilesContent(fileModels);
            RunOnFiles(filesContent);
            IsLoading = false;
            GenerateUMLCommand.Execute(null);
        }

        private List<string> GetFilesContent(List<FileSystemItemModel> fileModels)
        {
            if (mainVM.SourceVM.SourceType == SourceTypes.Folder)
                return fileModels.Select(file => System.IO.File.ReadAllText(file.FullName)).ToList();
            else
                return Task.WhenAll(fileModels.Select(file => GetGithubFileContent(file))).GetAwaiter().GetResult().ToList();
        }

        private void RunOnFiles(List<string> filesContent)
        {
            filesContent.ForEach(code =>
            {
                var items = Libraries.CodeStructureLibrary.GetFileObjects(code, CodeProject);
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    items.ForEach(item => CodeProject.Children.Add(item));
                });
            });
            Libraries.CodeStructureLibrary.SetPathFields(CodeProject.Children);
        }

        private async Task<string> GetGithubFileContent(FileSystemItemModel file)
        {
            try
            {
                return (await mainVM.GithubVM.GitClient.Repository.Content
                    .GetAllContents(mainVM.GithubVM.RepostioryID, file.FullName))[0].Content;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return "";
            }
        }

        #endregion Methods
    }
}