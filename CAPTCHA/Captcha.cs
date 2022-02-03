using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CAPTCHA
{
    class Captcha
    {
        /// <summary>
        /// Реализация капчи на языке C#
        /// </summary>
        const int MAX_CAPTHCA_LENGHT = 5;
        const int CAPTCHA_WIDTH = 400;
        const int CAPTCHA_HEIGHT = 160;
        const int MIN_FONT_SIZE = 45;
        const int MAX_FONT_SIZE = 55;
        const double DISTORTION_STRENGHT = 1.5;


        public Bitmap Bitmap { get; private set; }
        public BitmapSource BitmapSource
        {
            get
            {
                var bitData = Bitmap.LockBits(new System.Drawing.Rectangle(0, 0, Bitmap.Width, Bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, Bitmap.PixelFormat);
                var bitMapSource = BitmapSource.Create(bitData.Width, bitData.Height, 96, 96, PixelFormats.Bgr32, null, bitData.Scan0, bitData.Stride * bitData.Height, bitData.Stride);
                Bitmap.UnlockBits(bitData);
                return bitMapSource;
            }
        }
        private Graphics graphics;
        private List<System.Drawing.Drawing2D.LinearGradientBrush> backgroundCapchaLinearGradients;

        public string Value { get; private set; }
        public Captcha()
        {
            Bitmap = new Bitmap(CAPTCHA_WIDTH, CAPTCHA_HEIGHT);
            graphics = Graphics.FromImage(Bitmap);


            //Инициализируем коллекцию градиентов
            backgroundCapchaLinearGradients = new List<System.Drawing.Drawing2D.LinearGradientBrush>
            {
                new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Point(0, 0), new System.Drawing.Point(CAPTCHA_WIDTH, 0), System.Drawing.Color.FromArgb(144, 0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0)),
                new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Point(CAPTCHA_WIDTH, 0), new System.Drawing.Point(0, 0), System.Drawing.Color.FromArgb(144, 0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0)),
                new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Point(0, 0), new System.Drawing.Point(0, CAPTCHA_HEIGHT), System.Drawing.Color.FromArgb(144, 0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0)),
                new System.Drawing.Drawing2D.LinearGradientBrush(new System.Drawing.Point(0, CAPTCHA_HEIGHT), new System.Drawing.Point(0, 0), System.Drawing.Color.FromArgb(144, 0, 0, 0), System.Drawing.Color.FromArgb(0, 0, 0, 0))
            };

            GenerationCaptcha();
        }
        /// <summary>
        /// Генерация капчи
        /// </summary>
        /// 
        public void GenerationCaptcha()
        {
            //сбрасываем капчу
            graphics.Clear(System.Drawing.Color.FromArgb(0, 0, 0, 0));

            Random rand = new Random(); //объект для получения случайных значений
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Brushes.Black) { Width = 2 }; //перо для отрисовки линий

            //Генерируем случаную строку
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Value = new string(chars.Select(c => chars[rand.Next(chars.Length)]).Take(MAX_CAPTHCA_LENGHT).ToArray());

            //отрисовка линий за текстом
            int count_line = rand.Next(3, 5);
            for (int i = 0; i < count_line; i++)
            {
                graphics.DrawLine(pen, new PointF(rand.Next(0, 30), rand.Next(70, CAPTCHA_HEIGHT)), new PointF(rand.Next(350, CAPTCHA_WIDTH), rand.Next(70, CAPTCHA_HEIGHT)));
            }
            //Отрисовка символов
            PointF startPoint = new PointF(CAPTCHA_WIDTH * 0.15f, CAPTCHA_HEIGHT / 2 - (MAX_FONT_SIZE + 2) / 1.5f);
            foreach (char c in Value)
            {
                int rand_angel = rand.Next(-8, 8);
                graphics.RotateTransform(rand_angel);
                Font font = new Font("Times New Roman", rand.Next(MIN_FONT_SIZE, MAX_FONT_SIZE));
                graphics.DrawString(c.ToString(), font, new SolidBrush(System.Drawing.Color.FromArgb(rand.Next(155, 255), 0, 0, 0)), startPoint.X, startPoint.Y + rand.Next(-15, 15));
                startPoint.X += font.Size * 1.1f;
                graphics.RotateTransform(rand_angel * (-1));
            }
            //отрисовка линий над текстом
            count_line = rand.Next(1, 3);
            for (int i = 0; i < count_line; i++)
            {
                graphics.DrawLine(pen, new PointF(rand.Next(0, 30), rand.Next(70, CAPTCHA_HEIGHT)), new PointF(rand.Next(350, CAPTCHA_WIDTH), rand.Next(70, CAPTCHA_HEIGHT)));
            }
            Bitmap distortionBitmap = new Bitmap(CAPTCHA_WIDTH, CAPTCHA_HEIGHT); //заводим переменную для искаженного изображения
            //искажаем изображение
            for (int x = 0; x < CAPTCHA_WIDTH; x++)
            {
                for (int y = 0; y < CAPTCHA_HEIGHT; y++)
                {
                    double uvX = (double)x / CAPTCHA_WIDTH;
                    double uvY = (double)y / CAPTCHA_HEIGHT;
                    double dx = uvX - 0.5;
                    double dy = uvY - 0.5;

                    double r = Math.Sqrt(dx * dx + dy * dy);
                    double a = Math.Atan2(dy, dx);
                    double rn = Math.Pow(r, DISTORTION_STRENGHT) / 0.5;

                    double newX = rn * Math.Cos(a) + 0.5;
                    double newY = rn * Math.Sin(a) + 0.5;

                    int normX = (int)(newX * CAPTCHA_WIDTH);
                    int normY = (int)(newY * CAPTCHA_HEIGHT);

                    if (normX >= 0 && normX < CAPTCHA_WIDTH && normY >= 0 && normY < CAPTCHA_HEIGHT)
                    {
                        distortionBitmap.SetPixel(x, y, Bitmap.GetPixel(normX, normY));
                    }
                }
            }
            graphics.Clear(System.Drawing.Color.White);
            graphics.FillRectangle(backgroundCapchaLinearGradients[rand.Next(0, 3)], new System.Drawing.Rectangle(0, 0, Bitmap.Width, Bitmap.Height)); //отрисовываем градиент
            graphics.DrawImage(distortionBitmap, 0, 0); //рисуем искаженные символы с линиями
        }
    }
}
