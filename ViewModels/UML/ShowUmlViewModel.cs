using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UMLGenerator.ViewModels.UML
{
    public class ShowUmlViewModel : BaseViewModel
    {
        #region Properties
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
        public bool IsLoading { get; set; } = false;
        public string SvgString { get; set; }
        public Bitmap UMLImage { get; set; }
        #endregion

        #region Fields
        private CancellationTokenSource cancellationTokenSource;
        #endregion

        #region Constructors
        public ShowUmlViewModel(string plantUml)
        {
            UpdateUML(plantUml);
        }
        #endregion

        #region Methods
        public async void UpdateUML(string plantUml)
        {
            if (IsLoading)
            {
                cancellationTokenSource.Cancel();
                while (IsLoading) ;
            }
            ImageSource = null;
            Message = "Loading ...";
            cancellationTokenSource = new CancellationTokenSource();
            IsLoading = true;
            await Task.Run(() =>
            {
                try
                {
                    SvgString = Libraries.PlantUMLMethods.GetSVG(plantUml, cancellationTokenSource.Token);
                    var svg = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(SvgString);

                    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke( () => 
                    {
                        UMLImage = svg.Draw();
                        ImageSource = UMLImage.BitmapToImageSource(System.Drawing.Imaging.ImageFormat.Png);
                    });
                        
                }
                catch
                {
                    Message = "Failed Getting UML";
                    IsLoading = false;
                }
            });
            IsLoading = false;
        }
        #endregion
    }
}
