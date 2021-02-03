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
    public class CodeComponentViewModel : BaseViewModel
    {
        #region Events

        public event EventHandler OnCreate;

        #endregion Events

        #region Commands

        public RelayCommand CreateComponentCommand { get; private set; }
        public RelayCommand AddTruePatternCommand { get; private set; }
        public RelayCommand AddFalsePatternCommand { get; private set; }
        public RelayCommand RemoveTruePatternCommand { get; private set; }
        public RelayCommand RemoveFalsePatternCommand { get; private set; }
        public RelayCommand AddFieldCommand { get; private set; }
        public RelayCommand RemoveFieldCommand { get; private set; }
        public RelayCommand AddDelimiterCommand { get; private set; }
        public RelayCommand RemoveDelimiterCommand { get; private set; }
        public RelayCommand AddSubCommand { get; private set; }
        public RelayCommand RemoveSubCommand { get; private set; }
        public RelayCommand SwapSubCommand { get; private set; }

        #endregion Commands

        #region Properties

        private bool isCreating;

        public bool IsCreating
        {
            get { return isCreating; }
            set { isCreating = value; NotifyPropertyChanged(); }
        }

        private IHighlightingDefinition syntax;

        public IHighlightingDefinition Syntax
        {
            get { return syntax; }
            set { syntax = value; NotifyPropertyChanged(); }
        }

        public CodeFieldTypeModel SelectedCommonField
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    var newField = new CodeFieldViewModel(value);
                    Fields.Add(newField);
                    Component.Fields.Add(newField.Field);
                }
            }
        }

        public CodeComponentTypeModel Component { get; set; }
        public ObservableCollection<CodeFieldViewModel> Fields { get; set; } = new ObservableCollection<CodeFieldViewModel>();
        public LanguagesEditorViewModel LanguageEditor { get; set; }

        #endregion Properties

        #region Constructors

        public CodeComponentViewModel(LanguagesEditorViewModel languageEditor, CodeComponentTypeModel component)
        {
            SetEditorSyntax();
            LanguageEditor = languageEditor;
            if (component != null)
            {
                IsCreating = false;
                Component = component;
                foreach (var field in Component.Fields)
                {
                    Fields.Add(new CodeFieldViewModel(field));
                }
            }
            else
            {
                IsCreating = true;
                Component = new CodeComponentTypeModel();
            }
        }

        #endregion Constructors

        #region Methods

        protected override void AddCommands()
        {
            base.AddCommands();
            CreateComponentCommand = new RelayCommand(o =>
            {
                if (!string.IsNullOrWhiteSpace(Component.Name))
                {
                    OnCreate?.Invoke(this, EventArgs.Empty);
                    IsCreating = false;
                }
                else
                {
                    MessageBox.Show("Component name cannot be empty!");
                }
            });

            #region True | False Patterns Commands

            AddTruePatternCommand = new RelayCommand(o => Component.TruePatterns.Add(new CodePatternModel()));
            AddFalsePatternCommand = new RelayCommand(o => Component.FalsePatterns.Add(new CodePatternModel()));
            RemoveTruePatternCommand = new RelayCommand(o => Component.TruePatterns.RemoveAt((int)o));
            RemoveFalsePatternCommand = new RelayCommand(o => Component.FalsePatterns.RemoveAt((int)o));

            #endregion True | False Patterns Commands

            #region Fields Commands

            AddFieldCommand = new RelayCommand(o =>
            {
                var field = new CodeFieldTypeModel();
                Component.Fields.Add(field);
                Fields.Add(new CodeFieldViewModel(field));
            });
            RemoveFieldCommand = new RelayCommand(o =>
            {
                var fieldVM = o as CodeFieldViewModel;
                Fields.Remove(fieldVM);
                Component.Fields.Remove(fieldVM.Field);
            });

            #endregion Fields Commands

            #region Delimiter Commands

            AddDelimiterCommand = new RelayCommand(o =>
            {
                Component.CodeDelimiters.Add(new CodeDelimiterModel());
            });
            RemoveDelimiterCommand = new RelayCommand(o =>
            {
                Component.CodeDelimiters.Remove(o as CodeDelimiterModel);
            });

            #endregion Delimiter Commands

            #region SubComponents Commands

            AddSubCommand = new RelayCommand(o =>
            {
                var (s, t) = o as Tuple<object, object>;
                if (s is CodeComponentTypeModel && t is CodeComponentViewModel)
                {
                    CodeComponentTypeModel source = s as CodeComponentTypeModel;
                    if (!Component.SubComponents.Contains(source.Name))
                        Component.SubComponents.Add(source.Name);
                }
            });
            RemoveSubCommand = new RelayCommand(o =>
            {
                var (s, t) = o as Tuple<object, object>;
                if (s is string && t is CodeComponentViewModel)
                {
                    string source = s as string;
                    if (Component.SubComponents.Contains(source))
                        Component.SubComponents.Remove(source);
                }
            });
            SwapSubCommand = new RelayCommand(o =>
            {
                var (s, t) = o as Tuple<object, object>;
                if (s is string && t is string)
                {
                    string source = s as string, target = t as string;
                    if (Component.SubComponents.Contains(source))
                        Component.SubComponents.Swap(source, target);
                }
            });

            #endregion SubComponents Commands
        }

        private void SetEditorSyntax()
        {
            string path = $"{Directory.GetCurrentDirectory()}\\Resources\\EditorSyntax\\PlantUMLBuilder.xshd";
            using (XmlTextReader reader = new XmlTextReader(path))
            {
                Syntax = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        #endregion Methods
    }
}