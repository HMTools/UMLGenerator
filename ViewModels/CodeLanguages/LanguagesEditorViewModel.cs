using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.ViewModels.Main;

namespace UMLGenerator.ViewModels.CodeLanguages
{
    public class LanguagesEditorViewModel : BaseMainPartViewModel
    {
        #region Properties
        public ObservableCollection<string> Languages { get; set; } = new ObservableCollection<string>();

        private string selectedLanguageName;

        public string SelectedLanguageName
        {
            get { return selectedLanguageName; }
            set
            {
                selectedLanguageName = value;
                SelectedLanguage = JsonSerializer.Deserialize<CodeLanguageModel>(File.ReadAllText($"{Directory.GetCurrentDirectory()}\\Languages\\{value}.json"));
                NotifyPropertyChanged();
            }
        }


        private CodeLanguageModel selectedLanguage;

        public CodeLanguageModel SelectedLanguage
        {
            get { return selectedLanguage; }
            set
            {
                selectedLanguage = value;
                SelectedViewModel = new CodeLanguageViewModel(value);
                NotifyPropertyChanged();
            }
        }

        private string selectedComponentName;

        public string SelectedComponentName
        {
            get { return selectedComponentName; }
            set
            {
                selectedComponentName = value;
                SelectedComponent = SelectedLanguage.Components[value];
                NotifyPropertyChanged();
            }
        }


        private CodeComponentTypeModel selectedComponent;

        public CodeComponentTypeModel SelectedComponent
        {
            get { return selectedComponent; }
            set 
            { 
                selectedComponent = value;
                SelectedViewModel = new CodeComponentViewModel(value);
                NotifyPropertyChanged(); 
            }
        }
        private BaseViewModel selectedViewModel;

        public BaseViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; NotifyPropertyChanged(); }
        }


        #endregion

        #region Constructors
        public LanguagesEditorViewModel(MainViewModel mainVM) : base(mainVM)
        {
            LoadData();
        }
        #endregion

        #region Methods
        private void LoadData()
        {
            Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\Languages");
            foreach (var fileName in Directory.GetFiles($"{Directory.GetCurrentDirectory()}\\Languages"))
            {
                Languages.Add(Path.GetFileNameWithoutExtension(fileName));
            }
            if (Languages.Count > 0)
                SelectedLanguageName = Languages[0];

        }
        #endregion

        private void LoadDemoData()
        {


            //var language = new CodeLanguageModel();
            //language.Name = "C#";
            //language.CleanupModels.Add(new CodeCleanupModel("Block Comments", @"/\*(.*?)\*/", false, true));
            //language.CleanupModels.Add(new CodeCleanupModel("Line Comments", @"//(.*?)\r?\n", true, true));
            //language.CleanupModels.Add(new CodeCleanupModel("Strings", @"""((\\[^\n]|[^""\n])*)""", false, true));
            //language.CleanupModels.Add(new CodeCleanupModel("Regions", @"^[ \t]*\#[ \t]*(region|endregion).*\n", false, false));
            //language.CleanupModels.Add(new CodeCleanupModel("New Lines And Tabs", @"\t|\n|\r", false, true));
            //language.CleanupModels.Add(new CodeCleanupModel("Verbatim Strings", @"@(""[^""]*"")+", false, true));
            //language.CleanupModels.Add(new CodeCleanupModel("All Strings", "((\".*\")|(\'.*\'))", false, false));
            //language.CodeDelimiters.Add(new CodeDelimiterModel() { OpenDelimiter = '{', CloseDelimiter = '}', HasClose = true });
            //language.CodeDelimiters.Add(new CodeDelimiterModel() { OpenDelimiter = ';', HasClose = false });

            //var classComp = new CodeComponentTypeModel();
            //classComp.Name = "Class";
            //classComp.NamePattern = @"(^| +)class +(?<Name>((\w+ *<[^>]+>)|\w+))";
            //classComp.TruePatterns.Add(@"(^| +)class ");
            //language.Components.Add(classComp.Name, classComp);


            //var abstractField = new CodeFieldTypeModel();
            //classComp.Fields.Add(abstractField);
            //abstractField.Name = "IsAbstract";
            //abstractField.Pattern = @"(^| +)(abstract) +";
            //abstractField.InputType = FieldInputType.Boolean;
            //abstractField.Values.Add("{abstract} ");
            //abstractField.Values.Add("");

            //var namespaceComp = new CodeComponentTypeModel();
            //namespaceComp.Containers.Add(classComp.Name);
            //namespaceComp.Name = "Namespace";
            //namespaceComp.NamePattern = @"(^| +)namespace +(?<Name>[\w.]+) *{";
            //namespaceComp.TruePatterns.Add(@"(^| +)namespace +([\w.]+) *{");
            //language.Components.Add(namespaceComp.Name, namespaceComp);
            //language.Containers.Add(namespaceComp.Name);

            //Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}\\Languages");
            //File.WriteAllText($"{Directory.GetCurrentDirectory()}\\Languages\\C#.json", JsonSerializer.Serialize<CodeLanguageModel>(language));
        }
    }
}
