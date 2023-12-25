using ColorPicker.utils;
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

namespace ColorPicker
{

    public partial class MainWindow : Window
    {
        private Image selectedImage;
        /// <summary>
        /// 最新鼠标位置
        /// </summary>
        private Point lastMousePosition;
        /// <summary>
        /// 缩放倍率
        /// </summary>
        private double currentScale = 1.0;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ColorViewModel();
        }
        /// <summary>
        /// 画布图片拖放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    Uri imageUri = new Uri(files[0]);
                    AddImageToCanvas(imageUri);
                }
            }
        }
        /// <summary>
        /// 创建并添加image图片对象到canvas
        /// </summary>
        /// <param name="imageUri"></param>
        private void AddImageToCanvas(Uri imageUri)
        {
            BitmapImage bitmap = new BitmapImage(imageUri);

            Image image = new Image
            {
                Source = bitmap,
                Stretch = Stretch.Fill
            };

            // 计算缩放因子以使图像适合窗口中的图像
            double widthScale = ImageCanvas.ActualWidth / bitmap.PixelWidth;
            double heightScale = ImageCanvas.ActualHeight / bitmap.PixelHeight;
            double scaleFactor = Math.Min(widthScale, heightScale);

            image.Width = bitmap.PixelWidth * scaleFactor;
            image.Height = bitmap.PixelHeight * scaleFactor;

            // 将图像置于画布的中心
            Canvas.SetLeft(image, (ImageCanvas.ActualWidth - image.Width) / 2);
            Canvas.SetTop(image, (ImageCanvas.ActualHeight - image.Height) / 2);

            ImageCanvas.Children.Clear(); // 移除已有的子元素

            ImageCanvas.Children.Add(image);
            selectedImage = image;
            // 注册缩放和拖动事件
            selectedImage.MouseWheel += Image_MouseWheel;
            selectedImage.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            selectedImage.MouseMove += Image_MouseMove;
            selectedImage.MouseLeftButtonUp += Image_MouseLeftButtonUp;
            
        }

        /// <summary>
        /// 图片粘贴到剪贴板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            PasteImageFromClipboard();
        }
        /// <summary>
        /// 读取剪贴板的图片并粘贴到canvas上
        /// </summary>
        private void PasteImageFromClipboard()
        {
            if (Clipboard.ContainsImage())
            {
                Image image = new Image();
                BitmapSource clipboardImage = Clipboard.GetImage();

                image.Source = clipboardImage;

                // 计算缩放因子以使图像适合窗口中的图像
                double widthScale = ImageCanvas.ActualWidth / clipboardImage.PixelWidth;
                double heightScale = ImageCanvas.ActualHeight / clipboardImage.PixelHeight;
                double scaleFactor = Math.Min(widthScale, heightScale);

                image.Width = clipboardImage.PixelWidth * scaleFactor;
                image.Height = clipboardImage.PixelHeight * scaleFactor;


                // 将图像置于画布的中心
                Canvas.SetLeft(image, (ImageCanvas.ActualWidth - image.Width) / 2);
                Canvas.SetTop(image, (ImageCanvas.ActualHeight - image.Height) / 2);

                ImageCanvas.Children.Clear(); // 移除已有的子元素

                ImageCanvas.Children.Add(image);
                
                selectedImage = image;
                // 注册缩放和拖动事件
                selectedImage.MouseWheel += Image_MouseWheel;
                selectedImage.MouseLeftButtonDown += Image_MouseLeftButtonDown;
                selectedImage.MouseMove += Image_MouseMove;
                selectedImage.MouseLeftButtonUp += Image_MouseLeftButtonUp;

            }
        }


        /// <summary>
        /// 滚轮缩放图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (selectedImage != null)
            {
                Point mousePos = e.GetPosition(selectedImage);

                double scaleFactor = e.Delta > 0 ? 1.1 : 0.9;
                currentScale *= scaleFactor;

                ScaleTransform scale = new ScaleTransform(currentScale, currentScale, mousePos.X, mousePos.Y);
                selectedImage.RenderTransform = scale;
            }

            
        }
        /// <summary>
        /// 检查鼠标左键按下是否选中图片对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 选中图片才能拖动
            if (selectedImage != null)
            {
                lastMousePosition = e.GetPosition(ImageCanvas);
                selectedImage.CaptureMouse();
            }
        }
        /// <summary>
        /// 拖动图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedImage != null && selectedImage.IsMouseCaptured)
            {
                Point currentPosition = e.GetPosition(ImageCanvas);
                double deltaX = currentPosition.X - lastMousePosition.X;
                double deltaY = currentPosition.Y - lastMousePosition.Y;

                Canvas.SetLeft(selectedImage, Canvas.GetLeft(selectedImage) + deltaX);
                Canvas.SetTop(selectedImage, Canvas.GetTop(selectedImage) + deltaY);

                lastMousePosition = currentPosition;
            }
        }
        /// <summary>
        /// 松开鼠标，放下图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedImage != null)
            {
                selectedImage.ReleaseMouseCapture();
            }
        }
        //设置或获取canvas的颜色码
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            sender.ToString();
            // 获取鼠标位置
            Point point = e.GetPosition(colorCanvas);

            // 从鼠标位置获取颜色
            Color color = GetColorAtPoint(point);
            // 输出颜色码到文本框
            colorInput.Text = ColorToHex(color);
        }
        /// <summary>
        /// 从鼠标位置获取颜色
        /// </summary>
        /// <param name="point">鼠标位置</param>
        /// <returns>颜色对象</returns>
        private Color GetColorAtPoint(Point point)
        {
            // 创建一个 VisualBrush 以捕获 Canvas 的可视化内容
            VisualBrush brush = new VisualBrush(ImageCanvas);

            // 创建一个 DrawingVisual，用于绘制 VisualBrush
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(brush, null, new Rect(new Point(0, 0), ImageCanvas.RenderSize));
            }

            // 创建 RenderTargetBitmap 以捕获 DrawingVisual 的内容
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ImageCanvas.ActualWidth,
                                                                           (int)ImageCanvas.ActualHeight,
                                                                           96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(drawingVisual);

            // 将鼠标位置转换为画布坐标
            Point canvasPosition = Mouse.GetPosition(ImageCanvas);

            // 复制指定位置的颜色值
            byte[] colorBytes = new byte[4];
            renderTargetBitmap.CopyPixels(new Int32Rect((int)canvasPosition.X, (int)canvasPosition.Y, 1, 1), colorBytes, 4, 0);

            // 返回颜色
            return Color.FromArgb(colorBytes[3], colorBytes[2], colorBytes[1], colorBytes[0]);
        }
        /// <summary>
        /// 显示颜色到画布
        /// </summary>
        /// <param name="color"></param>
        private void DisplayColor(Color color)
        {
            colorDisplay.Fill = new SolidColorBrush(color);
        }



        private void colorInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 从文本框的颜色码设置颜色到画布
            Color parsedColor;
            if (ColorUtils.TryParseColor(colorInput.Text, out parsedColor))
            {
                DisplayColor(parsedColor);
                //设置颜色值显示到textblock上
                txtRedByte.Text = parsedColor.R.ToString();
                txtGreenByte.Text = parsedColor.G.ToString();
                txtBlueByte.Text = parsedColor.B.ToString();
                //归一化显示
                double r=parsedColor.R/255d;
                double g = parsedColor.G / 255d;
                double b = parsedColor.B / 255d;

                txtRedOne.Text = r.ToString("N2");
                txtGreenOne.Text = g.ToString("N2");
                txtBlueOne.Text = b.ToString("N2");
            }
        }
        /// <summary>
        /// 颜色对象转换为16进制
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// 滑块值改变时，设置16位颜色码到文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string red = redBar.Value.ToString("N2");
            string green = greenBar.Value.ToString("N2");
            string blue = blueBar.Value.ToString("N2");
            //转换为0-255代码
            Color color = toByteColor(red,green,blue);
            //在转换为16进制代码
            string hexColorStr = ColorToHex(color);
            //设置颜色码到文本框

            colorInput.Text = hexColorStr;
        }
        /// <summary>
        /// 将0-255数字转换成颜色对象
        /// </summary>
        /// <param name="red">0-1</param>
        /// <param name="green">0-1</param>
        /// <param name="blue">0-1</param>
        /// <returns></returns>
        private Color toByteColor(string red,string green,string blue)
        {
            double r = 255d * double.Parse(red);
            double g = 255d * double.Parse(green);
            double b = 255d * double.Parse(blue);

            Color color = Color.FromRgb((byte)Convert.ToInt32(r), (byte)Convert.ToInt32(g), (byte)Convert.ToInt32(b));

            return color;
        }

       
    }

   
}
