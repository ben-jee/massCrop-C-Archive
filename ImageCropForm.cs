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

        private bool nearLeft(Point mouseLocation)
        {
            //define hit area
            int hitAreaSize = 10;
            Rectangle hitArea = new Rectangle(cropRectangle.Left - hitAreaSize, cropRectangle.Top - hitAreaSize, hitAreaSize * 2, hitAreaSize * 2); 

            return hitArea.Contains(mouseLocation);
        }

        private void ImageCropForm_Load(object sender, EventArgs e)
        {
            //loads image once form loads
            originalImage = (Bitmap)Bitmap.FromFile(@"C:\Users\benbw\OneDrive\Desktop\ROOT\photo.jpg");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = (Image)originalImage;

            //initialise croptangle
            cropRectangle = new Rectangle(10, 10, 200, 266); //initial position and size + sets at 3:4 aspect ratio
            isCropping = false;
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~DOWN~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //mouse down handler for dragging
            if (cropRectangle.Contains(e.Location))
            {
                isDragging = true;
                relativeMousePos = new Point(e.X - cropRectangle.X, e.Y - cropRectangle.Y);
            }

            //resizing handler
            if (nearLeft(e.Location))
            {
                resizeMode = ResizeMode.TopLeft;
                relativeMousePos = new Point(cropRectangle.Right - e.X, cropRectangle.Bottom - e.Y);
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~MOVE~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                //calc new drag pos
                //int newX = e.X - realtiveMousePos.X;
                //int newY = e.Y - relativeMousePos.Y;

                //refresh position when dragging
                cropRectangle.Location = new Point(e.X - relativeMousePos.X, e.Y - relativeMousePos.Y);
                pictureBox1.Invalidate(); //redraw
            }

            // Handle mouse move event for resizing the cropper (3:4 aspect ratio)
            if (isCropping)
            {
                int dx = e.X - cropStartPoint.X;
                int dy = e.Y - cropStartPoint.Y;

                // Force 3:4 aspect ratio
                int newWidth = cropRectangle.Width + dx;
                int newHeight = (int)((4.0 / 3.0) * newWidth);

                // Update the croptangle
                cropRectangle = new Rectangle(cropRectangle.Left, cropRectangle.Top, newWidth, newHeight);

                // Redraw the picture box
                pictureBox1.Invalidate();
                cropStartPoint = e.Location;
            }

            if (resizeMode != ResizeMode.None)
            {
                //calc new size
                int newWidth = cropRectangle.Width;
                int newHeight = cropRectangle.Height;

                switch (resizeMode)
                {
                    case ResizeMode.TopLeft:
                        //calc new width and height for top left resize#
                        newWidth = cropRectangle.Right - e.X + cropRectangle.Width;
                        newHeight = (int)((4.0 / 3.0) * newWidth);
                        break;
                }

                cropRectangle.Size = new Size(newWidth, newHeight);
                pictureBox1.Invalidate();
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~UP~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\\
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //mouse up handle
            isCropping = false;
            isDragging = false;
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
