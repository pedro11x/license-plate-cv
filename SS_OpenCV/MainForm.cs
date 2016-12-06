using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;

namespace SS_OpenCV
{
    public partial class MainForm : Form, MainForm.ImageUpdateble
    {
        Image<Bgr, Byte> img = null; // working image
        //Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
        Stack<Image<Bgr, Byte>> history = new Stack<Image<Bgr, byte>>();
        string title_bak = "";

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
            toggleZoom();
        }

        /// <summary>
        /// Opens a new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if(img!=null) history.Push(img.Copy());
                img = new Image<Bgr, byte>(openFileDialog1.FileName);
                Text = title_bak + " [" +
                        openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf("\\") + 1) +
                        "]";
                refresh();
                
            }
        }

        /// <summary>
        /// Saves an image with a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageViewer.Image.Save(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void refresh() {
            ImageViewer.Image = img.Bitmap;
            ImageViewer.Refresh(); // refresh image on the screen
        }

        private void undo() {
            try
            {
                Image<Bgr, Byte> oimg = history.Pop();
                img = oimg;
                ImageViewer.Image = oimg.Bitmap;
                ImageViewer.Refresh(); // refresh image on the screen
            }
            catch { }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Z))
            {
                undo();
                return true;
            }
            if (keyData == (Keys.Control | Keys.D))
            {
                toggleZoom();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// restore last undo copy of the working image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undo();
        }

        /// <summary>
        /// Chaneg visualization mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toggleZoom();  
        }

        void toggleZoom() {
            // zoom
            if (autoZoomToolStripMenuItem.Checked)
            {
                ImageViewer.SizeMode = PictureBoxSizeMode.Zoom;
                ImageViewer.Dock = DockStyle.Fill;
            }
            else // with scroll bars
            {
                ImageViewer.Dock = DockStyle.None;
                ImageViewer.SizeMode = PictureBoxSizeMode.AutoSize;
            }
        }

        /// <summary>
        /// Show authors form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AuthorsForm form = new AuthorsForm();
            form.ShowDialog();
        }


        /// <summary>
        /// Convert the working image to grayscale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.ConvertToGray(img);
                return img;
            });
        }

        private void negativeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            doAction((img) => {
                ImageClass.DNegative(img);
                return img;
            });
        }

        private void redToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            doAction((img) => {ImageClass.FilterComponent(img, ImageClass.Component.RED);
                ImageClass.Median3x3(img);
                return img;
            });
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.FilterComponent(img, ImageClass.Component.GREEN);
                return img;
            });
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.FilterComponent(img, ImageClass.Component.BLUE);
                return img;
            });
        }

        private void translationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.Translate(img, -30, -30);
                return img;
            });
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void typeAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.Avg(img,3);
                return img;
            }); 
        }

        private void robertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.EdgeDetectionRoberts(img);
                return img;
            });
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                //LPRecognition.EdgeDetectionSobel3x3(img);
                ImageClass.EdgeDetectionSobel3x3(img);
                return img;
            });
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.Median3x3(img);
                return img;
            });
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.Histogram h = ImageClass.ImageHistogram(img);
                Histogram histogramWindow = new Histogram(h);
            });
        }

        private void binarizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.OtsuBinarization(img);
                return img;
            });
        }

        private void sobelXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            var watch = System.Diagnostics.Stopwatch.StartNew();
            ImageClass.EdgeDetectionSobel3x3X(img);

            watch.Stop();
            Console.WriteLine("---> {0} ms ", watch.ElapsedMilliseconds);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void sobelYToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            var watch = System.Diagnostics.Stopwatch.StartNew();
            ImageClass.EdgeDetectionSobel3x3Y(img);

            

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void showProjectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                ImageClass.Projection v = ImageClass.HProjection(img);
                ImageClass.Projection h = ImageClass.VProjection(img);

                GraphXY g = new GraphXY(v.values, h.values);
            });
            
        }

        private void goToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img)=> {
                //LPRecognition.detectLPCharacterRegionsX(img);
                //LPRecognition.detectLPCharacterRegionsY(img);
                LPRecognition.detectCharacterRegions(img, this);
                return img;
            });
        }

        private void doAction(Action<Image<Bgr,Byte>> a) {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 
            var watch = System.Diagnostics.Stopwatch.StartNew();
            a(img);
            watch.Stop();
            Console.WriteLine("---> {0} ms ", watch.ElapsedMilliseconds);
            Cursor = Cursors.Default; // normal cursor 
        }
        private void doAction(Func<Image<Bgr, Byte>,Image<Bgr, Byte>> a)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 
            history.Push(img.Copy());
            var watch = System.Diagnostics.Stopwatch.StartNew();
            img = a(img);
            refresh();
            watch.Stop();
            Console.WriteLine("---> {0} ms ", watch.ElapsedMilliseconds);
            Cursor = Cursors.Default; // normal cursor 
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void lPHRegionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                LPRecognition.detectLPCharacterRegionsX(img);
                return img;
            });
        }

        private void lPVRegionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            doAction((img) => {
                LPRecognition.detectLPCharacterRegionsY(img);
                return img;
            });
        }

        public void updateImage(Image<Bgr, byte> newimg)
        {
            doAction((img)=> {
                return newimg;
            });
        }

        public interface ImageUpdateble {
            void updateImage(Image<Bgr, Byte> img);
        }

        private void evalFormToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SS_OpenCV.EvalForm().ShowDialog();
        }
    }



}