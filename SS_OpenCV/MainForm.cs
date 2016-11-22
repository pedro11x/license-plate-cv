﻿using System;
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
    public partial class MainForm : Form
    {
        Image<Bgr, Byte> img = null; // working image
        //Image<Bgr, Byte> imgUndo = null; // undo backup image - UNDO
        Stack<Image<Bgr, Byte>> history = new Stack<Image<Bgr, byte>>();
        string title_bak = "";

        public MainForm()
        {
            InitializeComponent();
            title_bak = Text;
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
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.ConvertToGray(img);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        /// <summary>
        /// Calculate the image negative
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void negativeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            //ImageClass.Negative(img);
            ImageClass.DNegative(img);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void bWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.ConvertToGray(img);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void redToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.FilterComponent(img, ImageClass.Component.RED);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void greenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.FilterComponent(img, ImageClass.Component.GREEN);

            refresh();
            Cursor = Cursors.Default; // normal cursor 
        }

        private void blueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;
            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.FilterComponent(img, ImageClass.Component.BLUE);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void translationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            //show dialog
            
            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.Translate(img, -30, -30);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void typeAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            //show dialog
            var watch = System.Diagnostics.Stopwatch.StartNew();
            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.Avg(img,3);

            watch.Stop();
            Console.WriteLine("---> %d ms ", watch.ElapsedMilliseconds);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void robertsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            //show dialog

            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.EdgeDetectionRoberts(img);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            //show dialog

            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            ImageClass.EdgeDetectionSobel3x3(img);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void medianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            //show dialog

            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            var watch = System.Diagnostics.Stopwatch.StartNew();
            ImageClass.Median3x3(img);

            watch.Stop();
            Console.WriteLine("---> {0} ms ", watch.ElapsedMilliseconds);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            //show dialog

            Cursor = Cursors.WaitCursor; // clock cursor 


            var watch = System.Diagnostics.Stopwatch.StartNew();
            ImageClass.Histogram h = ImageClass.ImageHistogram(img);
            watch.Stop();
            Console.WriteLine("---> {0} ms ", watch.ElapsedMilliseconds);
            Histogram histogramWindow = new Histogram(h);
            histogramWindow.Show();

            Cursor = Cursors.Default; // normal cursor 
        }

        private void binarizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (img == null) // verify if the image is already opened
                return;

            //show dialog

            Cursor = Cursors.WaitCursor; // clock cursor 

            history.Push(img.Copy());

            var watch = System.Diagnostics.Stopwatch.StartNew();

            ImageClass.OtsuBinarization(img);

            watch.Stop();
            Console.WriteLine("---> {0} ms ", watch.ElapsedMilliseconds);

            refresh();

            Cursor = Cursors.Default; // normal cursor 
        }





        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Z))
            {
                undo();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }



}