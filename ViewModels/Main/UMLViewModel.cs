using MVVMLibrary.ViewModels;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WPFLibrary.Commands;

namespace UMLGenerator.ViewModels.Main
{
    public class UMLViewModel : BaseGridColumnViewModel
    {
        #region Commands

        public RelayCommand SwitchViewCommand { get; private set; }
        public RelayCommand SwitchLocalOrRemoteCommand { get; private set; }
        public RelayCommand CopyToClipboardCommand { get; private set; }

        #endregion Commands

        #region Properties

        private string plantUml;

        public string PlantUml
        {
            get { return plantUml; }
            set { plantUml = value; NotifyPropertyChanged(); }
        }

        private bool isUmlView = true;

        public bool IsUmlView
        {
            get { return isUmlView; }
            set { isUmlView = value; NotifyPropertyChanged(); }
        }

        private BitmapImage imageSource;

        public BitmapImage ImageSource
        {
            get { return imageSource; }
            set { imageSource = value; NotifyPropertyChanged(); }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; NotifyPropertyChanged(); }
        }

        private bool isLocal;

        public bool IsLocal
        {
            get { return isLocal; }
            set { isLocal = value; NotifyPropertyChanged(); }
        }

        private bool isLoading = false;

        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; NotifyPropertyChanged(); }
        }

        private string svgString;

        public string SvgString
        {
            get { return svgString; }
            set { svgString = value; NotifyPropertyChanged(); }
        }

        private Bitmap umlImage;

        public Bitmap UMLImage
        {
            get { return umlImage; }
            set { umlImage = value; NotifyPropertyChanged(); }
        }

        #endregion Properties

        #region Fields

        private CancellationTokenSource cancellationTokenSource;
        private readonly MainViewModel mainVM;
        private readonly bool isJavaInstalled;

        #endregion Fields

        #region Constructors

        public UMLViewModel(MainViewModel mainVM) : base(300, new GridLength(1, GridUnitType.Star), false, true)
        {
            this.mainVM = mainVM;
            isJavaInstalled = CheckIsJavaInstalled();
            IsLocal = isJavaInstalled;
        }

        #endregion Constructors

        #region Methods

        protected override void AddCommands()
        {
            base.AddCommands();
            SwitchViewCommand = new RelayCommand(o => IsUmlView = !IsUmlView);
            CopyToClipboardCommand = new RelayCommand(o =>
            {
                System.Windows.Clipboard.SetText(PlantUml);
                mainVM.SetStatus("Copy PlantUML To Clipboard | Succeed", System.Windows.Media.Brushes.Blue, 2000);
            });
            SwitchLocalOrRemoteCommand = new RelayCommand(o =>
            {
                if (isJavaInstalled)
                    IsLocal = !IsLocal;
                else
                    System.Windows.MessageBox.Show
                    ("Changing to local UML generating Failed : Java isn't installed or ain't registered in the environment variables!!");
            });
        }

        public void UpdateUML(string plantUML)
        {
            ResetUMLProperties(plantUML);
            IsLoading = true;
            SetUml();
            IsLoading = false;
        }

        private static bool CheckIsJavaInstalled()
        {
            var javaEnvironmentVar = Environment.GetEnvironmentVariable("JAVA_HOME");
            var key32 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32)
                .OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment");
            var key64 = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64)
                .OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment");

            return (key32 != null || key64 != null) && javaEnvironmentVar != null;
        }

        private void ResetUMLProperties(string plantUML)
        {
            PlantUml = plantUML;
            if (IsLoading)
            {
                cancellationTokenSource.Cancel();
                while (IsLoading) ;
            }
            UMLImage = null;
            ImageSource = null;
            SvgString = "";
            Message = "Loading ...";
            cancellationTokenSource = new CancellationTokenSource();
        }

        private void SetUml()
        {
            Task.Run(async () =>
            {
                try
                {
                    if (IsLocal)
                    {
                        SvgString = await Libraries.PlantUMLMethods.GetLocalSVG(PlantUml);

                        UMLImage = Libraries.PlantUMLMethods.GetLocalPNG(PlantUml);
                        ImageSource = UMLImage.BitmapToImageSource(System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else
                    {
                        SvgString = await Libraries.PlantUMLMethods.GetRemoteSVG(PlantUml, cancellationTokenSource.Token);
                        var svg = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(SvgString);
                        System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            UMLImage = svg.Draw();
                            ImageSource = UMLImage.BitmapToImageSource(System.Drawing.Imaging.ImageFormat.Png);
                        });
                    }
                }
                catch (Exception exception)
                {
                    Message = $"Failed Getting UML | {exception.Message}";
                }
            });
        }

        #endregion Methods
    }
}