using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MVVMLibrary.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml;
using UMLGenerator.Models.CodeModels;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.CodeLanguages
{
    public class CodeLanguageViewModel : BaseViewModel
    {
        #region Events

        public event EventHandler OnCreate;

        #endregion Events

        #region Commands

        public RelayCommand CreateLanguageCommand { get; private set; }
        public RelayCommand AddFieldCommand { get; private set; }
        public RelayCommand RemoveFieldCommand { get; private set; }
        public RelayCommand AddDelimiterCommand { get; private set; }
        public RelayCommand RemoveDelimiterCommand { get; private set; }
        public RelayCommand AddCleanupCommand { get; private set; }
        public RelayCommand RemoveCleanupCommand { get; private set; }
        public RelayCommand AddSubComponentCommand { get; private set; }
        public RelayCommand RemoveSubComponentCommand { get; private set; }
        public RelayCommand SwapSubComponentCommand { get; private set; }

        #endregion Commands

        #region Properties

        private IHighlightingDefinition syntax;

        public IHighlightingDefinition Syntax
        {
            get { return syntax; }
            set { syntax = value; NotifyPropertyChanged(); }
        }

        public CodeLanguageModel Language { get; set; }
        public LanguagesEditorViewModel LanguagesEditor { get; set; }
        public ObservableCollection<CodeFieldViewModel> Fields { get; set; } = new ObservableCollection<CodeFieldViewModel>();

        private bool isCreating;

        public bool IsCreating
        {
            get { return isCreating; }
            set { isCreating = value; NotifyPropertyChanged(); }
        }

        #endregion Properties

        #region Constructor

        public CodeLanguageViewModel(LanguagesEditorViewModel languageEditor, CodeLanguageModel language)
        {
            SetEditorSyntax();
            LanguagesEditor = languageEditor;
            if (language != null)
            {
                Language = language;
                IsCreating = false;
                foreach (var field in Language.Fields)
                {
                    Fields.Add(new CodeFieldViewModel(field));
                }
            }
            else
            {
                Language = new CodeLanguageModel();
                IsCreating = true;
            }
        }

        #endregion Constructor

        #region Methods

        protected override void AddCommands()
        {
            base.AddCommands();
            CreateLanguageCommand = new RelayCommand(o =>
            {
                if (string.IsNullOrWhiteSpace(Language.Name))
                {
                    MessageBox.Show("Please decalre a name!");
                }
                else
                {
                    OnCreate?.Invoke(this, EventArgs.Empty);
                }
            });

            #region Fields Commands

            AddFieldCommand = new RelayCommand(o =>
            {
                var field = new CodeFieldTypeModel();
                Language.Fields.Add(field);
                Fields.Add(new CodeFieldViewModel(field));
            });
            RemoveFieldCommand = new RelayCommand(o =>
            {
                var fieldVM = o as CodeFieldViewModel;
                Fields.Remove(fieldVM);
                Language.Fields.Remove(fieldVM.Field);
            });

            #endregion Fields Commands

            #region Delimiter Commands

            AddDelimiterCommand = new RelayCommand(o =>
            {
                Language.CodeDelimiters.Add(new CodeDelimiterModel());
            });
            RemoveDelimiterCommand = new RelayCommand(o =>
            {
                Language.CodeDelimiters.Remove(o as CodeDelimiterModel);
            });

            #endregion Delimiter Commands

            #region Cleanup Commands

            AddCleanupCommand = new RelayCommand(o =>
            {
                Language.CleanupModels.Add(new CodeCleanupModel());
            });
            RemoveCleanupCommand = new RelayCommand(o =>
            {
                Language.CleanupModels.Remove(o as CodeCleanupModel);
            });

            #endregion Cleanup Commands

            #region SubComponents Commands

            AddSubComponentCommand = new RelayCommand(o =>
            {
                var (s, t) = o as Tuple<object, object>;
                if (s is CodeComponentTypeModel && t is CodeLanguageViewModel)
                {
                    CodeComponentTypeModel source = s as CodeComponentTypeModel;
                    if (!Language.SubComponents.Contains(source.Name))
                        Language.SubComponents.Add(source.Name);
                }
            });
            RemoveSubComponentCommand = new RelayCommand(o =>
            {
                var (s, t) = o as Tuple<object, object>;
                if (s is string && t is CodeLanguageViewModel)
                {
                    string source = s as string;
                    if (Language.SubComponents.Contains(source))
                        Language.SubComponents.Remove(source);
                }
            });
            SwapSubComponentCommand = new RelayCommand(o =>
            {
                var (s, t) = o as Tuple<object, object>;
                if (s is string && t is string)
                {
                    string source = s as string, target = t as string;
                    if (Language.SubComponents.Contains(source))
                        Language.SubComponents.Swap(source, target);
                }
            });

            #endregion SubComponents Commands
        }

        private void SetEditorSyntax()
        {
            string path = $"{Directory.GetCurrentDirectory()}\\Resources\\EditorSyntax\\PlantUMLBuilder.xshd";
            using XmlTextReader reader = new XmlTextReader(path);
            Syntax = HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }

        #endregion Methods
    }
}