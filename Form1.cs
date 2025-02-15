using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmsDecodeArt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int textSize = 4000;

        private void Form1_Load(object sender, EventArgs e)
        {        
            //Calculate lengths
            float degAngle = 18.0F;
            double radAngle = (degAngle * (Math.PI / 180));
            double cosAngle = Math.Cos(radAngle);
            double sinAngle = Math.Sin(radAngle);
            int imageSize = (int) Math.Floor(textSize / (cosAngle + sinAngle));
            int sizeDiff = (textSize - imageSize) / 2;

            //Setup brushes and fonts
            Bitmap bmp = new Bitmap(textSize, textSize, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            Graphics surface = Graphics.FromImage(bmp);
            Font emojiFont = new Font("Segoe UI Emoji", 8, FontStyle.Bold);
            SolidBrush blackBrush = new SolidBrush(Color.Black);
            SolidBrush whiteBrush = new SolidBrush(Color.White);
            Rectangle rectangle = new Rectangle(-textSize/2, -textSize/2, textSize, textSize);

            //shift to central co-ordinate system and fill background
            surface.TranslateTransform(textSize/2, textSize/2);
            surface.FillRectangle(whiteBrush, rectangle);

            //read text file from the SMSDecode project
            string filePath = "..\\..\\..\\..\\SmsDecode\\smsMessageString.txt";
            System.IO.StreamReader streamReader = new System.IO.StreamReader(filePath);
            string text = streamReader.ReadToEnd();

            //rotate about origin (now in centre) and draw text
            surface.RotateTransform(degAngle);
            surface.DrawString(text, emojiFont, blackBrush, rectangle);

            //recolour each pixel that is within bounds
            //turning black & white to red & white
            Color pixelColor = new Color();
            for (int x = 0; x < textSize; x++)
            {
                for (int y = 0; y < textSize; y++)
                {
                    if (HeartShape(x, y))
                    {
                        pixelColor = bmp.GetPixel(x, y);
                        bmp.SetPixel(x, y, Color.FromArgb(pixelColor.A, 255, pixelColor.G, pixelColor.B));                      
                    }
                }
            }

            //Take smaller square to make complete image. Save and Close
            Rectangle imageRec = new Rectangle(sizeDiff, sizeDiff, imageSize, imageSize);
            Bitmap imageBmp = bmp.Clone(imageRec, System.Drawing.Imaging.PixelFormat.DontCare);
            string savePath = "..\\..\\..\\smsArt.jpg";
            imageBmp.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            this.Close();
        }

        // check if point is within the heart shape
        // upper bound y<= sqrt(1-((abs(x)-1)^2))
        // lower bound y>= -3*(sqrt(1-((sqrt(abs(x)))/(sqrt2))))
        private bool HeartShape (int x, int y)
        {
            double heartSF = 1.4;
            double xOld = (double)x;
            double yOld = (double)y;
            //transform x,y co-ordinates
            double xNew = (((4*xOld)/textSize)-2)*heartSF;
            double yNew = (((-4*yOld)/textSize)+1.3)*heartSF;

            if (  yNew <= Math.Sqrt((1-((Math.Abs(xNew)-1)* (Math.Abs(xNew) - 1)))) 
                && yNew >= -3*(Math.Sqrt(1-((Math.Sqrt(Math.Abs(xNew)))/(Math.Sqrt(2))))))
            {
                return true;
            }
            return false;
        }



    }
}
