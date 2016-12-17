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
        private static bool _DEB = true;

        public class Region {
            public int startPoint, endPoint;
            public Region(int s, int e) { startPoint = s; endPoint = e; }
            public int delta { get { return endPoint - startPoint; } }
            override public String ToString() { return "[" + startPoint + " > " + endPoint + "]"; }
            public bool inside(int p) { return p > startPoint && p < endPoint; }
            public System.Drawing.Rectangle x(Region r){return new System.Drawing.Rectangle(startPoint,r.startPoint,delta,r.delta);}
        }

        public static MainForm.ImageUpdateble iu = null;

        public static List<Region> detectLPCharacterHorizontally(Image<Bgr, Byte> img)
        {
            //ImageClass.EdgeDetectionSobel3x3Y(img);
            //ImageClass.DNegative(img);
            //ImageClass.OtsuBinarization(img);
            ImageClass.Projection p = ImageClass.HProjection(img);

            List<Region> rl = null;

            //new GraphXY(p.values, new int[1]);
            Utils.applyMovingAverage(p, 0.005);
            new GraphXY(p.values, new int[1]);

            for (int t = 0; t < p.peak; t++) {
                List<Region> l = regionsListFromThreshold(p, t);
                //l = joinAdjointRegions(l);
                if (validRegionsListX(p, l)) { rl = l; break; }
            }

            //if (rl != null) rl = joinAdjointRegions(rl);
            if (rl != null) {
                Console.WriteLine("ajefhçwehfçiuwehfçiquwehfçi");
                foreach (Region r in rl)
                Utils.markVRegion(rl, img);
                iu.updateImage(img);
            }
                
            return rl;
        }

        static List<Region> joinAdjointRegions(List<Region> rs) {
            List<Region> nrs = new List<Region>();
            Region pr=null;
            foreach (Region r in rs) {
                if (pr == null) {
                    pr = r;
                } else if (r.startPoint - pr.endPoint <= 1) {
                    pr = new Region(pr.startPoint,r.endPoint);
                } else {
                    nrs.Add(pr);
                    pr = r;
                }
            }
            if (pr != null) nrs.Add(pr);
            return nrs;
        }

        public static bool validRegionsListX(ImageClass.Projection p, List<Region> l)
        {
            int size = p.values.Length;
            double minCharSize = size * 0.04; //4%
            double maxCharSize = size * 0.15; //10%
            int s = 0, m = 0, g = 0;
            foreach (Region r in l)
            {
                if (r.delta > minCharSize) s++;
                if (r.delta < minCharSize) g++;
                if (r.delta > maxCharSize) m++;
            }

            return
                s >= 6 &&
                //s < 9 && 
                m == 0 &&
                //g <= 2 &&
                l.Count >= 6 && l.Count <= 10;
        }

        public static Region findCentralRegion(Image<Bgr, Byte> img)
        {
            //ImageClass.EdgeDetectionSobel3x3X(img);
            //ImageClass.DNegative(img);
            //ImageClass.OtsuBinarization(img);

            ImageClass.Projection p = ImageClass.VProjection(img);
            List<Region> rl = null;

            //new GraphXY(new int[1], p.values);
            Utils.applyMovingAverage(p, 0.01);
            Console.WriteLine("[Finding central region {0}]", p.peak);
            int middle = img.Height / 2;
            int start = p.peak/8;
            for (int i = 0; i < p.peak ; i++)
            {
                int t = start + ((i%2==0)?(i/2):(-i));
                if (t > p.peak || t < 0) continue;

                List<Region> l = regionsListFromThreshold(p, t);
                Region r = getMiddleRegion(l, middle);
                if (r != null) {
                    Console.WriteLine("peak:{0}, found threshold:{1}", p.peak, t);
                    new GraphXY(new int[1], p.values, "Threshold: "+t);
                    return r;
                }
            }

            /*
            foreach(Region r in regionsListFromThreshold(p, 60)) { Console.WriteLine(r); }

           

            Region bestRegion = null;
            foreach (Region r in rl) if (bestRegion == null || bestRegion.delta < r.delta) bestRegion = r;
            
            return bestRegion;
            */
            return null;
        }
        public static bool validRegionsListY(ImageClass.Projection p, List<Region> l)
        {
            int size = p.values.Length;
            double minCharHeight = size * 0.6; //60%
            int s = 0;
            foreach (Region r in l)
            {
                if (r.delta > minCharHeight) s++;
            }

            return
                s == 1
                //&& l.Count < 3
                ;
        }
        static Region getMiddleRegion(List<Region> rl, int m) {
            foreach (Region r in rl) {
                if (r.inside(m)) return r;
            }
            return null;
        }

        public static List<Image<Bgr, Byte>> detectCharacterRegions(Image<Bgr, Byte> _img, MainForm.ImageUpdateble u) {
            Image<Bgr, Byte> img = _img;

            ImageClass.DNegative(img);
            ImageClass.OtsuBinarization(img);

            List<Region> hrl = detectLPCharacterHorizontally(img.Copy());

            List<Image<Bgr, Byte>> charImages = new List<Image<Bgr, byte>>();

            //ImageClass.OtsuBinarization(img);
            //ImageClass.DNegative(img);

            if (_DEB) { 
                //if (hrl != null) Utils.markVRegion(hrl, _img);
                //List<Region> temp = new List<Region>(); temp.Add(vr);
                //Utils.markHRegion(temp, _img);
            }

            if(hrl!=null)
                foreach (Region r in hrl){
                    //if(u!=null)u.updateImage(Utils.cutXRegion(img, r));

                    Image<Bgr, Byte> ci = img.Copy(new System.Drawing.Rectangle(r.startPoint,0,r.delta,img.Height));//Utils.cutXRegion(img, r);

                    Region vr = findCentralRegion(ci);

                    if (vr == null) continue;

                    System.Drawing.Rectangle rectangle = r.x(vr);

                    Image<Bgr, Byte> vc = img.Copy(rectangle);


                    Utils.markRectangle(rectangle, _img);
                    
                        charImages.Add(vc);
                    iu.updateImage(vc);
                    //System.Threading.Thread.Sleep(1000);
                }
            iu.updateImage(_img);
            //if(u!=null)u.updateImage(Utils.cutYRegion(img, vr));

            return charImages;
        }

        public static string read(Image<Bgr,Byte> img) {
            List<Image<Bgr, Byte>> charImages = detectCharacterRegions(img, null);
            string lp="";
            foreach (Image<Bgr, Byte> ci in charImages) {
                char character;
                
                double confidence = CharDB.GetInstance().match(ci, out character);
                //Console.WriteLine("{0} ({1})", character, confidence);
                if (confidence > 0.7) {
                    lp += character;
                }
            }
            Console.WriteLine(lp);
            return lp;
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
