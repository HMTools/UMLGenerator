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
        public RelayCommand OpenLanguagesFolderCommand { get; private set; }
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
        private readonly MainViewModel mainVM;
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
            OpenLanguagesFolderCommand = new RelayCommand(o => 
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"{Directory.GetCurrentDirectory()}\\Languages\\");
                }
                catch (System.ComponentModel.Win32Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            });
            #region Languages Commands
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
                SaveLanguage(o as string);
            });
            EditLanguageCommand = new RelayCommand(o =>
            {
                SelectedComponent = null;
                var newVM = new CodeLanguageViewModel(this, SelectedLanguage);
                SelectedViewModel = newVM;
            });
            RemoveLanguageCommand = new RelayCommand(o =>
            {
                if (MessageBox.Show($"Are you sure you want to delete {o as string}?", "Delete Language", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete($"{Directory.GetCurrentDirectory()}\\Languages\\{o as string}.json");
                    Languages.Remove(o as string);
                    if (Languages.Count > 0)
                        SelectedLanguageName = Languages[0];
                }
            });
            #endregion
            #region Components Commands
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
            });
            #endregion
        }
        private void SaveLanguage(string originalLanguageName)
        {
            //SelectedLanguage.Components.Clear();
            if(string.IsNullOrWhiteSpace(SelectedLanguage.Name))
            {
                MessageBox.Show("Please decalre a name!");
                return;
            }
            if(originalLanguageName != SelectedLanguage.Name && Languages.Contains(SelectedLanguage.Name))
            {
                MessageBox.Show("There is already a language object with the same name!");
                return;
            }
            if(Components.GroupBy(comp => comp.Name).Any(group => group.Count() > 1))
            {
                MessageBox.Show("Language object cannot have two or more components with the same name !");
                return;
            }

            SelectedLanguage.Components.Clear();
            foreach (var comp in Components)
                SelectedLanguage.Components.Add(comp.Name, comp);

            File.Delete($"{Directory.GetCurrentDirectory()}\\Languages\\{originalLanguageName}.json");
            File.WriteAllText($"{Directory.GetCurrentDirectory()}\\Languages\\{SelectedLanguage.Name}.json", JsonSerializer.Serialize(SelectedLanguage));
            mainVM.SetStatus($"{SelectedLanguage.Name} language saved", System.Windows.Media.Brushes.Blue, 2000);
            if (originalLanguageName != SelectedLanguage.Name)
            {
                Languages.Add(SelectedLanguage.Name);
                SelectedLanguageName = SelectedLanguage.Name;
                Languages.Remove(originalLanguageName);
            }
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
