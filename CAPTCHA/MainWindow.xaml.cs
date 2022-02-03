using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace CAPTCHA
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        Captcha captcha;
        public MainWindow()
        {
            InitializeComponent();
            captcha = new Captcha();
            captchaImage.DataContext = captcha;
            captchaImage.Source = captcha.BitmapSource;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            captcha.GenerationCaptcha();
            captchaImage.Source = captcha.BitmapSource;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (captcha.Value == captchaTextBox.Text)
                MessageBox.Show("Капча введена верно");
            else
            {
                captchaTextBox.Clear();
                captcha.GenerationCaptcha();
                captchaImage.Source = captcha.BitmapSource;
                MessageBox.Show("Капча введена неверно");
            }
        }
    }
}
