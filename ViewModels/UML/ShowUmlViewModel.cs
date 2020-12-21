using System;
using System.Collections.Generic;
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

        #endregion

        #region Fields
        private CancellationTokenSource cancellationTokenSource;
        private bool isLoading = false;
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
            if (isLoading)
            {
                cancellationTokenSource.Cancel();
                while (isLoading) ;
            }
            ImageSource = null;
            Message = "Loading ...";
            cancellationTokenSource = new CancellationTokenSource();
            isLoading = true;
            await Task.Run(() =>
            {
                try
                {
                    var svgString = Libraries.PlantUMLMethods.GetSVG(plantUml, cancellationTokenSource.Token);
                    var svg = Svg.SvgDocument.FromSvg<Svg.SvgDocument>(svgString);
                    System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke( () =>
                        ImageSource = svg.Draw().BitmapToImageSource(System.Drawing.Imaging.ImageFormat.Png));
                }
                catch
                {
                    Message = "Failed Getting UML";
                    isLoading = false;
                }
            });
            isLoading = false;
        }
        #endregion
    }
}
