
using Microsoft.Win32;
using ProjektASM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
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
using static System.Net.Mime.MediaTypeNames;
using Color = System.Windows.Media.Color;

namespace AssmblerRGB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool highLevelLanguage=true;
        private Color colorRGB=new Color();
        private Bitmap bitmap=new Bitmap(800,600);
        private Bitmap bitmapToSafe=new Bitmap(800,600);
        
        private uint numberOfThreads = 1;
        //[DllImport(@"C:\Users\adria\source\repos\ProjektASM\x64\Debug\JAAsm.dll")]

        //static extern int MyProc1(int a, int b);

        public MainWindow()
        {
            InitializeComponent();
        }
        private async void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
            try
            {
                Color color = Color.FromRgb((byte)slColorR.Value, (byte)slColorG.Value, (byte)slColorB.Value);
                this.colorRGB = color;
            }
            catch(Exception ) { }
                
        }
        private async void ThreadSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            try
            {
                numberOfThreads = (uint)(int)slValue.Value;     
            }
            catch (Exception) { }

        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
  
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.ShowDialog();
                bitmap = (Bitmap)Bitmap.FromFile(openFileDialog.FileName);
                ImageOne.Source = Convert(bitmap);
                Button2.IsEnabled = true;
            }
            catch (Exception)
            {
                MessageBox.Show("Please provide a photo in the correct format!!!");
            }
        }
        private BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //int x = 63, y = 6;
            //int retVal = MyProc1(x, y);
            Button3.IsEnabled = true;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            bitmapToSafe = bitmap.ColorBalanceParallel(colorRGB.B, colorRGB.G, colorRGB.R, numberOfThreads, highLevelLanguage);  
            watch.Stop();
            ImageTwo.Source = Convert(bitmapToSafe);
            timeLabel.Content = "Time: " + watch.Elapsed.Milliseconds.ToString() + " ms";
            //MessageBox.Show("Asm test: " + retVal.ToString() + highLevelLanguage.ToString());

        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {     
                bitmapToSafe.Save(@"C:\Users\adria\Desktop\savedImages\changedImage2.png", ImageFormat.Png);
                // bitmap.Save("D:\\Pobrane\\blur3.png");
                MessageBox.Show("Your photo has been saved!", "Success");
            }
            catch(Exception)
            {
                MessageBox.Show("Incorrect localization");
            }
           
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            highLevelLanguage = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            highLevelLanguage = false;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}
