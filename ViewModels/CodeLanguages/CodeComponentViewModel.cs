using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using UMLGenerator.Models.CodeModels;
using WPFLibrary;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.CodeLanguages
{
    public class CodeComponentViewModel : BaseViewModel
    {
        #region Events
        public event EventHandler OnCreate;
        #endregion

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


        #endregion
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



        public CodeComponentTypeModel Component { get; set; }
        public ObservableCollection<CodeFieldViewModel> Fields { get; set; } = new ObservableCollection<CodeFieldViewModel>();
        public LanguagesEditorViewModel LanguageEditor { get; set; }
        public CodeFieldTypeModel SelectedCommonField
        {
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
        #endregion


        #region Constructors
        public CodeComponentViewModel(LanguagesEditorViewModel languageEditor, CodeComponentTypeModel component)
        {
            SetEditorSyntax();
            this.LanguageEditor = languageEditor;
            if(component != null)
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
        #endregion

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

            AddTruePatternCommand = new RelayCommand(o => Component.TruePatterns.Add(new CodePatternModel()));
            AddFalsePatternCommand = new RelayCommand(o => Component.FalsePatterns.Add(new CodePatternModel()));
            RemoveTruePatternCommand = new RelayCommand(o =>  Component.TruePatterns.RemoveAt((int)o));
            RemoveFalsePatternCommand = new RelayCommand(o => Component.FalsePatterns.RemoveAt((int)o));
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
            AddDelimiterCommand = new RelayCommand(o =>
            {
                Component.CodeDelimiters.Add(new CodeDelimiterModel());
            });
            RemoveDelimiterCommand = new RelayCommand(o =>
            {
                Component.CodeDelimiters.Remove(o as CodeDelimiterModel);
            });
            AddSubCommand = new RelayCommand(o => 
            {
                var (s, t) = o as Tuple<object, object>;
                if(s is CodeComponentTypeModel && t is CodeComponentViewModel)
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
                if(s is string && t is string)
                {
                    string source = s as string, target = t as string;
                    if(Component.SubComponents.Contains(source))
                        Component.SubComponents.Swap(source, target);
                }
            });
        }
        private void SetEditorSyntax()
        {
            string path = $"{Directory.GetCurrentDirectory()}\\Resources\\EditorSyntax\\PlantUMLBuilder.xshd";
            using (XmlTextReader reader = new XmlTextReader(path))
            {
                Syntax = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }
        #endregion

    }
}
