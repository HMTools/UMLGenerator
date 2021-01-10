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

        public RelayCommand SettingsCommand { get; private set; }
        public RelayCommand ExportSVGCommand { get; private set; }
        public RelayCommand ExportPNGCommand { get; private set; }
        public RelayCommand ExportPlantCommand { get; private set; }
        #endregion

        #region Properties
        public SourceViewModel SourceVM { get; set; }
        public ObjectsTreeViewModel ObjectsTreeVM { get; set; }
        public UMLViewModel UmlVM { get; set; }

        public GitHubClient GitClient { get; set; }
        public long RepostioryID { get; set; }
        #endregion

        #region Constructors
        public MainViewModel()
        {
            SourceVM = new SourceViewModel(this);
            ObjectsTreeVM = new ObjectsTreeViewModel();
            UmlVM = new UMLViewModel();

            SourceVM.OnSourceSelectedUpdate += (s, e) =>
            {
                ObjectsTreeVM.GenerateObjectsTree(e, GitClient, RepostioryID);
            };
            ObjectsTreeVM.OnSelectedObjectsUpdate += (s, e) =>
            {
                UmlVM.UpdateUML(e);
            };
            LoadData();
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            SettingsCommand = new RelayCommand(o =>
            {
               new Views.Settings.SettingsView() { DataContext = new Settings.SettingsViewModel(this) }.ShowDialog();
            });

            ExportSVGCommand = new RelayCommand(o => 
            {
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "SVG file (*.svg)|*.svg" };
                dialog.ShowDialog();
                if (dialog.FileName != "")
                {
                    File.WriteAllText(dialog.FileName, UmlVM.SvgString);
                }
            }, o => UmlVM.SvgString != "" && UmlVM.IsLoading == false);

            ExportPNGCommand = new RelayCommand(o =>
            {
                SaveFileDialog dialog = new SaveFileDialog() { Filter = "PNG file (*.png)|*.png" };
                dialog.ShowDialog();
                if (dialog.FileName != "")
                {
                    UmlVM.UMLImage.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }, o => UmlVM.SvgString != "" && UmlVM.IsLoading == false);

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

        private void LoadData()
        {
            GitClient = GetGithubClient(ConfigurationManager.AppSettings["GitApiToken"]);
        }

        public GitHubClient GetGithubClient(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var productInformation = new ProductHeaderValue("UMLGenerator");
                var credentials = new Credentials(token);
                var client = new GitHubClient(productInformation) { Credentials = credentials };
                try
                {
                    var user = client.User.Current().GetAwaiter().GetResult();
                    return client;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Bad credentials")
                    {
                        return null;
                    }
                }
            }
            
            return null;
        }
        #endregion
    }
}
