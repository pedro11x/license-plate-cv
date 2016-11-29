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
            Console.WriteLine("--d: {0}, size: {1}", d, p.values.Length);
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
    }
}
