using MVVMLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using UMLGenerator.Models.CodeModels;
using UMLGenerator.ViewModels.Main;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.CodeLanguages
{
    public class LanguagesEditorViewModel : BaseGridColumnViewModel
    {
        #region Commands

        public RelayCommand AddLanguageCommand { get; private set; }
        public RelayCommand EditLanguageCommand { get; private set; }
        public RelayCommand SaveLanguageCommand { get; private set; }
        public RelayCommand RemoveLanguageCommand { get; private set; }
        public RelayCommand AddComponentCommand { get; private set; }
        public RelayCommand RemoveComponentCommand { get; private set; }

        #endregion

        #region Properties
        public ObservableCollection<string> Languages { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<CodeComponentTypeModel> Components { get; set; } = new ObservableCollection<CodeComponentTypeModel>();

        private string selectedLanguageName;

        public string SelectedLanguageName
        {
            get { return selectedLanguageName; }
            set
            {
                selectedLanguageName = value;
                if(value != null)
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
                Components.Clear();
                foreach(var comp in value.Components)
                {
                    Components.Add(comp.Value);
                }
                EditLanguageCommand?.Execute(value.Name);
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
                if(value != null)
                    SelectedViewModel = new CodeComponentViewModel(this, value);
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

        #region Fields
        private MainViewModel mainVM;
        #endregion

        #region Constructors
        public LanguagesEditorViewModel(MainViewModel mainVM) : base(300, new System.Windows.GridLength(1, System.Windows.GridUnitType.Star))
        {
            this.mainVM = mainVM;
            LoadData();
        }
        #endregion

        #region Methods
        protected override void AddCommands()
        {
            base.AddCommands();
            AddLanguageCommand = new RelayCommand(o => 
            {
                var newVM = new CodeLanguageViewModel(this, null);
                newVM.OnCreate += (s, e) =>
                {
                    File.WriteAllText($"{Directory.GetCurrentDirectory()}\\Languages\\{newVM.Language.Name}.json", JsonSerializer.Serialize(newVM.Language));
                    Languages.Add(newVM.Language.Name);
                    SelectedLanguageName = newVM.Language.Name;
                };
                SelectedViewModel = newVM;
            });
            SaveLanguageCommand = new RelayCommand(o => 
            {
                SelectedLanguage.Components.Clear();
                bool validate = true;
                foreach (var comp in Components)
                {
                    if(SelectedLanguage.Components.ContainsKey(comp.Name))
                    {
                        MessageBox.Show("Language cannot have two or more Components with the same name !");
                        validate = false;
                        break;
                    }
                    SelectedLanguage.Components.Add(comp.Name, comp);
                }
                if(string.IsNullOrWhiteSpace(SelectedLanguage.Name))
                {
                    MessageBox.Show("Please decalre a name!");
                    validate = false;
                }
                if(validate)
                {
                    File.Delete($"{Directory.GetCurrentDirectory()}\\Languages\\{o as string}.json");
                    File.WriteAllText($"{Directory.GetCurrentDirectory()}\\Languages\\{o as string}.json", JsonSerializer.Serialize(SelectedLanguage));
                    mainVM.SetStatus($"{o as string} language saved", System.Windows.Media.Brushes.Blue, 2000);
                }
            });
            EditLanguageCommand = new RelayCommand(o => 
            {
                SelectedComponent = null;
                var newVM = new CodeLanguageViewModel(this, SelectedLanguage);
                SelectedViewModel = newVM;
            });
            RemoveLanguageCommand = new RelayCommand(o => 
            { 
                if(MessageBox.Show($"Are you sure you want to delete {o as string}?", "Delete Language", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete($"{Directory.GetCurrentDirectory()}\\Languages\\{o as string}.json");
                    Languages.Remove(o as string);
                }
            });

            AddComponentCommand = new RelayCommand(o => 
            {
                var newVM = new CodeComponentViewModel(this, null);
                newVM.OnCreate += (s, e) =>
                {
                    Components.Add(newVM.Component);
                };
                SelectedViewModel = newVM;
            });
            RemoveComponentCommand = new RelayCommand(o =>
            {
                Components.Remove(o as CodeComponentTypeModel);
            }
            );
        }

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
    }
}
