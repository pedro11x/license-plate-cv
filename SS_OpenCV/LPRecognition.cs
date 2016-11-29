using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Emgu.CV.Structure;
using Emgu.CV;

namespace SS_OpenCV
{
    class LPRecognition
    {
        public static void LPLocalization(Image<Bgr, Byte> img) {
            

        }

        public class Region {
            public int startPoint, endPoint;
            public Region(int s, int e) { startPoint = s; endPoint = e; }
            public int delta() { return endPoint - startPoint; }
            override public String ToString() { return "["+startPoint+" > "+endPoint+"]"; }
        }

        

        public static List<Region> detectLPCharacterRegionsX(Image<Bgr, Byte> img)
        {
            ImageClass.EdgeDetectionSobel3x3Y(img);
            ImageClass.OtsuBinarization(img);
            ImageClass.Projection p = ImageClass.HProjection(img);

            List<Region> rl = null;

            //new GraphXY(p.values, new int[1]);
            Utils.applyMovingAverage(p, 0.005);
            new GraphXY(p.values, new int[1]);

            for (int t = 0; t < p.peak; t++) {
                List<Region> l = regionsListFromThreshold(p, t);
                if (validRegionsListX(p, l)) rl = l;
            }

            if (rl != null) {
                List<int> ll = new List<int>();
                foreach (Region r in rl)
                { ll.Add(r.startPoint); ll.Add(r.endPoint); }
                Utils.markVRegion(rl, img);
                Utils.markVLines(ll, img);
            }
                
            return rl;
        }

        public static Region detectLPCharacterRegionsY(Image<Bgr, Byte> img)
        {
            ImageClass.EdgeDetectionSobel3x3X(img);
            ImageClass.OtsuBinarization(img);

            ImageClass.Projection p = ImageClass.VProjection(img);
            List<Region> rl = null;

            //new GraphXY(new int[1], p.values);
            Utils.applyMovingAverage(p, 0.01);
            new GraphXY(new int[1], p.values);
            int t;
            for (t = p.peak; t > 0 ; t--)
            {
                List<Region> l = regionsListFromThreshold(p, t);
                if (validRegionsListY(p, l))
                {
                    rl = l;
                    break;
                }
            }
            
            if (rl != null)
            {
                List<int> ll = new List<int>();
                foreach (Region r in rl)
                { ll.Add(r.startPoint); ll.Add(r.endPoint); }
                Utils.markHRegion(rl, img);
                //markVLines(ll, img);
            }

            foreach(Region r in regionsListFromThreshold(p, 60)) { Console.WriteLine(r); }

            Console.WriteLine("peak:{0}, found threshold:{1}",p.peak,t);

            Region bestRegion = null;
            foreach (Region r in rl) if (bestRegion == null || bestRegion.delta() < r.delta()) bestRegion = r;
            
            return bestRegion;
        }
        public static bool validRegionsListY(ImageClass.Projection p, List<Region> l)
        {
            int size = p.values.Length;
            double minCharHeight = size * 0.7; //4%
            int s = 0;
            foreach (Region r in l)
            {
                if (r.delta() > minCharHeight) s++;
            }

            return
                s == 1
                //&& l.Count < 3
                ;
        }

        public static void detectCharacterRegions(Image<Bgr, Byte> img) {
            Image<Bgr, Byte> imgcpy = img.Copy();

            List<Region> hrl = detectLPCharacterRegionsX(imgcpy);

            imgcpy = img.Copy();

            Region vr = detectLPCharacterRegionsY(imgcpy);

            ImageClass.OtsuBinarization(img);
            ImageClass.DNegative(img);

            Utils.markVRegion(hrl, img);
            List<Region> temp = new List<Region>();temp.Add(vr);
            Utils.markHRegion(temp, img);
        }

        public static List<Region> regionsListFromThreshold(ImageClass.Projection p, int t) {
            List<Region> l = new List<Region>();
            Region curr = null;
            for (int i = 0; i<p.values.Length; i++) {
                if (p.values[i] >= t)
                {
                    if (curr == null)
                    {
                        curr = new Region(i, 0);
                    }
                }
                else {
                    if (curr != null) {
                        curr.endPoint = i;
                        l.Add(curr);
                        curr = null;
                    }
                }
            }
            if (curr != null) { curr.endPoint = p.values.Length - 1; l.Add(curr); }
            return l;
        }

        

        public static bool validRegionsListX(ImageClass.Projection p, List<Region> l)
        {
            int size = p.values.Length;
            double minCharSize = size * 0.04; //4%
            double maxCharSize = size * 0.1; //10%
            int s = 0, m=0;
            foreach (Region r in l) {
                if ((r.endPoint-r.startPoint)>minCharSize) s++;
                if ((r.endPoint - r.startPoint) > maxCharSize) m++;
            } 

            return 
                //s >= 6 && 
                //s < 9 && 
                //m == 0 && 
                l.Count==10;
        }

        

        


































        /// <summary>
        /// Sobel Edge detection filter 3 x 3
        /// </summary>
        /// <param name="img">image</param>
        internal static void EdgeDetectionSobel3x3(Image<Bgr, byte> img)
        {
            Image<Bgr, byte> source = img.Clone();

            int[,] qx = {   {-1, 0, 1}, 
                            {-2, 0, 1}, 
                            {-1, 0, 1}};

            int[,] qy = {   { 1, 2, 1},
                            { 0, 0, 0},
                            {-1,-2,-1}};
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
                            int sumX=0, sumY=0;
                            for (int i = -1; i <= 1; i++)
                                for (int j = -1; j <= 1; j++)
                                {
                                    int mx = qx[i + 1, j + 1];
                                    int my = qy[i + 1, j + 1];

                                    int sx = x + i;
                                    int sy = y + j;

                                    if (sy < 0) sy = 0;
                                    else if (sy > height) sy = height - 1;
                                    if (sx < 0) sx = 0;
                                    else if (sx > width) sx = width - 1;

                                    int v = srcPtr[(lineOffset) * (sy) + nChan * (sx) + component];

                                    sumX += v * mx;
                                    sumY += v * my;
                                }



                            *(dataPtr++) = (byte) (Math.Abs(sumX) + Math.Abs(sumY));
                        }
                    }
                    //at the end of the line advance the pointer by the aligment bytes (padding)
                    dataPtr += padding;
                }
            }

        }

        
    }
}
