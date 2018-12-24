using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace image_sizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            TopMost = true;
            btnPin.Text = "Unpin";

        }

        bool useX = true;
        bool saveWithNewName = true;
        int res = 600;
        long quality = 80;
        ImageFormat imageFormat;

        private void button1_Click(object sender, EventArgs e)
        {
            if (TopMost)
            {
                btnPin.Text = "Pin";
                TopMost = false;
            }
            else
            {
                btnPin.Text = "Unpin";
                TopMost = true;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            quality = trackBar1.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            res = (int)numericUpDown1.Value;
        }

        private void radioButtonUseX_CheckedChanged(object sender, EventArgs e)
        {
            useX = !useX;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            saveWithNewName = !saveWithNewName;
        }

        //Dragdrop sttuff

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Html) || e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            foreach(string file in files)
            {
                resizeImage(file).Wait();
            }


        }

        private Task resizeImage(string imagepath)
        {
            Task t = Task.Factory.StartNew(() =>
            {
                Bitmap image = new Bitmap(imagepath);

                if ((useX && res > image.Width) || (!useX && res > image.Height))
                    return;

                int width = useX ? res : res * image.Width / image.Height;
                int height = useX ? res * image.Height / image.Width : res;

                Bitmap destImage = new Bitmap(image, new Size(width, height));

                //Rectangle destRect = new Rectangle(0, 0, width, height);

                //using (var graphics = Graphics.FromImage(destImage))
                //{
                //    graphics.CompositingMode = CompositingMode.SourceCopy;
                //    graphics.CompositingQuality = CompositingQuality.HighQuality;
                //    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //    graphics.SmoothingMode = SmoothingMode.HighQuality;
                //    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                //    using (var wrapMode = new ImageAttributes())
                //    {
                //        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                //        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                //    }
                //}

                //Save as jpg
                ImageCodecInfo encoder = GetEncoder(imageFormat);

                // Create an Encoder object based on the GUID  
                // for the Quality parameter category.  
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                // Create an EncoderParameters object.  
                // An EncoderParameters object has an array of EncoderParameter  
                // objects. In this case, there is only one  
                // EncoderParameter object in the array.  
                EncoderParameters myEncoderParameters = new EncoderParameters(1);

                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;

                if (saveWithNewName)
                    destImage.Save(imagepath + "_resize." + imageFormat.ToString().ToLower(), encoder, myEncoderParameters);
                else
                    destImage.Save(imagepath, encoder, myEncoderParameters);

            });
            return t;
        }

        ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedText == "PNG")
            {
                imageFormat = ImageFormat.Png;
            }
            else if (comboBox1.SelectedText == "JPG")
            {
                imageFormat = ImageFormat.Jpeg;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            imageFormat = ImageFormat.Png;

        }
    }
}
