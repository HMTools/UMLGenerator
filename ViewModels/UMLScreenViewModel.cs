using Octokit;
using PlantUml.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMLGenerator.Interfaces;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.Models.FileSystemModels;
using UMLGenerator.WPFLibrary;

namespace UMLGenerator.ViewModels
{
    public class UMLScreenViewModel : BaseViewModel
    {
        #region Commands
        public RelayCommand ShowPreviewCommand { get; private set; }
        public RelayCommand BackCommand { get; private set; }

        #endregion
        #region Properties
        public Dictionary<string, NamespaceModel> Namespaces { get; set; }
        public Dictionary<string, List<string>> Classes { get; set; }
        public Dictionary<string, List<string>> Interfaces { get; set; }
        public string Results { get; set; }
        #endregion

        #region Public Static Fields
        public static Dictionary<string, char> AccessModifiersDict = new Dictionary<string, char>() //maybe need to remove from static
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

        #region Fields
        private MainViewModel mainVM;
        #endregion

        #region Constructors
        public UMLScreenViewModel(MainViewModel mainVM, List<FileModel> fileModels)
        {
            this.mainVM = mainVM;
            Namespaces = new Dictionary<string, NamespaceModel>();
            Classes = new Dictionary<string, List<string>>();
            Interfaces = new Dictionary<string, List<string>>();
            RunOnFiles(fileModels);
            Results = GenerateUML(Namespaces.Values);
            AddCommands();
        }

        public UMLScreenViewModel(MainViewModel mainVM, List<FileModel> fileModels, GitHubClient client, long repositoryId)
        {
            this.mainVM = mainVM;
            Namespaces = new Dictionary<string, NamespaceModel>();
            Classes = new Dictionary<string, List<string>>();
            Interfaces = new Dictionary<string, List<string>>();
            RunOnFiles(fileModels, client, repositoryId);
            Results = GenerateUML(Namespaces.Values);
            AddCommands();
        }
        #endregion

        #region Methods

        private void AddCommands()
        {
            ShowPreviewCommand = new RelayCommand(o =>
            {
                var factory = new RendererFactory();

                var renderer = factory.CreateRenderer(new PlantUmlSettings());

                new Views.OpenFullWindowPNG(renderer.RenderAsync(Results, OutputFormat.Png).GetAwaiter().GetResult()).ShowDialog();
            });

            BackCommand = new RelayCommand(o =>
            {
                mainVM.SelectedViewModel = new SelectSourceViewModel(mainVM);
            });
        }
        private void RunOnFiles(List<FileModel> fileModels) //local files
        {
            foreach(var file in fileModels)
            {
                new CodeFileViewModel(file.Name, System.IO.File.ReadAllText(file.FullName), Namespaces, Classes, Interfaces);
            }
        }

        private void RunOnFiles(List<FileModel> fileModels, GitHubClient client, long repositoryId) // github files
        {
            foreach (var file in fileModels)
            {
                try
                {
                    var code = client.Repository.Content.GetAllContents(repositoryId, file.FullName).GetAwaiter().GetResult()[0].Content;
                    new CodeFileViewModel(file.Name, code, Namespaces, Classes, Interfaces);
                }
                catch
                {
                    //failed getting this file
                }
            }
        }

        private string GenerateUML(IEnumerable<IUMLTransferable> source)
        {
            string res = "@startuml\n";
            foreach (var obj in source)
            {
                res += obj.TransferToUML(0, Classes, Interfaces);
            }
            return res + "@enduml";
        }
        #endregion
    }
}
