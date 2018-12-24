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
            if (comboBoxImageFormat.SelectedText == "PNG")
            {
                imageFormat = ImageFormat.Png;
                trackBar1.Enabled = false;
            }
            else if (comboBoxImageFormat.SelectedText == "JPG")
            {
                imageFormat = ImageFormat.Jpeg;
                trackBar1.Enabled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxImageFormat.SelectedIndex = 0;
            imageFormat = ImageFormat.Png;

            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;

            toolTip1.SetToolTip(radioButtonUseX, "If the images width is larger than \"New max res\" it gets set to \"New max res\". the height will be scaled accordingly");
            toolTip1.SetToolTip(radioButtonUseY, "If the images height is larger than \"New max res\" it gets set to \"New max res\". the width will be scaled accordingly");
            toolTip1.SetToolTip(trackBar1, "Compression quality of the jpeg image");
            toolTip1.SetToolTip(checkBoxSaveWithNewName, "If unchecked overrides the original image");

        }
    }
}
