using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SS_OpenCV.LPRecognition;

namespace SS_OpenCV
{
    class Utils
    {
        public static void applyMovingAverage(ImageClass.Projection p, double f)
        {
            int d = (int)(p.values.Length * f / 2);
            int[] v = new int[p.values.Length];
            //Console.WriteLine("--d: {0}, size: {1}", d, p.values.Length);
            p.peak = 0;
            for (int i = d; i < (p.values.Length - 1 - d); i++)
            {

                for (int dl = i - d; dl < i; dl++)
                {
                    //Console.WriteLine("--dl {0}, i {1}", dl, i);
                    v[i] += p.values[dl];

                }
                for (int dr = i + 1; dr <= i + d; dr++)
                    v[i] += p.values[dr];

                v[i] += p.values[i];
                v[i] = (int)(v[i] / (d * 2 + 1.0));
                if (v[i] > p.peak) p.peak = v[i];
            }
            p.values = v;
        }

        public static void markVRegion(List<Region> ls, Image<Bgr, Byte> img)
        {

            foreach (Region r in ls)
            {
                unsafe
                {
                    MIplImage m = img.MIplImage;
                    byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                    int width = img.Width;
                    int height = img.Height;
                    int nChan = m.nChannels; // number of channels - 3
                    int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                    int lineOffset = nChan * width + padding;
                    for (int i = r.startPoint; i < r.endPoint; i++)
                        for (int h = 0; h < height; h++)
                        {
                            //dataPtr[lineOffset * h + i * nChan] = 0;
                            //dataPtr[lineOffset * h + i * nChan + 1] = 0;
                            dataPtr[lineOffset * h + i * nChan + 2] = 255;
                        }

                }
            }
        }

        public static void markHRegion(List<Region> ls, Image<Bgr, Byte> img)
        {

            foreach (Region r in ls)
            {
                unsafe
                {
                    MIplImage m = img.MIplImage;
                    byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                    int width = img.Width;
                    int height = img.Height;
                    int nChan = m.nChannels; // number of channels - 3
                    int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                    int lineOffset = nChan * width + padding;
                    for (int i = r.startPoint; i < r.endPoint; i++)
                        for (int w = 0; w < width; w++)
                        {
                            //dataPtr[lineOffset * i + w * nChan] = 0;
                            dataPtr[lineOffset * i + w * nChan + 1] = 255;
                            //dataPtr[lineOffset * i + w * nChan + 2] = 255;
                        }

                }
            }
        }

        public static void markRectangle(System.Drawing.Rectangle r, Image<Bgr, Byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int lineOffset = nChan * width + padding;
                for (int i = r.Top; i < r.Bottom; i++)
                    for (int w = r.Left; w < r.Right; w++)
                    {
                        //dataPtr[lineOffset * i + w * nChan] = 0;
                        dataPtr[lineOffset * i + w * nChan + 1] = (byte)(255 - dataPtr[lineOffset * i + w * nChan + 1]);
                        //dataPtr[lineOffset * i + w * nChan + 2] = 255;
                    }

            }
            
        }

        public static void markVLines(List<int> ls, Image<Bgr, Byte> img)
        {

            foreach (int l in ls)
            {
                unsafe
                {
                    MIplImage m = img.MIplImage;
                    byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                    int width = img.Width;
                    int height = img.Height;
                    int nChan = m.nChannels; // number of channels - 3
                    int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                    int lineOffset = nChan * width + padding;
                    for (int h = 0; h < height; h++)
                    {
                        dataPtr[lineOffset * h + nChan * l] = 255;
                        dataPtr[lineOffset * h + nChan * l + 1] = 0;
                        //dataPtr[lineOffset * h + nChan * l + 2] = 255;
                    }

                }
            }
        }

        public static Image<Bgr, Byte> cutXRegion(Image<Bgr, Byte> img, Region region)
        {
            Image<Bgr, Byte> portion = new Image<Bgr, byte>(region.delta, img.Height);

            unsafe
            {
                MIplImage sm = img.MIplImage;
                byte* srcPtr = (byte*)sm.imageData.ToPointer(); // Pointer to the image

                MIplImage dm = portion.MIplImage;
                byte* dstPtr = (byte*)dm.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;

                int nChan = sm.nChannels; // number of channels - 3

                int srcPadding = sm.widthStep - sm.nChannels * sm.width; // alinhament bytes (padding)
                int srcLineOffset = nChan * width + srcPadding;

                int dstPadding = dm.widthStep - dm.nChannels * dm.width; // alinhament bytes (padding)
                int dstLineOffset = dm.nChannels * dm.width + dstPadding;

                for (int x = 0; x < region.delta; x++)
                {
                    for (int y = 0; y < img.Height; y++)
                    {
                        dstPtr[dstLineOffset * y + x * nChan] = srcPtr[srcLineOffset * y + (x + region.startPoint) * nChan];
                        dstPtr[dstLineOffset * y + x * nChan + 1] = srcPtr[srcLineOffset * y + (x + region.startPoint) * nChan + 1];
                        dstPtr[dstLineOffset * y + x * nChan + 2] = srcPtr[srcLineOffset * y + (x + region.startPoint) * nChan + 2];
                    }
                }
            }


            return portion;
        }


        public static Image<Bgr, Byte> cutYRegion(Image<Bgr, Byte> img, Region region)
        {
            Image<Bgr, Byte> portion = new Image<Bgr, byte>(img.Width, region.delta);

            unsafe
            {
                MIplImage sm = img.MIplImage;
                byte* srcPtr = (byte*)sm.imageData.ToPointer(); // Pointer to the image

                MIplImage dm = portion.MIplImage;
                byte* dstPtr = (byte*)dm.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;

                int nChan = sm.nChannels; // number of channels - 3

                int srcPadding = sm.widthStep - sm.nChannels * sm.width; // alinhament bytes (padding)
                int srcLineOffset = nChan * width + srcPadding;

                int dstPadding = dm.widthStep - dm.nChannels * dm.width; // alinhament bytes (padding)
                int dstLineOffset = dm.nChannels * dm.width + dstPadding;

                for (int x = 0; x < img.Width; x++)
                {
                    for (int y = 0; y < region.delta; y++)
                    {
                        dstPtr[dstLineOffset * y + x * nChan + 0] = srcPtr[srcLineOffset * (y + region.startPoint) + x * nChan];
                        dstPtr[dstLineOffset * y + x * nChan + 1] = srcPtr[srcLineOffset * (y + region.startPoint) + x * nChan + 1];
                        dstPtr[dstLineOffset * y + x * nChan + 2] = srcPtr[srcLineOffset * (y + region.startPoint) + x * nChan + 2];
                    }
                }
            }


            return portion;
        }

        static List<Region> joinAdjointRegions(List<Region> rs)
        {
            List<Region> nrs = new List<Region>();
            Region pr = null;
            foreach (Region r in rs)
            {
                if (pr == null)
                {
                    pr = r;
                }
                else if (r.startPoint - pr.endPoint <= 1)
                {
                    pr = new Region(pr.startPoint, r.endPoint);
                }
                else
                {
                    nrs.Add(pr);
                    pr = r;
                }
            }
            if (pr != null) nrs.Add(pr);
            return nrs;
        }
    }
}
