using MVVMLibrary.ViewModels;
using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using UMLGenerator.Interfaces;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.ViewModels.Main;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Commands

        public RelayCommand ToggleExportCommand { get; private set; }
        public RelayCommand ExportSVGCommand { get; private set; }
        public RelayCommand ExportPNGCommand { get; private set; }
        public RelayCommand ExportPlantCommand { get; private set; }
        #endregion

        #region Properties
        public SourceViewModel SourceVM { get; set; }
        public ObjectsTreeViewModel ObjectsTreeVM { get; set; }
        public UMLViewModel UmlVM { get; set; }
        public GithubViewModel GithubVM { get; set; }

        private bool isShowExport;

        public bool IsShowExport
        {
            get { return isShowExport; }
            set { isShowExport = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            GithubVM = new GithubViewModel(this);
            SourceVM = new SourceViewModel(this);
            ObjectsTreeVM = new ObjectsTreeViewModel(this);
            UmlVM = new UMLViewModel(this);
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            ToggleExportCommand = new RelayCommand(o => IsShowExport = !IsShowExport);
            ExportSVGCommand = new RelayCommand(o => 
            {
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "SVG file (*.svg)|*.svg" };
                dialog.ShowDialog();
                if (dialog.FileName != "")
                {
                    File.WriteAllText(dialog.FileName, UmlVM.SvgString);
                }
            }, o => !string.IsNullOrEmpty(UmlVM.SvgString) && UmlVM.IsLoading == false);

            ExportPNGCommand = new RelayCommand(o =>
            {
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "PNG file (*.png)|*.png" };
                dialog.ShowDialog();
                if (dialog.FileName != "")
                {
                    UmlVM.UMLImage.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }, o => !string.IsNullOrEmpty(UmlVM.SvgString) && UmlVM.IsLoading == false);

            ExportPlantCommand = new RelayCommand(o =>
            {
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "PlantUML file (*.wsd)|*.wsd" };
                dialog.ShowDialog();
                if (dialog.FileName != "")
                {
                    File.WriteAllText(dialog.FileName, UmlVM.PlantUml);
                }
            }, (o) => !string.IsNullOrWhiteSpace(UmlVM.PlantUml));
        }      
        #endregion
    }
}
