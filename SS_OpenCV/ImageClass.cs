using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;
using System.Drawing;

namespace SS_OpenCV
{
    public class ImageClass
    {

        /// <summary>
        /// Image Negative
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            /*B*/
                            dataPtr[0] = (byte)(255 - (int)dataPtr[0]);
                            /*G*/
                            dataPtr[1] = (byte)(255 - (int)dataPtr[1]);
                            /*R*/
                            dataPtr[2] = (byte)(255 - (int)dataPtr[2]);

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }


        /// <summary>
        /// Convert to gray
        /// Direct access to memory
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // convert to gray
                            gray = (byte)(((int)dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }


        public enum Component { RED, GREEN, BLUE }

        public static void RedChannel(Image<Bgr, Byte> img) {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            // store in the image
                            dataPtr[0] = dataPtr[2];
                            dataPtr[1] = dataPtr[2];

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }
                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        /// <summary>
        /// Filter color and convert to gray
        /// Direct access to memory
        /// </summary>
        /// <param name="img">image</param>
        public static void FilterComponent(Image<Bgr, byte> img, Component color)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //obtém as 3 componentes
                            if (color == Component.BLUE)
                                gray = dataPtr[0];
                            else if (color == Component.GREEN)
                                gray = dataPtr[1];
                            else
                                gray = dataPtr[2];

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }
                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }


        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dX, int dY)
        {
            Image<Bgr, byte> source = imgCopy;
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte* srcPtr = (byte*)source.MIplImage.imageData.ToPointer(); // Pointer to the original image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        int srcX = x - dX,
                            srcY = y - dY;

                        if (srcY >= 0 && srcY < height &&
                            srcX >= 0 && srcX < width)
                        {
                            byte* p = srcPtr + (nChan * width + padding) * srcY + nChan * srcX;
                            for (int i = 0; i < nChan; i++) dataPtr[i] = p[i];
                            byte val = p[0];
                            dataPtr[0] = val;
                        }
                        else
                        {   //Out of bounds
                            //-> fill with black
                            for (int i = 0; i < nChan; i++) dataPtr[i] = 0;
                        }
                        // advance the pointer to the next pixel
                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

        }

        /// <summary>
        /// Translate image
        /// Direct access to memory
        /// </summary>
        /// <param name="img">image</param>
        public static void Translate(Image<Bgr, byte> img, int dX, int dY)
        {
            Translation(img, img.Copy(), dX, dY);
        }

        /// <summary>
        /// Rotate image with center origin
        /// </summary>
        /// <param name="img">image</param>
        public static void Rotate(Image<Bgr, byte> img, double angle)
        {
            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte* srcPtr = (byte*)source.MIplImage.imageData.ToPointer(); // Pointer to the original image

                double centerX = img.Width / 2,
                       centerY = img.Height / 2;

                double cos = Math.Cos(angle),
                       sin = Math.Sin(angle);

                double srcX0 = -centerX * cos - centerY * sin + centerX,
                       srcY0 = centerY + centerX * sin - centerY * cos;

                double hVecX = -centerX * cos - (centerY - 1) * sin + centerX - srcX0,
                       hVecY = centerY + centerX * sin - (centerY - 1) * cos - srcY0,
                       wVecX = (1 - centerX) * cos - centerY * sin + centerX - srcX0,
                       wVecY = centerY - (1 - centerX) * sin - centerY * cos - srcY0;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;

                double srcX = srcX0, srcY = srcY0;

                for (y = 0; y < height; y++)
                {
                    double lineSrcX = srcX, lineSrcY = srcY;
                    for (x = 0; x < width; x++)
                    {
                        byte r, g, b;
                        //byte* srcPoint = (byte*)(srcPtr + (lineOffset) * srcY + nChan * srcX);


                        // dataPtr[0] = b; dataPtr[1] = g; dataPtr[2] = r;

                        lineSrcX += wVecX;
                        lineSrcY *= wVecY;
                        // advance the pointer to the next pixel
                        dataPtr += nChan;
                    }
                    //Apply vector
                    srcX += hVecX;
                    srcY += hVecY;
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

        }

        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy) { Avg(img, 3); }

        /// <summary>
        /// Apply average filter
        /// </summary>
        /// <param name="filterSize">size of filter (3 for 3x3, 5 for 5x5), must be odd number</param>
        /// <param name="img">image</param>
        public static void Avg(Image<Bgr, byte> img, int filterSize)
        {
            int avgHeight = filterSize,
                avgWidth = filterSize;

            int dHeight = avgHeight / 2,
                dWidth = avgWidth / 2;

            uint area = (uint)(avgHeight * avgWidth);

            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte* srcPtr = (byte*)source.MIplImage.imageData.ToPointer(); // Pointer to the original image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;

                int sumR, sumG, sumB;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {

                        sumR = 0; sumG = 0; sumB = 0;
                        for (int i = -dHeight; i <= dHeight; i++)
                            for (int j = -dWidth; j <= dWidth; j++)
                            {
                                int srcX = x + i,
                                    srcY = y + j;

                                /*MIRROR EDGES 
                                if (srcX < 0) srcX = -srcX + 1;
                                if (srcX >= width) srcX -= srcX - width;
                                if (srcY < 0) srcY = -srcY + 1;
                                if (srcY >= height) srcY -= srcY - height;
                                /*COPY EDGES*/
                                if (srcX < 0) srcX = 0;
                                if (srcX >= width) srcX = width - 1;
                                if (srcY < 0) srcY = 0;
                                if (srcY >= height) srcY = height - 1;
                                /**/
                                byte* p = srcPtr + (lineOffset) * srcY + nChan * srcX;

                                sumB += p[0]; sumG += p[1]; sumR += p[2];

                            }
                        dataPtr[0] = (byte)(sumB / area);
                        dataPtr[1] = (byte)(sumG / area);
                        dataPtr[2] = (byte)(sumR / area);

                        // advance the pointer to the next pixel
                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

        }



        /// <summary>
        /// Roberts Edge detection filter
        /// </summary>
        /// <param name="img">image</param>
        public static void EdgeDetectionRoberts(Image<Bgr, byte> img)
        {
            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte* srcPtr = (byte*)source.MIplImage.imageData.ToPointer(); // Pointer to the original image


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        for (int component = 0; component < nChan; component++)
                        {
                            int s;
                            if (y < (height - 1) && x < (width - 1))
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * y + nChan * x + component] -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component])
                                  + Math.Abs(srcPtr[(lineOffset) * y + nChan * (x + 1) + component] -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * x + component]);
                            }
                            else if (y == (height - 1))
                            {
                                if (x == (width - 1)) { s = 0; }
                                else
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * y + nChan * x + component] -
                                                 srcPtr[(lineOffset) * y + nChan * (x + 1) + component]) * 2;
                                }
                            }
                            else
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * y + nChan * x + component] -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * x + component]) * 2;
                            }

                            dataPtr[0] = (byte)s;

                            dataPtr++;
                        }


                        // advance the pointer to the next pixel
                        //dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

        }






        /// <summary>
        /// Sobel Edge detection filter 3 x 3
        /// </summary>
        /// <param name="img">image</param>
        public static void EdgeDetectionSobel3x3(Image<Bgr, byte> img)
        {
            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte* srcPtr = (byte*)source.MIplImage.imageData.ToPointer(); // Pointer to the original image


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        for (int component = 0; component < nChan; component++)
                        {
                            int s = 0;
                            ///Normal case (not on border)
                            if (y < (height - 1) && x < (width - 1) && x > 0 && y > 0)
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] +
                                             srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 2 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component] -
                                             srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 2 -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component]
                                             )
                                  + Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 2 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -
                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 2 -
                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                            }
                            else if (y == 0)
                            {///Top border
                                if (x == 0)
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component]
                                             )
                                      + Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component]
                                             );
                                }
                                else if (x == (width - 1))
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component]
                                             )
                                      + Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 3 -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] -
                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3
                                             );
                                }
                                else
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component]
                                             )
                                      + Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 2 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] -
                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 2 -
                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component]
                                             );
                                }
                            }
                            else if (y == (height - 1))
                            {
                                if (x == 0)
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             )
                                      + Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] -

                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                                }
                                else if (x == (width - 1))
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component]
                                             )
                                      + Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] +
                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 -

                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 3
                                             );
                                }
                                else
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             )
                                      + Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] +
                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 2 +
                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] -

                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 2 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                                }
                            }
                            else if (x == 0)
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] +
                                             srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 2 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component] -
                                             srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 2 -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component]
                                             )
                                  + Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 3 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 3 -
                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                            }
                            else
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] +
                                             srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 2 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] -
                                             srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 2 -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component]
                                             )
                                  + Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 3 -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -
                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 3
                                             );
                            }

                            if (s > 255) s = 255; else if (s < 0) s = 0;
                            *(dataPtr++) = (byte)s;
                        }
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

        }


        /// <summary>
        /// 3D Median filter 3x3
        /// </summary>
        /// <param name="img">image</param>
        public static void Median3x3(Image<Bgr, byte> img)
        {
            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte* srcPtr = (byte*)source.MIplImage.imageData.ToPointer(); // Pointer to the original image


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;
                byte[,,] mat = new byte[3, 3, 3];
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        for (int i = 0; i < 3; i++)
                            for (int j = 0; j < 3; j++)
                            {
                                int srcX = x + i - 1,
                                    srcY = y + j - 1;
                                if (srcX >= width) srcX = width - 1;
                                else if (srcX < 0) srcX = 0;
                                if (srcY >= height) srcY = height - 1;
                                else if (srcY < 0) srcY = 0;

                                byte* p = srcPtr + (lineOffset) * srcY + nChan * srcX;
                                mat[i, j, 0] = p[0];
                                mat[i, j, 1] = p[1];
                                mat[i, j, 2] = p[2];
                            }
                        int bestDistance = int.MaxValue;
                        int bestX = 0, bestY = 0;
                        for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++)
                            {
                                int sum = 0;
                                for (int ni = 0; ni < 3; ni++) for (int nj = 0; nj < 3; nj++)
                                        if (i != ni && j != nj)
                                        {
                                            sum += Math.Abs(mat[i, j, 0] - mat[ni, nj, 0]) +
                                                Math.Abs(mat[i, j, 1] - mat[ni, nj, 1]) +
                                                Math.Abs(mat[i, j, 2] - mat[ni, nj, 2]);
                                        }
                                if (bestDistance > sum)
                                {
                                    bestDistance = sum;
                                    bestX = i; bestY = j;
                                }
                            }

                        dataPtr[0] = mat[bestX, bestY, 0];
                        dataPtr[1] = mat[bestX, bestY, 1];
                        dataPtr[2] = mat[bestX, bestY, 2];

                        // advance the pointer to the next pixel
                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }
        }

        public class Histogram
        {
            public int[] Red;
            public int[] Green;
            public int[] Blue;
            public int[] BW;
            public int levels;
            public Histogram(int kn)
            {
                levels = kn;
                Red = new int[kn];
                Green = new int[kn];
                Blue = new int[kn];
                BW = new int[kn];
            }
        }

        public static Histogram ImageHistogram(Image<Bgr, byte> img)
        {
            Histogram h = new Histogram(256);
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        h.Blue[dataPtr[0]]++;
                        h.Green[dataPtr[1]]++;
                        h.Red[dataPtr[2]]++;
                        int w = ((dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3);
                        h.BW[(byte)(w)]++;
                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

            return h;
        }


        public static void Binarization(Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;
                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        
                        int w = ((dataPtr[0] + dataPtr[1] + dataPtr[2]) / 3);
                        if (w > threshold)
                        { dataPtr[0] = 0xFF; dataPtr[1] = 0xFF; dataPtr[2] = 0xFF; }
                        else
                        { dataPtr[0] = 0x00; dataPtr[1] = 0x00; dataPtr[2] = 0x00; }

                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }
        }

        public static void OtsuBinarization(Image<Bgr, byte> img)
        {
            int bestT = 0;
            double o2max = 0;

            //TODO: betterize
            Histogram h = ImageHistogram(img);
            double nPixels = img.Height * img.Width;
            for (int t = 0; t <= 255; t++) {
                double q1 = 0.0; for (int i = 0; i < t; i++) q1 += h.BW[i] / nPixels;
                double q2 = 1 - q1;

                double u1 = 0.0; for (int i = 0; i < t; i++) u1 += i * h.BW[i] / nPixels; u1 /= q1;
                double u2 = 0.0; for (int i = t+1; i <= 255; i++) u2 += i * h.BW[i] / nPixels; u2 /= q2;

                /* Complete OTSU (is shit) * /
                double o21 = 0.0; for (int i = 0; i < t; i++) o21 += Math.Pow(u1 - i, 2) * h.BW[i]/nPixels; o21 /= t;
                double o22 = 0.0; for (int i = t + 1; i <= 255; i++) o22 += Math.Pow(u2 - i, 2) * h.BW[i] / nPixels; o22 /= 255-t;
                double o2w = q1 * o21 + q2 * o22;
                /*/
                double o2w = q1 * q2 * Math.Pow(u1 - u2, 2);
                /**/
                //Console.WriteLine("------->{0} -- {1}; {2}; {3}; {4} --- {5}", t, q1, q2, u1, u2, o2w);
                if (o2w > o2max) { bestT = t;  o2max = o2w; }
            }
            //Console.WriteLine("------->{0}", bestT);
            Binarization(img, bestT);
        }














        /// <summary>
        /// Sobel Edge detection filter 3 x 3
        /// </summary>
        /// <param name="img">image</param>
        public static void EdgeDetectionSobel3x3X(Image<Bgr, byte> img)
        {
            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte* srcPtr = (byte*)source.MIplImage.imageData.ToPointer(); // Pointer to the original image


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        for (int component = 0; component < nChan; component++)
                        {
                            int s = 0;
                            ///Normal case (not on border)
                            if (y < (height - 1) && x < (width - 1) && x > 0 && y > 0)
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] +
                                             srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 2 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component] -
                                             srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 2 -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component]
                                             );
                                  
                            }
                            else if (y == 0)
                            {///Top border
                                if (x == 0)
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component]
                                             );
                                      
                                }
                                else if (x == (width - 1))
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component]
                                             );
                                }
                                else
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component]
                                             );
                                }
                            }
                            else if (y == (height - 1))
                            {
                                if (x == 0)
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                                }
                                else if (x == (width - 1))
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component]
                                             );
                                }
                                else
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                                }
                            }
                            else if (x == 0)
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] +
                                             srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 2 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component] -
                                             srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] * 2 -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component]
                                             );
                            }
                            else
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] +
                                             srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] * 2 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] -
                                             srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 2 -
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component]
                                             );
                            }

                            if (s > 255) s = 255; else if (s < 0) s = 0;
                            *(dataPtr++) = (byte)s;
                        }
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

        }

        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, Byte> _) {
            EdgeDetectionSobel3x3(img);
        }

        /// <summary>
        /// Sobel Edge detection filter 3 x 3
        /// </summary>
        /// <param name="img">image</param>
        public static void EdgeDetectionSobel3x3Y(Image<Bgr, byte> img)
        {
            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                byte* srcPtr = (byte*)source.MIplImage.imageData.ToPointer(); // Pointer to the original image


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        for (int component = 0; component < nChan; component++)
                        {
                            int s = 0;
                            ///Normal case (not on border)
                            if (y < (height - 1) && x < (width - 1) && x > 0 && y > 0)
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 2 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -
                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 2 -
                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                            }
                            else if (y == 0)
                            {///Top border
                                if (x == 0)
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component]
                                             );
                                }
                                else if (x == (width - 1))
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 3 -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] -
                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3
                                             );
                                }
                                else
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 2 +
                                                 srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component] -

                                                 srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] -
                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 2 -
                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component]
                                             );
                                }
                            }
                            else if (y == (height - 1))
                            {
                                if (x == 0)
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 +
                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] -

                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 3 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                                }
                                else if (x == (width - 1))
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] +
                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 3 -

                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 3
                                             );
                                }
                                else
                                {
                                    s = Math.Abs(srcPtr[(lineOffset) * (y) + nChan * (x - 1) + component] +
                                                 srcPtr[(lineOffset) * (y) + nChan * (x) + component] * 2 +
                                                 srcPtr[(lineOffset) * (y) + nChan * (x + 1) + component] -

                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 2 -
                                                 srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                                }
                            }
                            else if (x == 0)
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 3 +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x + 1) + component] -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 3 -
                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x + 1) + component]
                                             );
                            }
                            else
                            {
                                s = Math.Abs(srcPtr[(lineOffset) * (y + 1) + nChan * (x - 1) + component] +
                                             srcPtr[(lineOffset) * (y + 1) + nChan * (x) + component] * 3 -

                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x - 1) + component] -
                                             srcPtr[(lineOffset) * (y - 1) + nChan * (x) + component] * 3
                                             );
                            }

                            if (s > 255) s = 255; else if (s < 0) s = 0;
                            *(dataPtr++) = (byte)s;
                        }
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

        }

        public class Projection { public int[] values; public int peak; public Projection(int[] v, int p) { values = v; peak = p; } }

        /// <summary>
        /// HorizontalProjection
        /// Image must be binary
        /// </summary>
        /// <param name="img">image</param>
        public static Projection VProjection(Image<Bgr, byte> img)
        {
            int[] v;
            int peak = 0;

            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;

                v = new int[height];

                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        if (dataPtr[0] > 128) {
                            if (++v[y] > peak) peak = v[y];
                        }

                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }
            return new Projection(v, peak);
        }




        /// <summary>
        /// HorizontalProjection
        /// Image must be binary
        /// </summary>
        /// <param name="img">image</param>
        public static Projection HProjection(Image<Bgr, byte> img)
        {
            int[] v;
            int peak = 0;

            Image<Bgr, byte> source = img.Clone();
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image

                int width = img.Width;
                int height = img.Height;

                v = new int[width];

                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                int x, y;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        if (dataPtr[0] > 128)
                        {
                            if(++v[x]>peak) peak=v[x];
                        }

                        dataPtr += nChan;
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }
            return new Projection(v, peak);
        }


        public static void LP_Recognition(
            Image<Bgr, byte> img, // imagem a alterar
            Image<Bgr, byte> imgCopy, // cópia da imagem
            out Rectangle LP_Location, // rectangulo(x,y,largura, altura) contendo a matricula
            out Rectangle LP_Chr1, // rectangulo contendo o primeiro carater
            out Rectangle LP_Chr2, // rectangulo contendo o segundo carater
            out Rectangle LP_Chr3, // rectangulo contendo o terceiro carater
            out Rectangle LP_Chr4, // rectangulo contendo o quarto carater
            out Rectangle LP_Chr5, // rectangulo contendo o quinto carater
            out Rectangle LP_Chr6, // rectangulo contendo o sexto carater
            out string LP_C1, // valor do primeiro carater
            out string LP_C2, // valor do segundo carater
            out string LP_C3, // valor do terceiro carater
            out string LP_C4, // valor do quarto carater
            out string LP_C5, // valor do quinto carater
            out string LP_C6, // valor do sexto carater
            out string LP_Country, // valor do País,
            out string LP_Month, // valor do mês da matricula,
            out string LP_Year) // valor do ano da matrícula
        {
            LP_Chr1 = new Rectangle(); LP_Chr2 = new Rectangle(); LP_Chr3 = new Rectangle();
            LP_Chr4 = new Rectangle(); LP_Chr5 = new Rectangle(); LP_Chr6 = new Rectangle();
            LP_Country = "?"; LP_Month = "??"; LP_Year = "??";
            LP_C1 = "?"; LP_C2 = "?"; LP_C3 = "?"; LP_C4 = "?"; LP_C5 = "?"; LP_C6 = "?";

            string lpread = LPLocation.locateAndRead(img, out LP_Location);

            if (lpread != null) {
                LP_C1 = lpread[0] + "";
                LP_C2 = lpread[1] + "";
                LP_C3 = lpread[2] + "";
                LP_C4 = lpread[3] + "";
                LP_C5 = lpread[4] + "";
                LP_C6 = lpread[5] + "";
                Console.WriteLine(lpread);
            }
        }
    }
}
