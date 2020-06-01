using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFZooming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TransformGroup transformGroup;
        private TranslateTransform translation;
        private ScaleTransform scale;
        private bool isScale;
        private double previousCumlativeScale;
        private double previousTranslateX;
        private double previousTranslateY;
        private double previousXPosition;
        private double previousYPosition;

        public MainWindow()
        {
            InitializeComponent();

            translation = new TranslateTransform(0, 0);
            scale = new ScaleTransform(1, 1);

            transformGroup = new TransformGroup();
            transformGroup.Children.Add(scale);
            transformGroup.Children.Add(translation);
            this.JPGImage.RenderTransform = transformGroup;
        }

        private void JPGImage_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (scale.ScaleX > 1)
                isScale = true;
        }

        private void Image_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.DeltaManipulation.Translation.X + " " + e.DeltaManipulation.Translation.Y);
            if (e.Manipulators.Count() > 1)
            {
                if (scale.ScaleX > 1.6 || scale.ScaleX < 0.5)
                {
                    scale.ScaleX = 1;
                    scale.ScaleY = 1;
                    translation.X = 0;
                    translation.Y = 0;
                    this.JPGImage.RenderTransform = transformGroup;
                }
                else
                {
                    if (isScale)
                    {
                        scale.ScaleX = previousCumlativeScale * e.DeltaManipulation.Scale.X;
                        scale.ScaleY = previousCumlativeScale * e.DeltaManipulation.Scale.Y;
                        var x = (e.ManipulationOrigin.X - previousXPosition) * (scale.ScaleX - 1);
                        var y = (e.ManipulationOrigin.Y - previousYPosition) * (scale.ScaleY - 1);
                        translation.X += x;
                        translation.Y += y;
                    }
                    else
                    {
                        scale.ScaleX *= e.DeltaManipulation.Scale.X;
                        scale.ScaleY *= e.DeltaManipulation.Scale.Y;
                        translation.X += e.DeltaManipulation.Translation.X;
                        translation.Y += e.DeltaManipulation.Translation.Y;
                    }

                    scale.CenterX = e.ManipulationOrigin.X;
                    scale.CenterY = e.ManipulationOrigin.Y;
                    this.JPGImage.RenderTransform = transformGroup;
                }

                previousCumlativeScale = scale.ScaleX;
                previousTranslateX = translation.X;
                previousTranslateY = translation.Y;
                previousXPosition = scale.CenterX;
                previousYPosition = scale.CenterY;
            }
            else
            {
                translation.X += e.DeltaManipulation.Translation.X;
                translation.Y += e.DeltaManipulation.Translation.Y;
            }
        }

        private void JPGImage_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (isScale)
            {
                isScale = false;
            }
        }

    }
}
