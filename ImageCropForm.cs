using System;
using System.Drawing;
using System.Windows.Forms;

public enum ResizeMode
{
    None,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
}

namespace WindowsFormsApp1
{
    public partial class ImageCropForm : Form
    {
        private Bitmap originalImage;
        private Rectangle cropRectangle;
        private Point cropStartPoint;
        private bool isCropping;
        private bool isDragging;
        private ResizeMode resizeMode = ResizeMode.None;
        private Point relativeMousePos;

        public ImageCropForm()
        {
            InitializeComponent();
        }

        private void SetResizeCursor(ResizeMode mode)
        {
            Cursor cursor = Cursors.Default;

            switch (mode)
            {
                case ResizeMode.TopLeft:
                    cursor = Cursors.SizeNWSE;
                    break;
            }

            pictureBox1.Cursor = cursor;
        }

        private void SetDefaultCursor()
        {
            pictureBox1.Cursor = Cursors.Default;
        }

        private bool nearLeft(Point mouseLocation)
        {
            //define hit area
            int hitAreaSize = 15;
            Rectangle hitArea = new Rectangle(cropRectangle.Left - hitAreaSize, cropRectangle.Top - hitAreaSize, hitAreaSize * 2, hitAreaSize * 2); 

            return hitArea.Contains(mouseLocation);
        }

        private void ImageCropForm_Load(object sender, EventArgs e)
        {
            //load files AFTER form loaded
            //studentCSV =
            originalImage = (Bitmap)Bitmap.FromFile(@"C:\Users\bbouaziz\Desktop\scripts\testIMG.jpg");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = (Image)originalImage;

            //initialise croptangle
            cropRectangle = new Rectangle(10, 10, 200, 266); //initial position and size + sets at 3:4 aspect ratio
            isCropping = false;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~DOWN~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        private Point previousMouseLocation;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            previousMouseLocation = e.Location;

            if (cropRectangle.Contains(e.Location))
            {
                if (e.X - cropRectangle.Left < 10 && e.Y - cropRectangle.Top < 10)
                {
                    resizeMode = ResizeMode.TopLeft;
                }
                // Add cases for other corners if needed
                else
                {
                    resizeMode = ResizeMode.None;
                }
            }
            else
            {
                resizeMode = ResizeMode.None;
            }
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~MOVE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int dx = e.X - previousMouseLocation.X;
                int dy = e.Y - previousMouseLocation.Y;

                if (resizeMode == ResizeMode.None)
                {
                    // Move the rectangle
                    cropRectangle = new Rectangle(cropRectangle.Left + dx, cropRectangle.Top + dy, cropRectangle.Width, cropRectangle.Height);
                }
                else
                {
                    // Resize the rectangle
                    switch (resizeMode)
                    {
                        case ResizeMode.TopLeft:
                            cropRectangle = new Rectangle(cropRectangle.Left + dx, cropRectangle.Top + dy, cropRectangle.Width - dx, cropRectangle.Height - dy);
                            break;
                            // Add cases for other corners if needed
                    }
                }

                pictureBox1.Invalidate();
            }

            previousMouseLocation = e.Location;
        }

    
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~UP~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //mouse up handle
            resizeMode = ResizeMode.None;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~PAINT~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
           //check if called
           Console.WriteLine("Paint event called");
           //draw overlay
           using (Pen pen = new Pen(Color.Red, 3))
            {
                int maxWidth = pictureBox1.ClientSize.Width;
                int maxHeight = pictureBox1.ClientSize.Height;
                int newWidth = Math.Min(Math.Max(0, cropRectangle.Width), maxWidth);
                int newHeight = Math.Min(Math.Max(0, cropRectangle.Height), maxHeight);

                Rectangle validCropRectangle = new Rectangle(cropRectangle.Location, new Size(newWidth, newHeight));
                
                e.Graphics.DrawRectangle(pen, cropRectangle);
            }
        }

        private void buttonCrop_Click(object sender, EventArgs e)
        {
            //crop selected area
            Bitmap croppedImage = new Bitmap(cropRectangle.Width, cropRectangle.Height);
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(originalImage, new Rectangle(0, 0, cropRectangle.Width, cropRectangle.Height), cropRectangle, GraphicsUnit.Pixel);
            }

            //display crop
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = croppedImage;
        }
    }
}
