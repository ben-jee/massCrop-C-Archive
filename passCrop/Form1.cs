using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace passCrop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // LOAD image
            Bitmap bmp = (Bitmap)Bitmap.FromFile(@"C:\Users\bbouaziz\Desktop\scripts\testIMG.jpg");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = (Image)bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // declare mouse event handlers
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            pictureBox1.MouseEnter += new EventHandler(pictureBox1_MouseEnter);
            Controls.Add(pictureBox1);
        }

        // declare vars for crop params
        int crpX, crpY, rectW, rectH;
        // declare crop pen
        public Pen crpPen = new Pen(Color.White);

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // mouse down handler
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Cursor = Cursors.Cross;
                crpPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                // initialise x, y coords
                crpX = e.X;
                crpY = e.Y;
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            // mouse enter handler
            Cursor = Cursors.Cross;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // crop selection drawing handler
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                pictureBox1.Refresh();
                // determine width and height for crop (650x700)
                rectW = e.X - crpX;
                rectH = e.Y - crpY;
                Graphics g = pictureBox1.CreateGraphics();
                g.DrawRectangle(crpPen, crpX, crpY, rectW, rectH);
                g.Dispose();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // reset cursor on mouse enter
            base.OnMouseEnter(e);
            Cursor = Cursors.Default;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // display dimensions of crop rectangle
            label2.Text = "Dimensions: " + rectW + ", " + rectH;
            Cursor = Cursors.Default;

            // Create a separate thread to perform cropping
            Thread cropThread = new Thread(CropImage);
            cropThread.Start();
        }

        private void CropImage()
        {
            // Draw cropped image in a new cropped box
            Bitmap bmp2 = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            pictureBox1.Invoke((MethodInvoker)delegate
            {
                pictureBox1.DrawToBitmap(bmp2, pictureBox1.ClientRectangle);
            });

            // Create cropped image based on selection
            Bitmap crpImg = new Bitmap(rectW, rectH);

            for (int i = 0; i < rectW; i++)
            {
                for (int y = 0; y < rectH; y++)
                {
                    Color pxlclr = bmp2.GetPixel(crpX + i, crpY + y);
                    crpImg.SetPixel(i, y, pxlclr);
                }
            }

            // Update the PictureBox on the UI thread
            pictureBox2.Invoke((MethodInvoker)delegate
            {
                pictureBox2.Image = (Image)crpImg;
                pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            });

            // Automatically save the cropped image
            SaveCroppedImage(crpImg);
        }

        private void SaveCroppedImage(Bitmap croppedImage)
        {
            try
            {
                // Specify the path where you want to save the cropped image
                string savePath = @"C:\Users\bbouaziz\Desktop\scripts\croppedIMG.jpg";

                // Save the image to the specified path
                croppedImage.Save(savePath);

                // Optionally, you can display a message or perform other actions after saving.
                MessageBox.Show("Cropped image saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving cropped image: " + ex.Message);
            }
        }
    }
}
